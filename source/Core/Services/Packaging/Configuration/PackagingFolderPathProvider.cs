using System;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Services.Packaging.Configuration
{
    public class PackagingFolderPathProvider : IPackagingFolderPathProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PackagingFolderPathProvider(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
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

        public string GetPackagingFolderPath()
        {
            string packagingFolder = this.applicationInformation.PackagingFolder;

            if (!this.filesystemAccessor.DirectoryExists(packagingFolder))
            {
                this.filesystemAccessor.CreateDirectory(packagingFolder);
            }

            return packagingFolder;
        }
    }
}