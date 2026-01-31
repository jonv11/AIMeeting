using System.CommandLine;
using AIMeeting.CLI.Commands;
using AIMeeting.CLI.Logging;
using Serilog;

namespace AIMeeting.CLI
{
	internal class Program
	{
		static int Main(string[] args)
		{
			LoggingBootstrapper.Configure();
			try
			{
				var rootCommand = new RootCommand("AIMeeting - Multi-Agent Meeting System");
				
				rootCommand.Subcommands.Add(ValidateConfigCommandBuilder.BuildCommand());
				rootCommand.Subcommands.Add(StartMeetingCommandBuilder.BuildCommand());

				var parseResult = rootCommand.Parse(args);
				return parseResult.Invoke();
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
	}
}
