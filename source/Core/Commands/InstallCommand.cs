using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;

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

        private IPackageManager packageManager;

        public InstallCommand(IUserInterface userInterface, IPackageRepository packageRepository)
        {
            this.userInterface = userInterface;
            this.packageRepository = packageRepository;

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

        protected IPackageManager PackageManager
        {
            get
            {
                if (this.packageManager == null)
                {
                    this.packageManager = new PackageManager(this.packageRepository, Directory.GetCurrentDirectory());

                    this.packageManager.PackageInstalling +=
                        (sender, args) =>
                        this.userInterface.Show(
                            string.Format("Installing package \"{0}\" (Version: {1}) to folder \"{2}\".", args.Package.Id, args.Package.Version, args.InstallPath));

                    this.packageManager.PackageInstalled +=
                        (sender, args) =>
                        this.userInterface.Show(
                            string.Format(
                                "Package \"{0}\" (Version: {1}) has been installed to folder \"{2}\".", args.Package.Id, args.Package.Version, args.InstallPath));
                }

                return this.packageManager;
            }
        }

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

            this.PackageManager.InstallPackage(package, false, true);

            this.userInterface.Show("Installation finished.");
        }
    }
}