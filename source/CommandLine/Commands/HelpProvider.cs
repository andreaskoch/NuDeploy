using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;

namespace NuDeploy.CommandLine.Commands
{
    public class HelpProvider : IHelpProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IUserInterface userInterface;

        public HelpProvider(ApplicationInformation applicationInformation, IUserInterface userInterface)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            this.applicationInformation = applicationInformation;
            this.userInterface = userInterface;
        }

        public void ShowHelp(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            // command name
            string commandNameText = this.applicationInformation.NameOfExecutable + " " + command.Attributes.CommandName;
            this.userInterface.WriteLine(commandNameText);

            this.userInterface.WriteLine(string.Empty);

            // command description
            this.userInterface.WriteLine(command.Attributes.Description);

            this.userInterface.WriteLine(string.Empty);

            // command usage
            string usageLabel = Resources.HelpCommand.UsageLabel + ":";
            string usageText = this.applicationInformation.NameOfExecutable + " " + command.Attributes.Usage;
            this.userInterface.ShowLabelValuePair(usageLabel, usageText, distanceBetweenLabelAndValue: 2);

            this.userInterface.WriteLine(string.Empty);

            // examples
            string examplesLabel = Resources.HelpCommand.ExamplesLabel + ":";
            this.userInterface.WriteLine(examplesLabel);

            foreach (KeyValuePair<string, string> pair in command.Attributes.Examples)
            {
                this.userInterface.WriteLine(string.Empty);

                string commandText = string.Format("> {0} {1}", this.applicationInformation.NameOfExecutable, pair.Key);
                string description = pair.Value;

                this.userInterface.ShowIndented(description, 3);

                this.userInterface.WriteLine(string.Empty);
                this.userInterface.ShowIndented(commandText, 6);
            }

            this.userInterface.WriteLine(string.Empty);

            // options
            if (command.Attributes.ArgumentDescriptions.Count > 0)
            {
                string optionsLabel = Resources.HelpCommand.OptionsLabel + ":";
                this.userInterface.WriteLine(optionsLabel);
                this.userInterface.WriteLine(string.Empty);

                var formattedOptions =
                    command.Attributes.ArgumentDescriptions.Select(pair => new KeyValuePair<string, string>("-" + pair.Key, pair.Value)).ToDictionary(
                        pair => pair.Key, pair => pair.Value);

                this.userInterface.ShowKeyValueStore(formattedOptions, 4, 3);

                this.userInterface.WriteLine(string.Empty);
            }
        }

        public void ShowHelpOverview(IEnumerable<ICommand> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            // version
            this.userInterface.WriteLine(string.Format("{0} ({1})", this.applicationInformation.ApplicationName, this.applicationInformation.ApplicationVersion));

            this.userInterface.WriteLine(string.Empty);

            // usage
            string usageLabel = Resources.HelpCommand.UsageLabel + ":" + Environment.NewLine;
            this.userInterface.WriteLine(usageLabel);

            string usageText = string.Format(Resources.HelpCommand.UsagePattern, this.applicationInformation.NameOfExecutable);
            this.userInterface.ShowIndented(usageText, 4);

            this.userInterface.WriteLine(string.Empty);

            // help
            string helpLabel = Resources.HelpCommand.HelpLabel + ":" + Environment.NewLine;
            this.userInterface.WriteLine(helpLabel);

            string helpText = string.Format(Resources.HelpCommand.HelpPattern, this.applicationInformation.NameOfExecutable);
            this.userInterface.ShowIndented(helpText, 4);

            this.userInterface.WriteLine(string.Empty);

            // available commands
            string availableCommandsLabel = Resources.HelpCommand.AvailableCommandsLabel + ":";
            this.userInterface.WriteLine(availableCommandsLabel);
            this.userInterface.WriteLine(string.Empty);

            // display command name and description
            this.userInterface.ShowKeyValueStore(
                commands.ToDictionary(g => g.Attributes.CommandName, v => v.Attributes.Description),
                distanceBetweenColumns: 4,
                indentation: 2);

            this.userInterface.WriteLine(string.Empty);
        }
    }
}