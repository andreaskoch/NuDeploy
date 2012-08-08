namespace NuDeploy.Core.Services.Transformation
{
    public interface IConfigurationFileTransformationService
    {
        IServiceResult TransformConfigurationFiles(string baseDirectoryPath, string[] transformationProfileNames);
    }
}