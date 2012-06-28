using System.IO;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

namespace NuDeploy.Core.Services
{
    public class PackageInstaller : IPackageInstaller
    {
        private const string UninstallPowerShellScriptName = "Remove.ps1";

        private readonly IUserInterface userInterface;

        private readonly PSHost powerShellHost;

        public PackageInstaller(IUserInterface userInterface, PSHost powerShellHost)
        {
            this.userInterface = userInterface;
            this.powerShellHost = powerShellHost;
        }

        public bool Uninstall(NuDeployPackageInfo installedPackage)
        {
            var powerShellScriptExecutor = new PowerShellScriptExecutor(this.powerShellHost);

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