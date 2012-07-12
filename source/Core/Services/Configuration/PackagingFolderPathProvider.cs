using System.IO;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Services.Configuration
{
    public class PackagingFolderPathProvider : IPackagingFolderPathProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PackagingFolderPathProvider(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
        {
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
        }

        public string GetPackagingFolderPath()
        {
            string packagingFolder = Path.GetFullPath(this.applicationInformation.PackagingFolder);

            if (!this.filesystemAccessor.DirectoryExists(packagingFolder))
            {
                this.filesystemAccessor.CreateDirectory(packagingFolder);
            }

            return packagingFolder;
        }
    }
}