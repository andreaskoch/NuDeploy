using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;

using StructureMap;

namespace NuDeploy.Core.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private IList<ICommand> availableCommands;

        public HelpCommand(IUserInterface userInterface, ApplicationInformation applicationInformation)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;

            this.Attributes = new CommandAttributes
                {
                    CommandName = "help",
                    AlternativeCommandNames = new[] { "?" },
                    Description = Resources.HelpCommand.CommandDescriptionText,
                    Usage = ""
                };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        protected IList<ICommand> AvailableCommands
        {
            get
            {
                if (this.availableCommands == null)
                {
                    var commandProvider = ObjectFactory.GetInstance<ICommandProvider>();
                    this.availableCommands = commandProvider.GetAvailableCommands();
                }

                return this.availableCommands;
            }
        }

        public void Execute()
        {
            if (this.Arguments.Count > 0 && string.IsNullOrWhiteSpace(this.Arguments.Values.First()) == false)
            {
                string commandName = this.Arguments.Values.First().Trim();
                ICommand matchedCommand =
                    this.AvailableCommands.First(
                        c =>
                        c.Attributes.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase)
                        || c.Attributes.AlternativeCommandNames.Any(a => a.Equals(commandName, StringComparison.OrdinalIgnoreCase)));

                if (matchedCommand != null)
                {
                    this.CommandHelp(matchedCommand);
                    return;
                }
            }

            this.Overview();
        }

        private void CommandHelp(ICommand command)
        {
            // command name
            string commandNameText = this.applicationInformation.NameOfExecutable + " " + command.Attributes.CommandName;
            this.userInterface.Show(commandNameText);

            this.userInterface.Show(string.Empty);

            // command description
            this.userInterface.Show(command.Attributes.Description);

            this.userInterface.Show(string.Empty);

            // command usage
            string usageLabel = Resources.HelpCommand.UsageLabel + ": ";
            string usageText = this.applicationInformation.NameOfExecutable + " " + command.Attributes.Usage;
            this.userInterface.ShowLabelValuePair(usageLabel, usageText, distanceBetweenLabelAndValue: 1);

            this.userInterface.Show(string.Empty);

            // examples
            string examplesLabel = Resources.HelpCommand.ExamplesLabel + ":";
            this.userInterface.Show(examplesLabel);
            this.userInterface.Show(string.Empty);

            foreach (KeyValuePair<string, string> pair in command.Attributes.Examples)
            {
                string commandText = string.Format("{0} {1}", this.applicationInformation.NameOfExecutable, pair.Key);
                string description = pair.Value;

                this.userInterface.Show(description);

                this.userInterface.Show(string.Empty);
                this.userInterface.Show(commandText, 4);
                this.userInterface.Show(string.Empty);
            }
        }

        private void Overview()
        {
            this.userInterface.Show("{0} Version: {1}", this.applicationInformation.ApplicationName, this.applicationInformation.ApplicationVersion.ToString());
            this.userInterface.Show("usage: {0} <command> [args] [options] ", this.applicationInformation.NameOfExecutable);
            this.userInterface.Show("Type '{0} help <command>' for help on a specific command.", this.applicationInformation.NameOfExecutable);
            this.userInterface.Show(string.Empty);

            this.userInterface.Show("Available commands:");
            this.userInterface.Show(string.Empty);

            // display command name and description
            this.userInterface.ShowKeyValueStore(
                this.AvailableCommands.OrderBy(c => c.Attributes.CommandName).ToDictionary(g => g.Attributes.CommandName, v => v.Attributes.Description),
                distanceBetweenColumns: 4,
                indentation: 2);
        }
    }
}