using System;
using System.Collections.Generic;
using System.Linq;

namespace NuDeploy.Core.Commands
{
    public class CommandLineArgumentInterpreter : ICommandLineArgumentInterpreter
    {
        private readonly ICommandProvider commandProvider;

        private readonly ICommandNameMatcher commandNameMatcher;

        private readonly ICommandArgumentParser commandArgumentParser;

        private readonly ICommandArgumentNameMatcher commandArgumentNameMatcher;

        public CommandLineArgumentInterpreter(ICommandProvider commandProvider, ICommandNameMatcher commandNameMatcher, ICommandArgumentParser commandArgumentParser, ICommandArgumentNameMatcher commandArgumentNameMatcher)
        {
            this.commandProvider = commandProvider;
            this.commandNameMatcher = commandNameMatcher;
            this.commandArgumentParser = commandArgumentParser;
            this.commandArgumentNameMatcher = commandArgumentNameMatcher;
        }

        public ICommand GetCommand(IList<string> commandLineArguments)
        {
            if (commandLineArguments == null)
            {
                throw new ArgumentNullException("commandLineArguments");
            }

            if (!commandLineArguments.Any())
            {
                return null;
            }

            IList<ICommand> availableCommands = this.commandProvider.GetAvailableCommands();
            if (!availableCommands.Any())
            {
                return null;
            }

            // find command
            string commandName = commandLineArguments.First();
            IList<ICommand> matchingCommands = availableCommands.Where(cmd => this.commandNameMatcher.IsMatch(cmd, commandName)).ToList();

            if (matchingCommands.Count == 0)
            {
                // no match
                return null;
            }

            if (matchingCommands.Count > 1)
            {
                // ambiguous command name 
                return null;
            }

            ICommand command = matchingCommands.First();

            // assign command arguments (if available)
            if (commandLineArguments.Count > 1)
            {
                int argumentPosition = 0;
                int unrecognizedArgumentIndex = 1;
                var commandArguments = this.commandArgumentParser.ParseParameters(commandLineArguments.Skip(1).Take(commandLineArguments.Count - 1));
                foreach (var commandArgument in commandArguments)
                {
                    string suppliedArgumentName = commandArgument.Key;
                    string suppliedArgumentValue = commandArgument.Value;

                    // named argument
                    string matchedArgumentName =
                        command.Attributes.RequiredArguments.FirstOrDefault(
                            originalArgumentName => this.commandArgumentNameMatcher.IsMatch(originalArgumentName, suppliedArgumentName));

                    if (matchedArgumentName == null)
                    {
                        // positional argument
                        if (string.IsNullOrWhiteSpace(suppliedArgumentName))
                        {
                            matchedArgumentName =
                                command.Attributes.PositionalArguments.Where((s, positionalArgIndex) => positionalArgIndex == argumentPosition).FirstOrDefault();
                        }

                        // unnamed argument
                        if (matchedArgumentName == null)
                        {
                            if (string.IsNullOrWhiteSpace(suppliedArgumentName))
                            {
                                // unnamed argument without a name
                                matchedArgumentName = string.Format("Unnamed-Argument-{0}", unrecognizedArgumentIndex++);
                            }
                            else
                            {
                                // unrecognized argument
                                matchedArgumentName = suppliedArgumentName;
                            }                            
                        }
                    }

                    command.Arguments[matchedArgumentName] = suppliedArgumentValue;
                    argumentPosition++;
                }
            }

            return command;
        }
    }
}