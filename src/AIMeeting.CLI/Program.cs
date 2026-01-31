using System.CommandLine;
using AIMeeting.CLI.Commands;

namespace AIMeeting.CLI
{
	internal class Program
	{
		static int Main(string[] args)
		{
			var rootCommand = new RootCommand("AIMeeting - Multi-Agent Meeting System");
			
			rootCommand.Subcommands.Add(ValidateConfigCommandBuilder.BuildCommand());
			// start-meeting command will be added in next phase

			var parseResult = rootCommand.Parse(args);
			return parseResult.Invoke();
		}
	}
}
