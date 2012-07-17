using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Packaging.Configuration;

using NuGet;

namespace NuDeploy.Core.Services.Packaging.Nuget
{
    public class PackagingService : IPackagingService
    {
        private const string NuSpecFileExtension = ".nuspec";

        private const string NuGetFileExtension = ".nupkg";

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly string prePackagingFolderPath;

        private readonly string packagingFolderPath;

        public PackagingService(IPrePackagingFolderPathProvider prePackagingFolderPathProvider, IPackagingFolderPathProvider packagingFolderPathProvider, IFilesystemAccessor filesystemAccessor)
        {
            if (prePackagingFolderPathProvider == null)
            {
                throw new ArgumentNullException("prePackagingFolderPathProvider");
            }

            if (packagingFolderPathProvider == null)
            {
                throw new ArgumentNullException("packagingFolderPathProvider");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.filesystemAccessor = filesystemAccessor;
            this.prePackagingFolderPath = prePackagingFolderPathProvider.GetPrePackagingFolderPath();
            this.packagingFolderPath = packagingFolderPathProvider.GetPackagingFolderPath();
        }

        public bool Package()
        {
            // Locate work folders
            string packageBasePath = this.prePackagingFolderPath;
            string packageFolder = this.packagingFolderPath;

            if (!this.filesystemAccessor.DirectoryExists(packageBasePath) || !this.filesystemAccessor.DirectoryExists(packageFolder))
            {
                return false;
            }

            // Locate NuSpec file
            FileInfo nuspecFile =
                this.filesystemAccessor.GetFiles(packageBasePath).FirstOrDefault(
                    f => f.Extension.Equals(NuSpecFileExtension, StringComparison.OrdinalIgnoreCase));
            if (nuspecFile == null)
            {
                return false;
            }

            // Build package
            using (Stream nuspecFileStream = this.filesystemAccessor.GetFileStream(nuspecFile.FullName))
            {
                if (nuspecFileStream == null)
                {
                    return false;
                }

                try
                {
                    var packageBuilder = new PackageBuilder(nuspecFileStream, packageBasePath);

                    string nugetPackageFileName = string.Format("{0}.{1}{2}", packageBuilder.Id, packageBuilder.Version, NuGetFileExtension);
                    string nugetPackageFilePath = Path.Combine(packageFolder, nugetPackageFileName);

                    using (Stream outputStream = this.filesystemAccessor.GetNewFileStream(nugetPackageFilePath))
                    {
                        if (outputStream == null)
                        {
                            return false;
                        }

                        packageBuilder.Save(outputStream);
                    }

                    return true;
                }
                catch (Exception packageBuilderException)
                {
                    return false;
                }
            }
        }
    }
}