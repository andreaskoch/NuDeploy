using System;
using System.Collections.Generic;
using System.Linq;

namespace NuDeploy.Core.Commands
{
    public class CommandLineArgumentParser : ICommandLineArgumentParser
    {
        private readonly ICommandProvider commandProvider;

        public CommandLineArgumentParser(ICommandProvider commandProvider)
        {
            this.commandProvider = commandProvider;
        }

        public ICommand ParseCommandLineArguments(IEnumerable<string> commandLineArguments)
        {
            if (commandLineArguments == null)
            {
                throw new ArgumentNullException("commandLineArguments");
            }

            string[] commandFragments = commandLineArguments.ToArray();
            if (commandFragments.Length == 0)
            {
                return null;
            }

            IList<ICommand> availableCommands = this.commandProvider.GetAvailableCommands();
            if (!availableCommands.Any())
            {
                return null;
            }

            foreach (string commandFragment in commandFragments)
            {
                ICommand command =
                    availableCommands.FirstOrDefault(
                        c =>
                        c.Attributes.CommandName.Equals(commandFragment, StringComparison.OrdinalIgnoreCase)
                        || c.Attributes.AlternativeCommandNames.Any(alt => alt.Equals(commandFragment, StringComparison.OrdinalIgnoreCase)));

                if (command != null)
                {
                    return command;
                }
            }

            return null;
        }
    }
}