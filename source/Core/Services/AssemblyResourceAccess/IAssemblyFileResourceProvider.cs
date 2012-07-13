using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public interface IAssemblyFileResourceProvider
    {
        _Assembly SourceAssembly { get; }

        IEnumerable<AssemblyFileResourceInfo> GetAllAssemblyResourceInfos(string baseNamespace);
    }
}