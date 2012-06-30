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

        private readonly IPackageRepository packageRepository;

        private readonly PSHost powerShellHost;

        public PackageInstaller(IUserInterface userInterface, IPackageRepository packageRepository, PSHost powerShellHost)
        {
            this.userInterface = userInterface;
            this.packageRepository = packageRepository;
            this.powerShellHost = powerShellHost;
        }

        public bool Install(IPackage package)
        {
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

        public bool Uninstall(NuDeployPackageInfo installedPackage)
        {
            using (var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost))
            {
                string uninstallScriptPath = Path.Combine(installedPackage.Folder, UninstallPowerShellScriptName);
                if (File.Exists(uninstallScriptPath) == false)
                {
                    this.userInterface.WriteLine(
                        string.Format(
                            "Uninstall script \"{0}\" not found for package \"{1} Version({2})\".",
                            UninstallPowerShellScriptName,
                            installedPackage.Id,
                            installedPackage.Version));

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