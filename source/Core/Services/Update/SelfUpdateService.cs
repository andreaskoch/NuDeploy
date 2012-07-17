using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Repositories;

using NuGet;

namespace NuDeploy.Core.Services.Update
{
    public class SelfUpdateService : ISelfUpdateService
    {
        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IFilesystemAccessor filesystemAccessor;

        public SelfUpdateService(IUserInterface userInterface, ApplicationInformation applicationInformation, IPackageRepositoryBrowser packageRepositoryBrowser, IFilesystemAccessor filesystemAccessor)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.filesystemAccessor = filesystemAccessor;
        }

        public void SelfUpdate(string exePath, Version version)
        {
            var nugetVersion = new SemanticVersion(version);

            string selfUpdateMessage = string.Format(
                Resources.SelfUpdateCommand.SelfupdateMessageTemplate,
                NuDeployConstants.NuDeployCommandLinePackageId,
                string.Join(", ", this.packageRepositoryBrowser.RepositoryConfigurations.Select(r => r.Url)));

            this.userInterface.WriteLine(selfUpdateMessage);

            // fetch package
            IPackageRepository packageRepository;
            IPackage package = this.packageRepositoryBrowser.FindPackage(NuDeployConstants.NuDeployCommandLinePackageId, out packageRepository);
            if (package == null)
            {
                this.userInterface.WriteLine(Resources.SelfUpdateCommand.PackageNotFound);
                return;
            }

            // version check
            this.userInterface.WriteLine(string.Format(Resources.SelfUpdateCommand.CurrentVersionTemplate, this.applicationInformation.NameOfExecutable, version));
            if (nugetVersion >= package.Version)
            {
                this.userInterface.WriteLine(string.Format(Resources.SelfUpdateCommand.NoUpdateRequiredMessageTemplate, this.applicationInformation.NameOfExecutable));
                return;
            }

            // update
            this.userInterface.WriteLine(string.Format(Resources.SelfUpdateCommand.UpdateMessageTemplate, this.applicationInformation.NameOfExecutable, package.Version));

            IPackageFile executable =
                package.GetFiles().FirstOrDefault(
                    file =>
                        {
                            var fileName = Path.GetFileName(file.Path);
                            return fileName != null && fileName.Equals(this.applicationInformation.NameOfExecutable, StringComparison.OrdinalIgnoreCase);
                        });

            if (executable == null)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.SelfUpdateCommand.ExecutableNotFoundInPackageMessageTemplate,
                        NuDeployConstants.NuDeployCommandLinePackageId,
                        this.applicationInformation.NameOfExecutable));
            }

            // Get the exe path and move it to a temp file (NuGet.exe.old) so we can replace the running exe with the bits we got from the package repository
            string renamedPath = exePath + ".old";
            this.filesystemAccessor.MoveFile(exePath, renamedPath);

            // Update the file
            this.UpdateFile(exePath, executable);

            this.userInterface.WriteLine(Resources.SelfUpdateCommand.UpdateSuccessful);
        }

        private void UpdateFile(string exePath, IPackageFile file)
        {
            using (Stream fromStream = file.GetStream(), toStream = this.filesystemAccessor.GetWriteStream(exePath))
            {
                fromStream.CopyTo(toStream);
            }
        }
    }
}