using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Status;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class InstallationStatusCommand : ICommand
    {
        public const string CommandName = "status";

        public const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "installationstatus" };

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        public InstallationStatusCommand(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (installationStatusProvider == null)
            {
                throw new ArgumentNullException("installationStatusProvider");
            }

            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new string[] { },
                Description = Resources.InstallationStatusCommand.CommandDescriptionText,
                Usage = string.Format("{0}", CommandName),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.InstallationStatusCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                            Resources.InstallationStatusCommand.CommandExampleDescription2
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.InstallationStatusCommand.ArgumentDescriptionNugetPackageId }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // package id
            string packageId = this.Arguments.ContainsKey(ArgumentNameNugetPackageId) ? this.Arguments[ArgumentNameNugetPackageId] : string.Empty;

            // retrieve packages
            IList<NuDeployPackageInfo> packages = string.IsNullOrWhiteSpace(packageId)
                                                      ? this.installationStatusProvider.GetPackageInfo().ToList()
                                                      : this.installationStatusProvider.GetPackageInfo(packageId).ToList();

            // abort if no packages are returned
            if (packages.Count == 0)
            {
                this.userInterface.WriteLine(
                    string.IsNullOrWhiteSpace(packageId)
                        ? Resources.InstallationStatusCommand.NoPackagesInstalledMessage
                        : string.Format(Resources.InstallationStatusCommand.NoInstancesOfPackageInstalledMessageTemplate, packageId));

                return true;
            }

            // display package installation status
            var dataToDisplay = new Dictionary<string, string>
                {
                    { Resources.InstallationStatusCommand.InstallationStatusTableHeadlineColumn1, Resources.InstallationStatusCommand.InstallationStatusTableHeadlineColumn2 },
                    { new string('-', Resources.InstallationStatusCommand.InstallationStatusTableHeadlineColumn1.Length + 3), new string('-', Resources.InstallationStatusCommand.InstallationStatusTableHeadlineColumn2.Length + 3) },
                    { string.Empty, string.Empty },
                };

            foreach (NuDeployPackageInfo package in packages)
            {
                string key = string.Format(Resources.InstallationStatusCommand.InstallationStatusTableKeyColumnTemplate, package.Id, package.Version);
                string value = package.IsInstalled
                                   ? Resources.InstallationStatusCommand.PackageIsInstalled
                                   : Resources.InstallationStatusCommand.PackageIsNotInstalled;

                dataToDisplay.Add(key, value);
            }

            this.userInterface.ShowKeyValueStore(dataToDisplay, 4);
            return true;
        }
    }
}