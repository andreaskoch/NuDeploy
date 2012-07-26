namespace NuDeploy.Core.Services.Transformation
{
    public interface IConfigurationFileTransformationService
    {
        bool TransformConfigurationFiles(string[] systemSettingTransformationProfileNames);
    }
}