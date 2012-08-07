using System;
using System.IO;

namespace NuDeploy.Core.Services.Transformation
{
    public class PackageConfigurationTransformationService : IPackageConfigurationTransformationService
    {
        public const string SystemSettingsFileName = "systemsettings.xml";

        public const string SystemSettingsFolder = "tools";

        public const string SystemSettingsTransformationFilenameTemplate = "systemsettings.{0}.xml";

        public const string TransformedSystemSettingsFileName = SystemSettingsFileName + ".transformed";

        private readonly IConfigurationFileTransformer configurationFileTransformer;

        public PackageConfigurationTransformationService(IConfigurationFileTransformer configurationFileTransformer)
        {
            if (configurationFileTransformer == null)
            {
                throw new ArgumentNullException("configurationFileTransformer");
            }

            this.configurationFileTransformer = configurationFileTransformer;
        }

        public IServiceResult TransformSystemSettings(string packageFolder, string[] systemSettingTransformationProfileNames)
        {
            if (string.IsNullOrWhiteSpace(packageFolder))
            {
                throw new ArgumentException("packageFolder");
            }

            if (systemSettingTransformationProfileNames == null)
            {
                throw new ArgumentNullException("systemSettingTransformationProfileNames");
            }

            string sourceFileFolder = Path.Combine(packageFolder, SystemSettingsFolder);
            string sourceFilePath = Path.Combine(sourceFileFolder, SystemSettingsFileName);

            foreach (var systemSettingTransformationProfileName in systemSettingTransformationProfileNames)
            {
                string transformationFilename = string.Format(SystemSettingsTransformationFilenameTemplate, systemSettingTransformationProfileName);
                string transformationFilePath = Path.Combine(packageFolder, SystemSettingsFolder, transformationFilename);
                string destinationFilePath = Path.Combine(packageFolder, SystemSettingsFolder, TransformedSystemSettingsFileName);

                IServiceResult transformationResult = this.configurationFileTransformer.Transform(
                    sourceFilePath, transformationFilePath, destinationFilePath);

                if (transformationResult.Status == ServiceResultType.Failure)
                {
                    return new FailureResult(
                        Resources.PackageConfigurationTransformationService.TransformationProfileFailedMessageTemplate, systemSettingTransformationProfileName)
                        {
                            InnerResult = transformationResult
                        };
                }

                // make the transformed file the source for the next transformation
                sourceFilePath = destinationFilePath;
            }

            return new SuccessResult(
                Resources.PackageConfigurationTransformationService.TransformationSucceededMessageTemplate,
                packageFolder,
                string.Join(", ", systemSettingTransformationProfileNames));
        }
    }
}