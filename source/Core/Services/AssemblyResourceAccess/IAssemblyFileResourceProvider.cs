using System.Reflection;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public interface IAssemblyFileResourceProvider
    {
        Assembly SourceAssembly { get;  }

        AssemblyFileResourceInfo[] GetAllAssemblyResourceInfos(string baseNamespace);
    }
}