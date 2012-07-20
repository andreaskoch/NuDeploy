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
        private const string BackupFileFileExtension = ".old";

        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        private readonly IFilesystemAccessor filesystemAccessor;

        public SelfUpdateService(IUserInterface userInterface, ApplicationInformation applicationInformation, IPackageRepositoryBrowser packageRepositoryBrowser, IFilesystemAccessor filesystemAccessor)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (packageRepositoryBrowser == null)
            {
                throw new ArgumentNullException("packageRepositoryBrowser");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
            this.packageRepositoryBrowser = packageRepositoryBrowser;
            this.filesystemAccessor = filesystemAccessor;
        }

        public bool SelfUpdate(string exePath, Version version)
        {
            if (string.IsNullOrWhiteSpace(exePath))
            {
                throw new ArgumentException("exePath");
            }

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            var currentPackageVersion = new SemanticVersion(version);

            string selfUpdateMessage = string.Format(
                Resources.SelfUpdateService.SelfupdateMessageTemplate,
                NuDeployConstants.NuDeployCommandLinePackageId,
                string.Join(", ", this.packageRepositoryBrowser.RepositoryConfigurations.Select(r => r.Url)));

            this.userInterface.WriteLine(selfUpdateMessage);

            // fetch package
            IPackage updatePackage = this.packageRepositoryBrowser.FindPackage(NuDeployConstants.NuDeployCommandLinePackageId);
            if (updatePackage == null)
            {
                this.userInterface.WriteLine(Resources.SelfUpdateService.PackageNotFound);
                return false;
            }

            // version check
            this.userInterface.WriteLine(string.Format(Resources.SelfUpdateService.CurrentVersionTemplate, this.applicationInformation.NameOfExecutable, version));
            if (currentPackageVersion >= updatePackage.Version)
            {
                this.userInterface.WriteLine(string.Format(Resources.SelfUpdateService.NoUpdateRequiredMessageTemplate, this.applicationInformation.NameOfExecutable));
                return false;
            }

            // update
            this.userInterface.WriteLine(string.Format(Resources.SelfUpdateService.UpdateMessageTemplate, this.applicationInformation.NameOfExecutable, updatePackage.Version));

            IPackageFile executable =
                updatePackage.GetFiles().FirstOrDefault(
                    file =>
                        {
                            var fileName = Path.GetFileName(file.Path);
                            return fileName != null && fileName.Equals(this.applicationInformation.NameOfExecutable, StringComparison.OrdinalIgnoreCase);
                        });

            if (executable == null)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.SelfUpdateService.ExecutableNotFoundInPackageMessageTemplate,
                        NuDeployConstants.NuDeployCommandLinePackageId,
                        this.applicationInformation.NameOfExecutable));

                return false;
            }

            // Get the exe path and move it to a temp file (NuGet.exe.old) so we can replace the running exe with the bits we got from the package repository
            string renamedPath = exePath + BackupFileFileExtension;
            this.filesystemAccessor.MoveFile(exePath, renamedPath);

            // Update the file
            if (!this.UpdateFile(exePath, executable))
            {
                this.userInterface.WriteLine(string.Format(Resources.SelfUpdateService.UpdateFailedMessageTemplate, exePath));
                return false;
            }

            this.userInterface.WriteLine(Resources.SelfUpdateService.UpdateSuccessful);
            return true;
        }

        private bool UpdateFile(string exePath, IPackageFile file)
        {
            using (Stream fromStream = file.GetStream())
            {
                if (fromStream == null)
                {
                    return false;
                }

                using (Stream toStream = this.filesystemAccessor.GetWriteStream(exePath))
                {
                    if (toStream == null)
                    {
                        return false;
                    }

                    fromStream.CopyTo(toStream);

                    return true;
                }
            }
        }
    }
}