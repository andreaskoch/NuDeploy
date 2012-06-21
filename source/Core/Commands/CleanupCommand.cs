using System.Collections.Generic;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Commands
{
    public class CleanupCommand : ICommand
    {
        private const string CommandName = "cleanup";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "purge" };

        private readonly IUserInterface userInterface;

        public CleanupCommand(IUserInterface userInterface)
        {
            this.userInterface = userInterface;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new[] { ArgumentNameNugetPackageId },
                Description = Resources.CleanupCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.CleanupCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                            Resources.CleanupCommand.CommandExampleDescription2
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.CleanupCommand.ArgumentDescriptionNugetPackageId }
                    }
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