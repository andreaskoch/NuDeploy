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

        public bool Prepackage(string buildConfiguration)
        {
            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                throw new ArgumentException("buildConfiguration");
            }

            if (!this.filesystemAccessor.DirectoryExists(this.prePackagingFolderPath))
            {
                return false;
            }

            try
            {
                this.CopyFilesToPrePackagingFolder(buildConfiguration);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void CopyFilesToPrePackagingFolder(string buildConfiguration)
        {
            // nuspec
            var nuspecFile = this.buildResultFilePathProvider.GetNuspecFilePath(buildConfiguration);
            if (nuspecFile != null)
            {
                string sourcePath = nuspecFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(nuspecFile.RelativeFilePath);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // deployment package additions
            var deploymentPackageAdditionSourceFiles = this.buildResultFilePathProvider.GetDeploymentPackageAdditionFilePaths();
            foreach (var sourceFile in deploymentPackageAdditionSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // deployment scripts
            this.assemblyResourceDownloader.Download(this.prePackagingFolderPath);

            // web sites
            var websiteSourceFiles = this.buildResultFilePathProvider.GetWebsiteFilePaths();
            foreach (var sourceFile in websiteSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath, TargetFolderNameWebsites);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // web applications
            var webApplicationSourceFiles = this.buildResultFilePathProvider.GetWebApplicationFilePaths();
            foreach (var sourceFile in webApplicationSourceFiles)
            {
                string sourcePath = sourceFile.AbsoluteFilePath;
                string targetPath = this.GetTargetPath(sourceFile.RelativeFilePath, TargetFolderNameWebApplications);
                this.filesystemAccessor.CopyFile(sourcePath, targetPath);
            }

            // applications
            var applicationSourceFiles = this.buildResultFilePathProvider.GetApplicationFilePaths();
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