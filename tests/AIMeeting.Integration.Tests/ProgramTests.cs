using System.Reflection;
using Xunit;

namespace AIMeeting.Integration.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Main_WithHelp_ReturnsSuccess()
        {
            var programType = typeof(AIMeeting.CLI.Commands.StartMeetingCommandBuilder)
                .Assembly
                .GetType("AIMeeting.CLI.Program", throwOnError: true);

            var mainMethod = programType!.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(mainMethod);

            var result = (int)mainMethod!.Invoke(null, [new[] { "--help" }])!;

            Assert.Equal(0, result);
        }
    }
}
