using System.IO;
using System.Linq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Packaging.Configuration;

using NuGet;

namespace NuDeploy.Core.Services.Packaging.Nuget
{
    public class PackagingService : IPackagingService
    {
        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly string prePackagingFolderPath;

        private readonly string packagingFolderPath;

        public PackagingService(IPrePackagingFolderPathProvider prePackagingFolderPathProvider, IPackagingFolderPathProvider packagingFolderPathProvider, IFilesystemAccessor filesystemAccessor)
        {
            this.filesystemAccessor = filesystemAccessor;
            this.prePackagingFolderPath = prePackagingFolderPathProvider.GetPrePackagingFolderPath();
            this.packagingFolderPath = packagingFolderPathProvider.GetPackagingFolderPath();
        }

        public bool Package()
        {
            // build package
            string packageBasePath = Path.GetFullPath(this.prePackagingFolderPath);
            string nuspecFilePath = Directory.GetFiles(packageBasePath, "*.nuspec", SearchOption.TopDirectoryOnly).First();

            string packageFolder = Path.GetFullPath(this.packagingFolderPath);

            var packageBuilder = new PackageBuilder(nuspecFilePath, packageBasePath);

            string nugetPackageFilePath = Path.Combine(
                packageFolder, new FileInfo(nuspecFilePath).Name.Replace(".nuspec", string.Empty) + "." + packageBuilder.Version + ".nupkg");

            using (Stream stream = this.filesystemAccessor.GetNewFileStream(nugetPackageFilePath))
            {
                packageBuilder.Save(stream);
            }

            return true;
        }
    }
}