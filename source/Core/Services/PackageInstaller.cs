using System.IO;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

using NuGet;

namespace NuDeploy.Core.Services
{
    public class PackageInstaller : IPackageInstaller
    {
        private const string InstallPowerShellScriptName = "Deploy.ps1";

        private const string UninstallPowerShellScriptName = "Remove.ps1";

        private readonly IUserInterface userInterface;

        private readonly IInstallationStatusProvider installationStatusProvider;

        private readonly IPackageRepository packageRepository;

        private readonly PSHost powerShellHost;

        public PackageInstaller(IUserInterface userInterface, IInstallationStatusProvider installationStatusProvider, IPackageRepository packageRepository, PSHost powerShellHost)
        {
            this.userInterface = userInterface;
            this.installationStatusProvider = installationStatusProvider;
            this.packageRepository = packageRepository;
            this.powerShellHost = powerShellHost;
        }

        public bool Install(string packageId, bool forceInstallation)
        {
            // fetch package
            IPackage package = this.packageRepository.FindPackage(packageId);
            if (package == null)
            {
                this.userInterface.WriteLine(string.Format("Package \"{0}\" was not found at \"{1}\".", packageId, this.packageRepository.Source));
                return false;
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

                        return false;
                    }

                    if (package.Version < packageInfoOfInstalledVersion.Version)
                    {
                        this.userInterface.WriteLine(
                            string.Format(
                                "You already have a more recent version installed: {0} (Version: {1}). Use the -force option if you still want to install this older version.",
                                packageInfoOfInstalledVersion.Id,
                                packageInfoOfInstalledVersion.Version));

                        return false;
                    }
                }

                /* installed version is older and must be removed */
                this.userInterface.WriteLine(string.Format("Removing previous version of {0} from folder {1}.", packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Folder));
                bool uninstallResult = this.Uninstall(packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Version);
                if (uninstallResult)
                {
                    this.userInterface.WriteLine(
                        string.Format("{0} (Version: {1}) has been successfully removed.", packageInfoOfInstalledVersion.Id, packageInfoOfInstalledVersion.Version));                    
                }
                else
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            "The removal of the the previous version of {0} (Version: {1}) failed.",
                            packageInfoOfInstalledVersion.Id,
                            packageInfoOfInstalledVersion.Version));

                    if (forceInstallation == false)
                    {
                        this.userInterface.WriteLine("Please make sure the package has been removed properly before installing a new version or use the -force option if you still want to install the new version.");
                        return false;
                    }
                }
            }

            using (var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost))
            {
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

                return true;
            }
        }

        public bool Uninstall(string packageId, SemanticVersion version = null)
        {
            // check if package is installed
            if (!this.installationStatusProvider.IsInstalled(packageId))
            {
                this.userInterface.WriteLine(string.Format("Package \"{0}\" is not installed in the current folder.", packageId));
                return false;
            }

            // remove the package
            NuDeployPackageInfo installedPackage = this.installationStatusProvider.GetPackageInfo(packageId);

            using (var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost))
            {
                string uninstallScriptPath = Path.Combine(installedPackage.Folder, UninstallPowerShellScriptName);
                if (File.Exists(uninstallScriptPath) == false)
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            "Uninstall script \"{0}\" not found for package \"{1} Version({2})\" in folder \"{3}\".",
                            UninstallPowerShellScriptName,
                            installedPackage.Id,
                            installedPackage.Version,
                            installedPackage.Folder));

                    return false;
                }

                // uninstall
                this.userInterface.WriteLine(string.Format("Uninstalling package \"{0} Version({1})\"", installedPackage.Id, installedPackage.Version));
                powerShellScriptExecutor.ExecuteScript(uninstallScriptPath);

                // remove package files
                this.userInterface.WriteLine(string.Format("Deleting package folder \"{0}\"", installedPackage.Folder));
                Directory.Delete(installedPackage.Folder, true);

                return true;
            }
        }
    }
}