using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly IUserInterface userInterface;

        public HelpCommand(IUserInterface userInterface)
        {
            this.userInterface = userInterface;

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
            this.userInterface.Show("help");
        }
    }
}