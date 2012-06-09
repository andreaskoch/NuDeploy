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
            ICommand command = availableCommands.FirstOrDefault(cmd => this.commandNameMatcher.IsMatch(cmd, commandName));
            if (command == null)
            {
                return null;
            }

            // assign command arguments (if available)
            if (commandLineArguments.Count > 1)
            {
                int unrecognizedArgumentIndex = 1;
                IDictionary<string, string> commandArguments = this.commandArgumentParser.ParseParameters(commandLineArguments.Skip(1).Take(commandLineArguments.Count - 1));
                foreach (var commandArgument in commandArguments)
                {
                    string suppliedArgumentName = commandArgument.Key;
                    string suppliedArgumentValue = commandArgument.Value;

                    string matchedArgumentName =
                        command.Arguments.Keys.FirstOrDefault(
                            originalArgumentName => this.commandArgumentNameMatcher.IsMatch(originalArgumentName, suppliedArgumentName));

                    if (matchedArgumentName == null)
                    {
                        if (string.IsNullOrWhiteSpace(suppliedArgumentName))
                        {
                            matchedArgumentName = string.Format("Unnamed-Argument-{0}", unrecognizedArgumentIndex++);
                        }
                        else
                        {
                            matchedArgumentName = suppliedArgumentName;
                        }
                    }

                    command.Arguments[matchedArgumentName] = suppliedArgumentValue;
                }
            }

            return command;
        }
    }
}