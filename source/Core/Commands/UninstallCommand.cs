using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

using NuGet;

namespace NuDeploy.Core.Commands
{
    public class UninstallCommand : ICommand
    {
        private const string CommandName = "uninstall";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "remove" };

        private readonly IUserInterface userInterface;

        private readonly IPackageRepository packageRepository;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageInstaller packageInstaller;

        public UninstallCommand(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageInstaller packageInstaller, IPackageRepository packageRepository)
        {
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageInstaller = packageInstaller;
            this.packageRepository = packageRepository;

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
            string packageId = this.Arguments.Values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(packageId))
            {
                this.userInterface.WriteLine("No package id specified.");
                return;
            }

            // check if package is installed
            if (!this.installationStatusProvider.IsInstalled(packageId))
            {
                this.userInterface.WriteLine(string.Format("Package \"{0}\" is not installed in the folder \"{1}\".", packageId, this.packageRepository.Source));
                return;
            }

            // remove the package
            NuDeployPackageInfo packageInfoOfInstalledVersion = this.installationStatusProvider.GetPackageInfo(packageId);

            this.userInterface.WriteLine(
                string.Format("Removing package \"{0}\" from folder \"{1}\".", packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Folder));

            bool result = this.packageInstaller.Uninstall(packageInfoOfInstalledVersion);
            if (result)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "{0} (Version: {1}) has been successfully uninstalled.", packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Version));
            }
            else
            {
                this.userInterface.WriteLine(
                    string.Format(
                        "The uninstallation of the package \"{0} (Version: {1})\" failed.",
                        packageInfoOfInstalledVersion.Id,
                        packageInfoOfInstalledVersion.Version));
            }
        }
    }
}