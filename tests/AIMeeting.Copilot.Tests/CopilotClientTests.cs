using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AIMeeting.Copilot;
using AIMeeting.Core.Exceptions;
using Xunit;

namespace AIMeeting.Copilot.Tests
{
    public class CopilotClientTests
    {
        [Fact]
        public void CopilotOptions_Defaults_To_ThirtySeconds()
        {
            var options = new CopilotOptions();

            Assert.Equal(TimeSpan.FromSeconds(30), options.Timeout);
        }

        [Fact]
        public void NewClient_IsNotConnected()
        {
            var client = new CopilotClient();

            Assert.False(client.IsConnected);
        }

        [Fact]
        public async Task GenerateAsync_Throws_On_EmptyPrompt()
        {
            var client = new CopilotClient();

            await Assert.ThrowsAsync<ArgumentException>(() => client.GenerateAsync(""));
        }

        [Fact]
        public async Task GenerateAsync_Throws_When_NotConnected()
        {
            var client = new CopilotClient();

            await Assert.ThrowsAsync<CopilotApiException>(() => client.GenerateAsync("Hello"));
        }

        [Fact]
        public async Task StopAsync_NoConnection_DoesNotThrow()
        {
            var client = new CopilotClient();

            await client.StopAsync();
        }

        [Fact]
        public async Task GenerateAsync_ReturnsResponse_WhenConnected()
        {
            var client = new CopilotClient();
            using var stdinStream = new MemoryStream();
            using var stdin = new StreamWriter(stdinStream) { AutoFlush = true };
            using var stdout = CreateStdout("Line one\nLine two\n---END---\n");

            SetPrivateField(client, "_stdin", stdin);
            SetPrivateField(client, "_stdout", stdout);
            SetPrivateField(client, "_isConnected", true);

            var result = await client.GenerateAsync("Hello");

            Assert.Equal("Line one\nLine two", result);
        }

        [Fact]
        public async Task GenerateAsync_Truncates_WhenMaxLengthProvided()
        {
            var client = new CopilotClient();
            using var stdinStream = new MemoryStream();
            using var stdin = new StreamWriter(stdinStream) { AutoFlush = true };
            using var stdout = CreateStdout("Hello World\n---END---\n");

            SetPrivateField(client, "_stdin", stdin);
            SetPrivateField(client, "_stdout", stdout);
            SetPrivateField(client, "_isConnected", true);

            var result = await client.GenerateAsync("Hello", new CopilotOptions { MaxResponseLength = 5 });

            Assert.Equal("Hello", result);
        }

        [Fact]
        public async Task StartAsync_Throws_WhenGhMissing()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), $"AIMeeting.Tests.{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);

            var originalPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            var originalOverride = Environment.GetEnvironmentVariable("AIMEETING_GH_PATH");
            try
            {
                Environment.SetEnvironmentVariable("PATH", tempDir);
                Environment.SetEnvironmentVariable("AIMEETING_GH_PATH", null);

                var client = new CopilotClient();

                var ex = await Assert.ThrowsAsync<CopilotApiException>(() => client.StartAsync());

                Assert.Contains("Copilot", ex.Message);
            }
            finally
            {
                Environment.SetEnvironmentVariable("PATH", originalPath);
                Environment.SetEnvironmentVariable("AIMEETING_GH_PATH", originalOverride);
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
        }

        [Fact]
        public async Task StartAsync_UsesFakeGh_Process()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), $"AIMeeting.Tests.{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);

            var originalOverride = Environment.GetEnvironmentVariable("AIMEETING_GH_PATH");
            try
            {
                var fakeGh = CreateFakeGh(tempDir);
                Environment.SetEnvironmentVariable("AIMEETING_GH_PATH", fakeGh);

                var client = new CopilotClient();
                await client.StartAsync();

                Assert.True(client.IsConnected);

                await client.StopAsync();
                Assert.False(client.IsConnected);
            }
            finally
            {
                Environment.SetEnvironmentVariable("AIMEETING_GH_PATH", originalOverride);
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
        }

        private static StreamReader CreateStdout(string output)
        {
            var buffer = Encoding.UTF8.GetBytes(output);
            return new StreamReader(new MemoryStream(buffer));
        }

        private static void SetPrivateField<T>(CopilotClient client, string fieldName, T value)
        {
            var field = typeof(CopilotClient).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException($"Field '{fieldName}' not found.");
            }

            field.SetValue(client, value);
        }

        private static string CreateFakeGh(string directory)
        {
            if (OperatingSystem.IsWindows())
            {
                var scriptPath = Path.Combine(directory, "gh.cmd");
                var script = "@echo off\r\n" +
                             "if \"%1\"==\"copilot\" (\r\n" +
                             "  if \"%2\"==\"--version\" (\r\n" +
                             "    echo 0.0.0\r\n" +
                             "    exit /b 0\r\n" +
                             "  )\r\n" +
                             "  if \"%2\"==\"-s\" (\r\n" +
                             "    :loop\r\n" +
                             "    set /p line=\r\n" +
                             "    if \"%line%\"==\"exit\" exit /b 0\r\n" +
                             "    if \"%line%\"==\"---END---\" (\r\n" +
                             "      echo stub\r\n" +
                             "      echo ---END---\r\n" +
                             "      exit /b 0\r\n" +
                             "    )\r\n" +
                             "    goto loop\r\n" +
                             "  )\r\n" +
                             ")\r\n" +
                             "exit /b 1\r\n";

                File.WriteAllText(scriptPath, script, Encoding.ASCII);
                return scriptPath;
            }

            var scriptPathUnix = Path.Combine(directory, "gh");
            var scriptUnix = "#!/bin/sh\n" +
                             "if [ \"$1\" = \"copilot\" ] && [ \"$2\" = \"--version\" ]; then\n" +
                             "  echo 0.0.0\n" +
                             "  exit 0\n" +
                             "fi\n" +
                             "if [ \"$1\" = \"copilot\" ] && [ \"$2\" = \"-s\" ]; then\n" +
                             "  while IFS= read -r line; do\n" +
                             "    if [ \"$line\" = \"exit\" ]; then\n" +
                             "      exit 0\n" +
                             "    fi\n" +
                             "    if [ \"$line\" = \"---END---\" ]; then\n" +
                             "      echo stub\n" +
                             "      echo ---END---\n" +
                             "      exit 0\n" +
                             "    fi\n" +
                             "  done\n" +
                             "  exit 0\n" +
                             "fi\n" +
                             "exit 1\n";

            File.WriteAllText(scriptPathUnix, scriptUnix, Encoding.ASCII);
            File.SetUnixFileMode(scriptPathUnix, UnixFileMode.UserRead | UnixFileMode.UserExecute | UnixFileMode.UserWrite);
            return scriptPathUnix;
        }

    }
}
