namespace AIMeeting.Core.Configuration
{
    /// <summary>
    /// Parses agent configuration files in text format.
    /// Format: KEY: VALUE with support for multi-line sections (PERSONA, INSTRUCTIONS, etc)
    /// </summary>
    public class AgentConfigurationParser
    {
        private const int MaxFileSizeBytes = 64 * 1024; // 64 KB

        /// <summary>
        /// Parses an agent configuration file from a file path.
        /// </summary>
        /// <param name="filePath">Path to the configuration file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Parsed configuration or parse errors</returns>
        public async Task<AgentConfigurationParseResult> ParseAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var content = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8, cancellationToken);

                // Validate file size
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > MaxFileSizeBytes)
                {
                    return AgentConfigurationParseResult.Failure(
                        new ParseError($"File size exceeds maximum of {MaxFileSizeBytes} bytes", 1));
                }

                return Parse(content);
            }
            catch (IOException ex)
            {
                return AgentConfigurationParseResult.Failure(
                    new ParseError($"Failed to read file: {ex.Message}", 0));
            }
            catch (Exception ex) when (ex is ArgumentException || ex.Message.Contains("encoding"))
            {
                return AgentConfigurationParseResult.Failure(
                    new ParseError($"File is not valid UTF-8: {ex.Message}", 1));
            }
        }

        /// <summary>
        /// Parses an agent configuration from a string.
        /// </summary>
        /// <param name="content">The configuration content as a string</param>
        /// <returns>Parsed configuration or parse errors</returns>
        public AgentConfigurationParseResult Parse(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return AgentConfigurationParseResult.Failure(
                    new ParseError("Configuration content is empty", 1));
            }

            var config = new AgentConfiguration();
            var lines = NormalizeLineEndings(content).Split('\n');
            var errors = new List<ParseError>();

            int lineNumber = 0;
            int currentLineInFile = 1;

            while (lineNumber < lines.Length)
            {
                var line = lines[lineNumber];
                lineNumber++;

                // Skip blank lines and trim whitespace
                var trimmed = line.TrimEnd();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    currentLineInFile++;
                    continue;
                }

                // Skip comment lines
                if (trimmed.StartsWith("#"))
                {
                    currentLineInFile++;
                    continue;
                }

                // Parse header (KEY: VALUE or KEY: for multi-line sections)
                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex <= 0)
                {
                    errors.Add(new ParseError($"Invalid line format (expected 'KEY: VALUE'): {trimmed}", currentLineInFile));
                    currentLineInFile++;
                    continue;
                }

                var header = trimmed[..colonIndex].Trim();
                var value = trimmed[(colonIndex + 1)..].Trim();

                // Validate header format: must be alphanumeric, underscore, or space
                if (!IsValidHeaderName(header))
                {
                    errors.Add(new ParseError($"Invalid header name '{header}': use alphanumeric characters, underscores, and spaces only", currentLineInFile));
                    currentLineInFile++;
                    continue;
                }

                currentLineInFile++;

                // Handle multi-line sections (like PERSONA and INSTRUCTIONS)
                if (header is "PERSONA" or "INSTRUCTIONS")
                {
                    var items = new List<string>();

                    // Add initial value if present
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (value.StartsWith("-"))
                        {
                            items.Add(value[1..].Trim());
                        }
                        else
                        {
                            items.Add(value);
                        }
                    }

                    // Read continuation lines starting with '-'
                    while (lineNumber < lines.Length)
                    {
                        var nextLine = lines[lineNumber];
                        var nextTrimmed = nextLine.TrimEnd();

                        // Check if this is a new header (has ':')
                        if (nextTrimmed.Contains(':') && !nextTrimmed.StartsWith("#") && !string.IsNullOrWhiteSpace(nextTrimmed))
                        {
                            var nextColonIndex = nextTrimmed.IndexOf(':');
                            var nextHeaderPart = nextTrimmed[..nextColonIndex].Trim();
                            if (IsValidHeaderName(nextHeaderPart) && !nextTrimmed.StartsWith("-"))
                            {
                                // This is a new section, stop here
                                break;
                            }
                        }

                        // Skip blank lines and comments within section
                        if (string.IsNullOrWhiteSpace(nextTrimmed) || nextTrimmed.StartsWith("#"))
                        {
                            lineNumber++;
                            currentLineInFile++;
                            continue;
                        }

                        // Parse list item (must start with '-')
                        if (nextTrimmed.StartsWith("-"))
                        {
                            var item = nextTrimmed[1..].Trim();
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                items.Add(item);
                            }
                            lineNumber++;
                            currentLineInFile++;
                        }
                        else
                        {
                            // Not a list item, assume new section
                            break;
                        }
                    }

                    if (header == "PERSONA")
                    {
                        config.PersonaTraits = items;
                    }
                    else if (header == "INSTRUCTIONS")
                    {
                        config.Instructions = items;
                    }
                }
                else if (header == "ROLE")
                {
                    config.Role = value;
                }
                else if (header == "DESCRIPTION")
                {
                    config.Description = value;
                }
                else if (header == "INITIAL_MESSAGE_TEMPLATE")
                {
                    config.InitialMessageTemplate = value;
                }
                else if (header == "RESPONSE_STYLE")
                {
                    config.ResponseStyle = value;
                }
                else if (header == "MAX_MESSAGE_LENGTH")
                {
                    if (int.TryParse(value, out var maxLength) && maxLength > 0)
                    {
                        config.MaxMessageLength = maxLength;
                    }
                    else
                    {
                        errors.Add(new ParseError($"MAX_MESSAGE_LENGTH must be a positive integer, got '{value}'", currentLineInFile - 1));
                    }
                }
                else if (header == "EXPERTISE_AREAS")
                {
                    var areas = value.Split(',')
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();
                    config.ExpertiseAreas = areas;
                }
                else if (header == "COMMUNICATION_APPROACH")
                {
                    config.CommunicationApproach = value;
                }
                else
                {
                    // Unknown header - store in custom fields and emit warning
                    config.CustomFields[header] = value;
                    errors.Add(new ParseError($"Unknown header '{header}': will be stored but not used", currentLineInFile - 1, isWarning: true));
                }
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(config.Role))
            {
                errors.Insert(0, new ParseError("Missing required field: ROLE", 1));
            }
            if (string.IsNullOrWhiteSpace(config.Description))
            {
                errors.Insert(0, new ParseError("Missing required field: DESCRIPTION", 1));
            }
            if (config.Instructions.Count == 0)
            {
                errors.Insert(0, new ParseError("Missing required field: INSTRUCTIONS (must have at least one item)", 1));
            }

            // Return result (may contain warnings even on success)
            var result = new AgentConfigurationParseResult
            {
                Configuration = config,
                Errors = errors
            };

            return result;
        }

        private static string NormalizeLineEndings(string content)
        {
            // Normalize all line endings to \n
            return content
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }

        private static bool IsValidHeaderName(string header)
        {
            // Valid: alphanumeric, underscores, spaces
            return !string.IsNullOrEmpty(header) && header.All(c => char.IsLetterOrDigit(c) || c == '_' || c == ' ');
        }
    }

    /// <summary>
    /// Result of parsing an agent configuration.
    /// </summary>
    public class AgentConfigurationParseResult
    {
        public required AgentConfiguration Configuration { get; set; }
        public List<ParseError> Errors { get; set; } = new();

        /// <summary>
        /// True if parse succeeded (no fatal errors, warnings are ok).
        /// </summary>
        public bool IsSuccess => !Errors.Any(e => !e.IsWarning);

        public static AgentConfigurationParseResult Failure(ParseError error) => new()
        {
            Configuration = new AgentConfiguration(),
            Errors = [error]
        };

        public static AgentConfigurationParseResult Failure(params ParseError[] errors) => new()
        {
            Configuration = new AgentConfiguration(),
            Errors = [..errors]
        };
    }

    /// <summary>
    /// A parse error or warning.
    /// </summary>
    public class ParseError
    {
        public string Message { get; }
        public int LineNumber { get; }
        public bool IsWarning { get; }

        public ParseError(string message, int lineNumber, bool isWarning = false)
        {
            Message = message;
            LineNumber = lineNumber;
            IsWarning = isWarning;
        }

        public override string ToString()
        {
            var severity = IsWarning ? "Warning" : "Error";
            return $"[{severity}] Line {LineNumber}: {Message}";
        }
    }
}
