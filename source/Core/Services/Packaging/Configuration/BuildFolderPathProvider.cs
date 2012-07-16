using System;
using System.IO;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Build;

namespace NuDeploy.Core.Services.Packaging.Configuration
{
    public class BuildFolderPathProvider : IBuildFolderPathProvider
    {
        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        public BuildFolderPathProvider(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
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

        public string GetBuildFolderPath()
        {
            string buildFolderPath = Path.GetFullPath(this.applicationInformation.BuildFolder);

            // create folder if it does not exist
            if (this.filesystemAccessor.DirectoryExists(buildFolderPath) == false)
            {
                this.filesystemAccessor.CreateDirectory(buildFolderPath);
                return buildFolderPath;
            }

            // cleanup existing folder
            if (this.filesystemAccessor.DeleteDirectory(buildFolderPath))
            {
                this.filesystemAccessor.CreateDirectory(buildFolderPath);
                return buildFolderPath;
            }

            return buildFolderPath;
        }
    }
}