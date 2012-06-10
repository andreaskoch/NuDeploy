using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        public HelpCommand(IUserInterface userInterface, ApplicationInformation applicationInformation)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;

            this.Attributes = new CommandAttributes
                {
                    CommandName = "help",
                    AlternativeCommandNames = new[] { "?" }
                };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            this.userInterface.Show("{0} Version: {1}", this.applicationInformation.ApplicationName, this.applicationInformation.ApplicationVersion);
            this.userInterface.Show("usage: {0} <command> [args] [options] ", this.applicationInformation.NameOfExecutable);
            this.userInterface.Show("Type '{0} help <command>' for help on a specific command.", this.applicationInformation.NameOfExecutable);
            this.userInterface.Show(string.Empty);

            this.userInterface.Show("Available commands:");
            this.userInterface.Show(string.Empty);
        }
    }
}