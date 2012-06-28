using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;
using NuDeploy.Core.Services;

using NuGet;

namespace NuDeploy.Core.Commands
{
    public class InstallCommand : ICommand
    {
        private const string CommandName = "install";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private const string InstallPowerShellScriptName = "Deploy.ps1";

        private readonly string[] alternativeCommandNames = new string[] { };

        private readonly IUserInterface userInterface;

        private readonly IPackageRepository packageRepository;

        private readonly PSHost powerShellHost;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageInstaller packageInstaller;

        public InstallCommand(IUserInterface userInterface, IPackageRepository packageRepository, PSHost powerShellHost, IInstallationStatusProvider installationStatusProvider, IPackageInstaller packageInstaller)
        {
            this.userInterface = userInterface;
            this.packageRepository = packageRepository;
            this.powerShellHost = powerShellHost;
            this.installationStatusProvider = installationStatusProvider;
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
                    pair.Key.Equals("Force", StringComparison.OrdinalIgnoreCase) && pair.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase));

            // fetch package
            IPackage package = this.packageRepository.FindPackage(packageId);
            if (package == null)
            {
                this.userInterface.WriteLine(string.Format("Package \"{0}\"was not found at \"{1}\".", packageId, this.packageRepository.Source));
                return;
            }

            // check if package is already installed
            if (this.installationStatusProvider.IsInstalled(package.Id))
            {
                NuDeployPackageInfo packageInfoOfInstalledVersion = this.installationStatusProvider.GetPackageInfo(package.Id);

                if (forceInstallation == false)
                {
                    if (package.Version == packageInfoOfInstalledVersion.Version)
                    {
                        this.userInterface.WriteLine(
                            string.Format(
                                "You already have the latest version installed: {0} (Version: {1}).",
                                packageInfoOfInstalledVersion.Id,
                                packageInfoOfInstalledVersion.Version));

                        return;
                    }

                    if (package.Version < packageInfoOfInstalledVersion.Version)
                    {
                        this.userInterface.WriteLine(
                            string.Format(
                                "You already have a more recent version installed: {0} (Version: {1}).",
                                packageInfoOfInstalledVersion.Id,
                                packageInfoOfInstalledVersion.Version));

                        return;
                    }                    
                }

                /* installed version is older and must be removed */
                this.userInterface.WriteLine(string.Format("Removing previous version of {0} from folder {1}.", packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Folder));
                if (this.packageInstaller.Uninstall(packageInfoOfInstalledVersion) == false)
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            "The removal of the the previous version of {0} (Version: {1}) failed. Please make sure the package has been removed properly before proceeding.",
                            packageInfoOfInstalledVersion.Id,
                            packageInfoOfInstalledVersion.Version));

                    return;
                }

                this.userInterface.WriteLine(
                    string.Format("{0} (Version: {1}) has been successfully removed.", packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Version));
            }

            var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost);
            var packageManager = new PackageManager(this.packageRepository, Directory.GetCurrentDirectory());
            packageManager.PackageInstalling +=
                (sender, args) =>
                this.userInterface.WriteLine(
                    string.Format("Downloading package \"{0}\" (Version: {1}) to folder \"{2}\".", args.Package.Id, args.Package.Version, args.InstallPath));

            packageManager.PackageInstalled += (sender, args) =>
                {
                    string packageFolder = args.InstallPath;
                    string installScriptPath = Path.Combine(packageFolder, InstallPowerShellScriptName);

                    this.userInterface.WriteLine(
                        string.Format(
                            "Package \"{0}\" (Version: {1}) has been downloaded to folder \"{2}\".", args.Package.Id, args.Package.Version, packageFolder));

                    if (File.Exists(installScriptPath) == false)
                    {
                        return;
                    }

                    this.userInterface.WriteLine("Starting the package installation.");
                    powerShellScriptExecutor.ExecuteScript(installScriptPath, new[] { "-DeploymentType Full" });
                };

            this.userInterface.WriteLine(string.Format("Starting installation of package \"{0}\" (Version: {1}).", package.Id, package.Version));
            packageManager.InstallPackage(package, false, true);
            this.userInterface.WriteLine("Installation finished.");
        }
    }
}