namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public interface IAssemblyResourceFilePathProvider
    {
        string GetRelativeFilePath(string baseNamespace, string assemblyResourceName);
    }
}