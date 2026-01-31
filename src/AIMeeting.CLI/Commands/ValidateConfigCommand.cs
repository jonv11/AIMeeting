using System.CommandLine;
using AIMeeting.Core.Configuration;

namespace AIMeeting.CLI.Commands
{
    /// <summary>
    /// Command to validate an agent configuration file.
    /// </summary>
    public static class ValidateConfigCommandBuilder
    {
        public static Command BuildCommand()
        {
            var command = new Command("validate-config", "Validate an agent configuration file");
            
            var pathArgument = new Argument<string>("path")
            {
                Description = "Path to the configuration file to validate"
            };
            command.Arguments.Add(pathArgument);

            command.SetAction(parseResult =>
            {
                return ExecuteAsync(parseResult.GetValue(pathArgument) ?? string.Empty).Result;
            });

            return command;
        }

        private static async Task<int> ExecuteAsync(string path)
        {
            try
            {
                var parser = new AgentConfigurationParser();
                var validator = new AgentConfigurationValidator();

                var parseResult = await parser.ParseAsync(path);
                var validationResult = validator.Validate(parseResult.Configuration, parseResult.Errors);

                // Print all errors (both parse and validation)
                if (parseResult.Errors.Count > 0)
                {
                    foreach (var error in parseResult.Errors.Where(e => !e.IsWarning))
                    {
                        Console.Error.WriteLine($"Error: {error}");
                    }

                    // Print warnings separately
                    foreach (var warning in parseResult.Errors.Where(e => e.IsWarning))
                    {
                        Console.WriteLine($"Warning: {warning}");
                    }
                }

                if (validationResult.IsValid && parseResult.IsSuccess)
                {
                    Console.WriteLine($"✓ Configuration is valid: {path}");
                    Console.WriteLine($"  Role: {parseResult.Configuration.Role}");
                    Console.WriteLine($"  Description: {parseResult.Configuration.Description}");
                    Console.WriteLine($"  Instructions: {parseResult.Configuration.Instructions.Count}");
                    return 0;
                }
                else
                {
                    Console.Error.WriteLine("✗ Configuration validation failed");
                    foreach (var error in validationResult.Errors)
                    {
                        Console.Error.WriteLine($"  {error}");
                    }
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: Failed to validate configuration: {ex.Message}");
                return 1;
            }
        }
    }
}
