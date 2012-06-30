using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

namespace NuDeploy.Core.Commands
{
    public class InstallCommand : ICommand
    {
        private const string CommandName = "install";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new string[] { };

        private readonly IUserInterface userInterface;

        private readonly IPackageInstaller packageInstaller;

        public InstallCommand(IUserInterface userInterface, IPackageInstaller packageInstaller)
        {
            this.userInterface = userInterface;
            this.packageInstaller = packageInstaller;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[] { ArgumentNameNugetPackageId },
                PositionalArguments = new[] { ArgumentNameNugetPackageId },
                Description = Resources.InstallCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} {1}", CommandName, "Newtonsoft.Json"),
                            Resources.InstallCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.InstallCommand.ArgumentDescriptionNugetPackageId }
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
                this.userInterface.WriteLine("No package id specified.");
                return;
            }

            // options
            bool forceInstallation =
                this.Arguments.Any(
                    pair =>
                    pair.Key.Equals(NuDeployConstants.CommonCommandOptionNameForce, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase));

            // install the package
            this.packageInstaller.Install(packageId, forceInstallation);
        }
    }
}