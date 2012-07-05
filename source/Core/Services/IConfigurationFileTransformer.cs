namespace NuDeploy.Core.Services
{
    public interface IConfigurationFileTransformer
    {
        bool Transform(string sourceFilePath, string transformationFilePath, string destinationFilePath);
    }
}
