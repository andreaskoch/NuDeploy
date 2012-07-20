using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Filesystem;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public class ConventionBasedBuildResultFilePathProvider : IBuildResultFilePathProvider
    {
        public const string FolderNameDeploymentPackageAdditions = "deploymentpackageadditions";

        public const string FolderNamePublishedApplications = "_PublishedApplications";

        public const string FolderNamePublishedWebsites = "_PublishedWebsites";

        public const string WebsiteFolderFragmentIdentifier = "Website";

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IRelativeFilePathInfoFactory relativeFilePathInfoFactory;

        private readonly string buildFolder;

        public ConventionBasedBuildResultFilePathProvider(IFilesystemAccessor filesystemAccessor, IBuildFolderPathProvider buildFolderPathProvider, IRelativeFilePathInfoFactory relativeFilePathInfoFactory)
        {
            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (buildFolderPathProvider == null)
            {
                throw new ArgumentNullException("buildFolderPathProvider");
            }

            if (relativeFilePathInfoFactory == null)
            {
                throw new ArgumentNullException("relativeFilePathInfoFactory");
            }

            this.filesystemAccessor = filesystemAccessor;
            this.buildFolder = buildFolderPathProvider.GetBuildFolderPath();
            this.relativeFilePathInfoFactory = relativeFilePathInfoFactory;
        }

        public RelativeFilePathInfo[] GetWebsiteFilePaths()
        {
            var publishedWebapplicationsSourceFolderPath = Path.Combine(this.buildFolder, FolderNamePublishedWebsites);
            if (this.filesystemAccessor.DirectoryExists(publishedWebapplicationsSourceFolderPath))
            {
                // get all directories which contain the term website
                var publishedWebsiteDirectories =
                    this.filesystemAccessor.GetSubDirectories(publishedWebapplicationsSourceFolderPath).Where(dir => dir.Name.ToLower().Contains(WebsiteFolderFragmentIdentifier.ToLower()));

                // get all files within these directories
                var publishedWebsiteFiles =
                    publishedWebsiteDirectories.SelectMany(websiteDirectory => this.filesystemAccessor.GetAllFiles(websiteDirectory.FullName));

                return
                    publishedWebsiteFiles.Select(
                        fileInfo => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(fileInfo.FullName, publishedWebapplicationsSourceFolderPath)).
                        ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo[] GetWebApplicationFilePaths()
        {
            var publishedWebapplicationsSourceFolderPath = Path.Combine(this.buildFolder, FolderNamePublishedWebsites);
            if (this.filesystemAccessor.DirectoryExists(publishedWebapplicationsSourceFolderPath))
            {
                var publishedWebapplicationDirectories =
                    this.filesystemAccessor.GetSubDirectories(publishedWebapplicationsSourceFolderPath).Where(dir => !dir.Name.ToLower().Contains(WebsiteFolderFragmentIdentifier.ToLower()));

                var publishedWebapplicationFiles =
                    publishedWebapplicationDirectories.SelectMany(directoryInfo => this.filesystemAccessor.GetAllFiles(directoryInfo.FullName));

                return
                    publishedWebapplicationFiles.Select(
                        fileInfo => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(fileInfo.FullName, publishedWebapplicationsSourceFolderPath)).
                        ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo[] GetApplicationFilePaths()
        {
            var publishedApplicationsSourceFolderPath = Path.Combine(this.buildFolder, FolderNamePublishedApplications);
            if (this.filesystemAccessor.DirectoryExists(publishedApplicationsSourceFolderPath))
            {
                var publishedApplicationFiles = this.filesystemAccessor.GetAllFiles(publishedApplicationsSourceFolderPath);

                return
                    publishedApplicationFiles.Select(
                        fileInfo => this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(fileInfo.FullName, publishedApplicationsSourceFolderPath)).ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo[] GetDeploymentPackageAdditionFilePaths()
        {
            var publishedDeploymentPackageAdditionsSourceFolderPath = Path.Combine(this.buildFolder, FolderNameDeploymentPackageAdditions);
            if (this.filesystemAccessor.DirectoryExists(publishedDeploymentPackageAdditionsSourceFolderPath))
            {
                var publishedDeploymentPackageAdditionFiles = this.filesystemAccessor.GetAllFiles(publishedDeploymentPackageAdditionsSourceFolderPath);

                return
                    publishedDeploymentPackageAdditionFiles.Select(
                        fileInfo =>
                        this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(fileInfo.FullName, publishedDeploymentPackageAdditionsSourceFolderPath)).
                        ToArray();
            }

            return new RelativeFilePathInfo[] { };
        }

        public RelativeFilePathInfo GetNuspecFilePath(string buildConfiguration)
        {
            var nuspecSourceFolderPath = this.buildFolder;

            var nuspecFiles =
                this.filesystemAccessor.GetFiles(nuspecSourceFolderPath).Where(
                    f => f.Extension.Equals(NuDeployConstants.NuSpecFileExtension, StringComparison.OrdinalIgnoreCase)).ToList();

            var buildConfigurationSpecificNuspecFileExtension = string.Format(".{0}{1}", buildConfiguration, NuDeployConstants.NuSpecFileExtension);

            var nuspecFile =
                nuspecFiles.FirstOrDefault(
                    specFile => specFile.Name.EndsWith(buildConfigurationSpecificNuspecFileExtension, StringComparison.OrdinalIgnoreCase))
                ?? nuspecFiles.FirstOrDefault();

            if (nuspecFile == null)
            {
                return null;
            }

            return this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(nuspecFile.FullName, nuspecSourceFolderPath);
        }
    }
}