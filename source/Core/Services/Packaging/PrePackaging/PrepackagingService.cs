using System;
using System.IO;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Configuration;

namespace NuDeploy.Core.Services.Packaging.PrePackaging
{
    public class PrepackagingService : IPrepackagingService
    {
        public const string TargetFolderNameWebsites = "content\\websites";

        public const string TargetFolderNameWebApplications = "content\\webapplications";

        public const string TargetFolderNameApplications = "content\\applications";

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IAssemblyResourceDownloader assemblyResourceDownloader;

        private readonly IBuildResultFilePathProvider buildResultFilePathProvider;

        private readonly string prePackagingFolderPath;

        public PrepackagingService(IFilesystemAccessor filesystemAccessor, IAssemblyResourceDownloader assemblyResourceDownloader, IBuildResultFilePathProvider buildResultFilePathProvider, IPrePackagingFolderPathProvider prePackagingFolderPathProvider)
        {
            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (assemblyResourceDownloader == null)
            {
                throw new ArgumentNullException("assemblyResourceDownloader");
            }

            if (buildResultFilePathProvider == null)
            {
                throw new ArgumentNullException("buildResultFilePathProvider");
            }

            if (prePackagingFolderPathProvider == null)
            {
                throw new ArgumentNullException("prePackagingFolderPathProvider");
            }

            this.filesystemAccessor = filesystemAccessor;
            this.assemblyResourceDownloader = assemblyResourceDownloader;
            this.buildResultFilePathProvider = buildResultFilePathProvider;
            this.prePackagingFolderPath = prePackagingFolderPathProvider.GetPrePackagingFolderPath();
        }

        public IServiceResult Prepackage(string buildFolder)
        {
            if (!this.filesystemAccessor.DirectoryExists(this.prePackagingFolderPath))
            {
                return new FailureResult(Resources.PrepackagingService.ErrorPrepackagingFolderDoesNotExistMessageTemplate, this.prePackagingFolderPath);
            }

            if (!this.filesystemAccessor.DirectoryExists(buildFolder))
            {
                return new FailureResult(Resources.PrepackagingService.ErrorBuildFolderDoesNotexistMessageTemplate, buildFolder);
            }

            try
            {
                this.CopyFilesToPrePackagingFolder(buildFolder);
                return new SuccessResult(Resources.PrepackagingService.SuccessMessageTemplate, this.prePackagingFolderPath);
            }
            catch (Exception prepackagingException)
            {
                return new FailureResult(Resources.PrepackagingService.FailureMessageTemplate, prepackagingException.Message);
            }
        }

        private void CopyFilesToPrePackagingFolder(string buildFolder)
        {
            // deployment package additions
            var deploymentPackageAdditionSourceFiles = this.buildResultFilePathProvider.GetDeploymentPackageAdditionFilePaths(buildFolder);
            foreach (var sourceFile in deploymentPackageAdditionSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // deployment scripts
            this.assemblyResourceDownloader.Download(this.prePackagingFolderPath);

            // web sites
            var websiteSourceFiles = this.buildResultFilePathProvider.GetWebsiteFilePaths(buildFolder);
            foreach (var sourceFile in websiteSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath, TargetFolderNameWebsites);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // web applications
            var webApplicationSourceFiles = this.buildResultFilePathProvider.GetWebApplicationFilePaths(buildFolder);
            foreach (var sourceFile in webApplicationSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath, TargetFolderNameWebApplications);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // applications
            var applicationSourceFiles = this.buildResultFilePathProvider.GetApplicationFilePaths(buildFolder);
            foreach (var sourceFile in applicationSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath, TargetFolderNameApplications);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }
        }

        private string GetTargetPath(string relativeFilePath, params string[] targetFolders)
        {
            string baseFolder = targetFolders != null && targetFolders.Length > 0 ? Path.Combine(targetFolders) : string.Empty;
            string targetPath = Path.Combine(this.prePackagingFolderPath, baseFolder, relativeFilePath);
            return targetPath;
        }
    }
}