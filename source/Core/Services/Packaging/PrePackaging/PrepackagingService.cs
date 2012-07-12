using System.IO;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.Core.Services.Configuration;
using NuDeploy.Core.Services.Packaging.Build;

namespace NuDeploy.Core.Services.Packaging.PrePackaging
{
    public class PrepackagingService : IPrepackagingService
    {
        private const string TargetFolderNameWebsites = "websites";

        private const string TargetFolderNameWebApplications = "webapplications";

        private const string TargetFolderNameApplications = "applications";

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IDeploymentScriptResourceDownloader deploymentScriptResourceDownloader;

        private readonly IBuildResultFilePathProvider buildResultFilePathProvider;

        private readonly string prePackagingFolderPath;

        public PrepackagingService(IFilesystemAccessor filesystemAccessor, IDeploymentScriptResourceDownloader deploymentScriptResourceDownloader, IBuildResultFilePathProvider buildResultFilePathProvider, IPrePackagingFolderPathProvider prePackagingFolderPathProvider)
        {
            this.filesystemAccessor = filesystemAccessor;
            this.deploymentScriptResourceDownloader = deploymentScriptResourceDownloader;
            this.buildResultFilePathProvider = buildResultFilePathProvider;
            this.prePackagingFolderPath = prePackagingFolderPathProvider.GetPrePackagingFolderPath();
        }

        public bool Prepackage(string buildConfiguration)
        {
            try
            {
                this.CopyFilesToPrePackagingFolder(buildConfiguration);
                return true;
            }
            catch
            {
            }

            return false;
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
            this.deploymentScriptResourceDownloader.Download(this.prePackagingFolderPath);

            // web sites
            var websiteSourceFiles = this.buildResultFilePathProvider.GetWesbiteFilePaths();
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
            var applicationSourceFiles = this.buildResultFilePathProvider.GetWebApplicationFilePaths();
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