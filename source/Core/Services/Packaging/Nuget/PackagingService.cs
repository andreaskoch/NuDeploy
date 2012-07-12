using System.IO;
using System.Linq;

using NuDeploy.Core.Services.Configuration;

using NuGet;

namespace NuDeploy.Core.Services.Packaging.Nuget
{
    public class PackagingService : IPackagingService
    {
        private readonly string prePackagingFolderPath;

        private readonly string packagingFolderPath;

        public PackagingService(IPrePackagingFolderPathProvider prePackagingFolderPathProvider, IPackagingFolderPathProvider packagingFolderPathProvider)
        {
            this.prePackagingFolderPath = prePackagingFolderPathProvider.GetPrePackagingFolderPath();
            this.packagingFolderPath = packagingFolderPathProvider.GetPackagingFolderPath();
        }

        public bool Package()
        {
            // build package
            string packageBasePath = Path.GetFullPath(this.prePackagingFolderPath);
            string packageFolder = Path.GetFullPath(this.packagingFolderPath);

            string nuspecFilePath = Directory.GetFiles(packageFolder, "*.nuspec", SearchOption.TopDirectoryOnly).First();
            string nugetPackageFilePath = Path.Combine(packageFolder, new FileInfo(nuspecFilePath).Name.Replace(".nuspec", string.Empty) + ".1.0.0.nupkg");

            var packageBuilder = new PackageBuilder(nuspecFilePath, packageBasePath);
            using (Stream stream = File.Create(nugetPackageFilePath))
            {
                packageBuilder.Save(stream);
            }

            return true;
        }
    }
}