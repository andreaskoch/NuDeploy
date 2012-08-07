namespace NuDeploy.Core.Services.Transformation
{
    public interface IConfigurationFileTransformer
    {
        IServiceResult Transform(string sourceFilePath, string transformationFilePath, string destinationFilePath);
    }
}
