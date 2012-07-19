using System;
using System.IO;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;

using NuGet;

namespace NuDeploy.Core.Services.Installation
{
    public class NugetPackageExtractor : INugetPackageExtractor
    {
        private readonly IFilesystemAccessor fileSystemAccessor;

        public NugetPackageExtractor(IFilesystemAccessor fileSystemAccessor)
        {
            if (fileSystemAccessor == null)
            {
                throw new ArgumentNullException("fileSystemAccessor");
            }

            this.fileSystemAccessor = fileSystemAccessor;
        }

        public NuDeployPackageInfo Extract(IPackage package, string targetFolder)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            if (string.IsNullOrWhiteSpace(targetFolder))
            {
                throw new ArgumentException("targetFolder");
            }

            string packageFolderName = string.Format("{0}.{1}", package.Id, package.Version);
            string packageFolderPath = Path.Combine(targetFolder, packageFolderName);

            if (this.fileSystemAccessor.DirectoryExists(packageFolderPath))
            {
                if (!this.fileSystemAccessor.DeleteDirectory(packageFolderPath))
                {
                    return null;
                }
            }

            try
            {
                foreach (var file in package.GetFiles())
                {
                    using (Stream reader = file.GetStream())
                    {
                        if (reader == null)
                        {
                            return null;
                        }

                        string targetPath = Path.Combine(packageFolderPath, file.Path);
                        using (Stream writeStream = this.fileSystemAccessor.GetWriteStream(targetPath))
                        {
                            if (writeStream == null)
                            {
                                return null;
                            }

                            reader.CopyTo(writeStream);
                        }
                    }
                }

                return new NuDeployPackageInfo
                    {
                        Id = package.Id, 
                        Version = package.Version, 
                        Folder = packageFolderPath, 
                        IsInstalled = false
                    };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}