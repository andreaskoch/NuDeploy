using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;

using StructureMap;

namespace NuDeploy.Core.Commands
{
    public class HelpCommand : ICommand
    {
        private const string CommandName = "help";

        private const string ArgumentNameCommandName = "CommandName";

        private readonly string[] alternativeCommandNames = new[] { "?" };

        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private IList<ICommand> availableCommands;

        public HelpCommand(IUserInterface userInterface, ApplicationInformation applicationInformation)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;

            this.Attributes = new CommandAttributes
                {
                    CommandName = CommandName,
                    AlternativeCommandNames = this.alternativeCommandNames,
                    RequiredArguments = new[] { ArgumentNameCommandName },
                    PositionalArguments = new[] { ArgumentNameCommandName },
                    Description = Resources.HelpCommand.CommandDescriptionText,
                    Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameCommandName),
                    Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.HelpCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} {1}", CommandName, "package"),
                            Resources.HelpCommand.CommandExampleDescription2
                        }
                    },
                    ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameCommandName, Resources.HelpCommand.ArgumentDescriptionCommandName }
                    }
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
            string usageLabel = Resources.HelpCommand.UsageLabel + ":";
            string usageText = this.applicationInformation.NameOfExecutable + " " + command.Attributes.Usage;
            this.userInterface.ShowLabelValuePair(usageLabel, usageText, distanceBetweenLabelAndValue: 2);

            this.userInterface.Show(string.Empty);

            // examples
            string examplesLabel = Resources.HelpCommand.ExamplesLabel + ":";
            this.userInterface.Show(examplesLabel);

            foreach (KeyValuePair<string, string> pair in command.Attributes.Examples)
            {
                this.userInterface.Show(string.Empty);

                string commandText = string.Format("> {0} {1}", this.applicationInformation.NameOfExecutable, pair.Key);
                string description = pair.Value;

                this.userInterface.ShowIndented(description, 3);

                this.userInterface.Show(string.Empty);
                this.userInterface.ShowIndented(commandText, 6);
            }

            this.userInterface.Show(string.Empty);

            // options
            if (command.Attributes.ArgumentDescriptions.Count > 0)
            {
                string optionsLabel = Resources.HelpCommand.OptionsLabel + ":";
                this.userInterface.Show(optionsLabel);
                this.userInterface.Show(string.Empty);

                var formattedOptions =
                    command.Attributes.ArgumentDescriptions.Select(pair => new KeyValuePair<string, string>("-" + pair.Key, pair.Value)).ToDictionary(
                        pair => pair.Key, pair => pair.Value);

                this.userInterface.ShowKeyValueStore(formattedOptions, 4, 3);

                this.userInterface.Show(string.Empty);                
            }
        }

        private void Overview()
        {
            // version
            this.userInterface.Show(string.Format("{0} ({1})", this.applicationInformation.ApplicationName, this.applicationInformation.ApplicationVersion));

            this.userInterface.Show(string.Empty);

            // usage
            string usageLabel = Resources.HelpCommand.UsageLabel + ":" + Environment.NewLine;
            this.userInterface.Show(usageLabel);

            string usageText = string.Format(Resources.HelpCommand.UsagePattern, this.applicationInformation.NameOfExecutable);
            this.userInterface.ShowIndented(usageText, 4);

            this.userInterface.Show(string.Empty);

            // help
            string helpLabel = Resources.HelpCommand.HelpLabel + ":" + Environment.NewLine;
            this.userInterface.Show(helpLabel);

            string helpText = string.Format(Resources.HelpCommand.HelpPattern, this.applicationInformation.NameOfExecutable);
            this.userInterface.ShowIndented(helpText, 4);

            this.userInterface.Show(string.Empty);

            // available commands
            string availableCommandsLabel = Resources.HelpCommand.AvailableCommandsLabel + ":";
            this.userInterface.Show(availableCommandsLabel);
            this.userInterface.Show(string.Empty);

            // display command name and description
            this.userInterface.ShowKeyValueStore(
                this.AvailableCommands.OrderBy(c => c.Attributes.CommandName).ToDictionary(g => g.Attributes.CommandName, v => v.Attributes.Description),
                distanceBetweenColumns: 4,
                indentation: 2);

            this.userInterface.Show(string.Empty);
        }
    }
}