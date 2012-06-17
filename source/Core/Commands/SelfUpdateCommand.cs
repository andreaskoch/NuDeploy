using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public class SelfUpdateCommand : ICommand
    {
        private const string CommandName = "selfupdate";

        private readonly string[] alternativeCommandNames = new[] { "update" };

        public SelfUpdateCommand()
        {
            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new string[] { },
                Description = Resources.SelfUpdateCommand.CommandDescriptionText,
                Usage = string.Format("{0}", CommandName),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.SelfUpdateCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>()
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}