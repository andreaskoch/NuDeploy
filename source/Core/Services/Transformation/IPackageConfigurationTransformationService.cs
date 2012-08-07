namespace NuDeploy.Core.Services.Transformation
{
    public interface IPackageConfigurationTransformationService
    {
        IServiceResult TransformSystemSettings(string packageFolder, string[] systemSettingTransformationProfileNames);
    }
}