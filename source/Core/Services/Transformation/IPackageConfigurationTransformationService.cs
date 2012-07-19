namespace NuDeploy.Core.Services.Transformation
{
    public interface IPackageConfigurationTransformationService
    {
        bool TransformSystemSettings(string packageFolder, string[] systemSettingTransformationProfileNames);
    }
}