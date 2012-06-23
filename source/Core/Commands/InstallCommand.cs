using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

using NuGet;

namespace NuDeploy.Core.Commands
{
    public class InstallCommand : ICommand
    {
        private const string CommandName = "install";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new string[] { };

        private readonly IUserInterface userInterface;

        private readonly IPackageRepository packageRepository;

        private readonly IPowerShellScriptExecutor powerShellScriptExecutor;

        public InstallCommand(IUserInterface userInterface, IPackageRepository packageRepository, IPowerShellScriptExecutor powerShellScriptExecutor)
        {
            this.userInterface = userInterface;
            this.packageRepository = packageRepository;
            this.powerShellScriptExecutor = powerShellScriptExecutor;

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
                this.userInterface.Show("No package id specified.");
                return;
            }

            // fetch package
            IPackage package = this.packageRepository.FindPackage(packageId);
            if (package == null)
            {
                this.userInterface.Show(string.Format("Package \"{0}\"was not found at \"{1}\".", packageId, this.packageRepository.Source));
                return;
            }

            this.userInterface.Show(string.Format("Starting installation of package \"{0}\".", package.Id));

            var packageManager = new PackageManager(this.packageRepository, Directory.GetCurrentDirectory());

            packageManager.PackageInstalling +=
                (sender, args) =>
                this.userInterface.Show(
                    string.Format("Downloading package \"{0}\" (Version: {1}) to folder \"{2}\".", args.Package.Id, args.Package.Version, args.InstallPath));

            packageManager.PackageInstalled += (sender, args) =>
                {
                    string packageFolder = args.InstallPath;
                    string installScriptPath = Path.Combine(packageFolder, "Deploy.ps1");

                    this.userInterface.Show(
                        string.Format(
                            "Package \"{0}\" (Version: {1}) has been downloaded to folder \"{2}\".", args.Package.Id, args.Package.Version, packageFolder));

                    if (File.Exists(installScriptPath) == false)
                    {
                        return;
                    }

                    this.userInterface.Show("Starting the package installation.");

                    string installPowerShellCommand = string.Format("{0} -DeploymentType {1}", installScriptPath, "Full");
                    this.powerShellScriptExecutor.ExecuteCommand(installPowerShellCommand);
                };

            packageManager.InstallPackage(package, false, true);

            this.userInterface.Show("Installation finished.");
        }
    }
}