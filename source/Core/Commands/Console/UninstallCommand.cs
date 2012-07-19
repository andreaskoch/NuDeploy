using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;

namespace NuDeploy.Core.Commands.Console
{
    public class UninstallCommand : ICommand
    {
        private const string CommandName = "uninstall";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "remove" };

        private readonly IUserInterface userInterface;

        private readonly IPackageUninstaller packageUninstaller;

        public UninstallCommand(IUserInterface userInterface, IPackageUninstaller packageUninstaller)
        {
            this.userInterface = userInterface;
            this.packageUninstaller = packageUninstaller;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[] { ArgumentNameNugetPackageId },
                PositionalArguments = new[] { ArgumentNameNugetPackageId },
                Description = Resources.UninstallCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                            Resources.UninstallCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.UninstallCommand.ArgumentDescriptionNugetPackageId }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            // package id (required parameter)
            string packageId = this.Arguments.Values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(packageId))
            {
                this.userInterface.WriteLine(Resources.UninstallCommand.NoPackageIdSpecifiedMessage);
                return;
            }

            this.packageUninstaller.Uninstall(packageId, null);
        }
    }
}