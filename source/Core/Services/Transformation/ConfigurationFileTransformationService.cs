using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common.FilesystemAccess;

namespace NuDeploy.Core.Services.Transformation
{
    public class ConfigurationFileTransformationService : IConfigurationFileTransformationService
    {
        public const string PackageContentFolderName = "Content";

        public const string ConfigurationFileExtension = ".config";

        public const string WebConfigurationFileNamePrefix = "web";

        public const string ApplicationConfigurationFileNamePrefix = "app";

        public const string WebConfigurationFileName = WebConfigurationFileNamePrefix + ConfigurationFileExtension;

        public const string ApplicationConfigurationFileName = ApplicationConfigurationFileNamePrefix + ConfigurationFileExtension;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IConfigurationFileTransformer configurationFileTransformer;

        public ConfigurationFileTransformationService(IFilesystemAccessor filesystemAccessor, IConfigurationFileTransformer configurationFileTransformer)
        {
            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (configurationFileTransformer == null)
            {
                throw new ArgumentNullException("configurationFileTransformer");
            }

            this.filesystemAccessor = filesystemAccessor;
            this.configurationFileTransformer = configurationFileTransformer;
        }

        public IServiceResult TransformConfigurationFiles(string baseDirectoryPath, string[] transformationProfileNames)
        {
            if (string.IsNullOrWhiteSpace(baseDirectoryPath))
            {
                throw new ArgumentException("baseDirectoryPath");
            }

            if (transformationProfileNames == null)
            {
                throw new ArgumentNullException("transformationProfileNames");
            }

            if (transformationProfileNames.Length == 0)
            {
                return new SuccessResult(Resources.ConfigurationFileTransformationService.NoTransformationProfilesSupplied);
            }
            
            // get all app and web.configs in the content folder
            string contentFolder = Path.Combine(baseDirectoryPath, PackageContentFolderName);
            var configurationFiles =
                this.filesystemAccessor.GetAllFiles(contentFolder).Where(
                    file =>
                    file.Name.Equals(WebConfigurationFileName, StringComparison.OrdinalIgnoreCase)
                    || file.Name.Equals(ApplicationConfigurationFileName, StringComparison.OrdinalIgnoreCase));

            // transform config files
            foreach (var configurationFile in configurationFiles)
            {
                string sourceFileFolder = configurationFile.Directory.FullName;
                string sourceFileName = configurationFile.Name;
                string sourceFilePath = configurationFile.FullName;

                foreach (var systemSettingTransformationProfileName in transformationProfileNames)
                {
                    // assemble file names
                    string transformationFilename = sourceFileName.Replace(
                        ConfigurationFileExtension, string.Format(".{0}{1}", systemSettingTransformationProfileName, ConfigurationFileExtension));

                    string transformationFilePath = Path.Combine(sourceFileFolder, transformationFilename);

                    if (this.filesystemAccessor.FileExists(transformationFilePath))
                    {
                        // transform
                        IServiceResult transformationResult = this.configurationFileTransformer.Transform(sourceFilePath, transformationFilePath, destinationFilePath: sourceFilePath);
                        if (transformationResult.Status == ServiceResultType.Failure)
                        {
                            return new FailureResult(
                                Resources.ConfigurationFileTransformationService.TransformationFailedForProfileMessageTemplate,
                                systemSettingTransformationProfileName)
                            {
                                InnerResult = transformationResult
                            };
                        }                        
                    }
                }

                // cleanup
                var transformationFiles =
                    this.filesystemAccessor.GetFiles(sourceFileFolder).Where(
                        file => file.Name.Equals(sourceFileName, StringComparison.OrdinalIgnoreCase) == false);

                foreach (var transformationFile in transformationFiles)
                {
                    var filePath = transformationFile.FullName;
                    if (!this.filesystemAccessor.DeleteFile(filePath))
                    {
                        return new FailureResult(Resources.ConfigurationFileTransformationService.CleanupFailedForFileMessageTemplate, filePath);
                    }
                }
            }

            return new SuccessResult(
                Resources.ConfigurationFileTransformationService.SuccessMessageTemplate, baseDirectoryPath, string.Join(", ", transformationProfileNames));
        }
    }
}