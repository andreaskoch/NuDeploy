using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
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

        public IServiceResult Package()
        {
            // Locate work folders
            string packageBasePath = this.prePackagingFolderPath;
            string packageFolder = this.packagingFolderPath;

            if (!this.filesystemAccessor.DirectoryExists(packageBasePath))
            {
                return new FailureResult(Resources.PackagingService.ErrorBaseFolderDoesNotExistMessageTemplate, packageBasePath);
            }

            if (!this.filesystemAccessor.DirectoryExists(packageFolder))
            {
                return new FailureResult(Resources.PackagingService.ErrorPackagingFolderDoesNotExistMessageTemplate, packageFolder);
            }

            // Locate NuSpec file
            FileInfo nuspecFile =
                this.filesystemAccessor.GetFiles(packageBasePath).FirstOrDefault(
                    f => f.Extension.Equals(NuDeployConstants.NuSpecFileExtension, StringComparison.OrdinalIgnoreCase));

            if (nuspecFile == null)
            {
                return
                    new FailureResult(
                        Resources.PackagingService.ErrorNuspecFileNotFoundMessageTemplate,
                        NuDeployConstants.NuSpecFileExtension,
                        packageBasePath);
            }

            // Build package
            try
            {
                string nuspecFilePath = nuspecFile.FullName;
                Stream nuspecFileStream = this.filesystemAccessor.GetReadStream(nuspecFilePath);
                if (nuspecFileStream == null)
                {
                    return new FailureResult(Resources.PackagingService.ErrorNuspecFileCannotBeReadMessageTemplate, nuspecFilePath);
                }

                var packageBuilder = new PackageBuilder(nuspecFileStream, packageBasePath);
                nuspecFileStream.Close();

                string nugetPackageFileName = string.Format("{0}.{1}{2}", packageBuilder.Id, packageBuilder.Version, NuDeployConstants.NuGetFileExtension);
                string nugetPackageFilePath = Path.Combine(packageFolder, nugetPackageFileName);

                using (Stream outputStream = this.filesystemAccessor.GetWriteStream(nugetPackageFilePath))
                {
                    if (outputStream == null)
                    {
                        return new FailureResult(Resources.PackagingService.ErrorOutputStreamCouldNotBeOpenedMessageTemplate, nugetPackageFilePath);
                    }

                    packageBuilder.Save(outputStream);
                }

                return new SuccessResult(
                    Resources.PackagingService.SuccessMessageTemplate, packageBasePath, nuspecFilePath)
                    {
                        ResultArtefact = nugetPackageFilePath
                    };
            }
            catch (Exception packagingException)
            {
                return new FailureResult(Resources.PackagingService.FailedMessageTemplate, packagingException.Message);
            }
        }
    }
}