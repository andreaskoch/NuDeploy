using System;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Services.Packaging.Configuration
{
    public class PrePackagingFolderPathProvider : IPrePackagingFolderPathProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PrePackagingFolderPathProvider(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
        }

        public string GetPrePackagingFolderPath()
        {
            string prePackagingFolder = this.applicationInformation.PrePackagingFolder;

            // create folder if it does not exist
            if (this.filesystemAccessor.DirectoryExists(prePackagingFolder) == false)
            {
                this.filesystemAccessor.CreateDirectory(prePackagingFolder);
                return prePackagingFolder;
            }

            // cleanup existing folder
            if (this.filesystemAccessor.DeleteDirectory(prePackagingFolder))
            {
                this.filesystemAccessor.CreateDirectory(prePackagingFolder);
                return prePackagingFolder;
            }

            return prePackagingFolder;
        }
    }
}