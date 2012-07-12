namespace NuDeploy.Core.Services.Transformation
{
    public interface IConfigurationFileTransformer
    {
        bool Transform(string sourceFilePath, string transformationFilePath, string destinationFilePath);
    }
}
