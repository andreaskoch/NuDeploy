using System.IO;
using System.Linq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Configuration;
using NuDeploy.Core.Services.Filesystem;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public class ConventionBasedBuildResultFilePathProvider : IBuildResultFilePathProvider
    {
        private const string FolderNameDeploymentPackageAdditions = "deploymentpackageadditions";

        private const string FolderNamePublishedApplications = "_PublishedApplications";

        private const string FolderNamePublishedWebsites = "_PublishedWebsites";

        private const string NuspecFilenamePattern = "*.{0}.nuspec";

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IRelativeFilePathInfoFactory relativeFilePathInfoFactory;

        private readonly string buildFolder;

        public ConventionBasedBuildResultFilePathProvider(IFilesystemAccessor filesystemAccessor, IBuildFolderPathProvider buildFolderPathProvider, IRelativeFilePathInfoFactory relativeFilePathInfoFactory)
        {
            this.filesystemAccessor = filesystemAccessor;
            this.buildFolder = buildFolderPathProvider.GetBuildFolderPath();
            this.relativeFilePathInfoFactory = relativeFilePathInfoFactory;
        }

        public RelativeFilePathInfo[] GetWesbiteFilePaths()
        {
            var publishedWebapplicationsSourceFolderPath = Path.GetFullPath(Path.Combine(this.buildFolder, FolderNamePublishedWebsites));
            if (this.filesystemAccessor.DirectoryExists(publishedWebapplicationsSourceFolderPath))
            {
                var publishedWebsiteDirectories =
                    Directory.GetFiles(publishedWebapplicationsSourceFolderPath, "*", SearchOption.TopDirectoryOnly).Where(
                        filePath => filePath.Contains(".Website"));

                var publishedWebsiteFiles =
                    publishedWebsiteDirectories.SelectMany(websiteDirectory => Directory.GetFiles(websiteDirectory, "*", SearchOption.AllDirectories));

                return publishedWebsiteFiles.Select(f => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(f, publishedWebapplicationsSourceFolderPath)).ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo[] GetWebApplicationFilePaths()
        {
            var publishedWebapplicationsSourceFolderPath = Path.GetFullPath(Path.Combine(this.buildFolder, FolderNamePublishedWebsites));
            if (this.filesystemAccessor.DirectoryExists(publishedWebapplicationsSourceFolderPath))
            {
                var publishedWebapplicationDirectories =
                    Directory.GetFiles(publishedWebapplicationsSourceFolderPath, "*", SearchOption.TopDirectoryOnly).Where(
                        filePath => filePath.Contains(".Website") == false);

                var publishedWebapplicationFiles =
                    publishedWebapplicationDirectories.SelectMany(websiteDirectory => Directory.GetFiles(websiteDirectory, "*", SearchOption.AllDirectories));

                return publishedWebapplicationFiles.Select(f => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(f, publishedWebapplicationsSourceFolderPath)).ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo[] GetApplicationFilePaths()
        {
            var publishedApplicationsSourceFolderPath = Path.Combine(this.buildFolder, FolderNamePublishedApplications);
            if (this.filesystemAccessor.DirectoryExists(publishedApplicationsSourceFolderPath))
            {
                var publishedApplicationFiles = Directory.GetFiles(publishedApplicationsSourceFolderPath, "*", SearchOption.AllDirectories);

                return publishedApplicationFiles.Select(f => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(f, publishedApplicationsSourceFolderPath)).ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo[] GetDeploymentPackageAdditionFilePaths()
        {
            var publishedDeploymentPackageAdditionsSourceFolderPath = Path.Combine(this.buildFolder, FolderNameDeploymentPackageAdditions);
            if (this.filesystemAccessor.DirectoryExists(publishedDeploymentPackageAdditionsSourceFolderPath))
            {
                var publishedDeploymentPackageAdditionFiles = Directory.GetFiles(publishedDeploymentPackageAdditionsSourceFolderPath, "*", SearchOption.AllDirectories);

                return publishedDeploymentPackageAdditionFiles.Select(f => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(f, publishedDeploymentPackageAdditionsSourceFolderPath)).ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo GetNuspecFilePath(string buildConfiguration)
        {
            var nuspecSourceFolderPath = this.buildFolder;
            var nuspecFileSearchPattern = string.Format(NuspecFilenamePattern, buildConfiguration);
            var nuspecFilePath = Directory.GetFiles(nuspecSourceFolderPath, nuspecFileSearchPattern, SearchOption.TopDirectoryOnly).First();

            return this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(nuspecFilePath, nuspecSourceFolderPath);
        }
    }
}