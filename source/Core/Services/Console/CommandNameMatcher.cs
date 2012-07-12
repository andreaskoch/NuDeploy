using System;
using System.Linq;
using System.Text.RegularExpressions;

using NuDeploy.Core.Commands;

namespace NuDeploy.Core.Services.Console
{
    public class CommandNameMatcher : ICommandNameMatcher
    {
        private const StringComparison ComparisionMethod = StringComparison.OrdinalIgnoreCase;

        private readonly Regex commandModifiersRegex = new Regex("^(--|-|/)");

        public bool IsMatch(ICommand command, string commandName)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (commandName == null)
            {
                throw new ArgumentNullException("commandName");
            }

            // prepare command name for comparision
            commandName = commandName.Trim();
            commandName = this.commandModifiersRegex.Replace(commandName, string.Empty);

            // full match
            if (command.Attributes.CommandName.Equals(commandName, ComparisionMethod))
            {
                return true;
            }

            // partial match
            if (command.Attributes.CommandName.StartsWith(commandName, ComparisionMethod))
            {
                return true;
            }

            // alternative name match (full)
            if (command.Attributes.AlternativeCommandNames.Any(alternativeCommandName => alternativeCommandName.Equals(commandName, ComparisionMethod)))
            {
                return true;
            }

            // no match
            return false;
        }
    }
}