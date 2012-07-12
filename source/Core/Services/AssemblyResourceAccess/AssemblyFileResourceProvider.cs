using System.Linq;
using System.Reflection;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class AssemblyFileResourceProvider : IAssemblyFileResourceProvider
    {
        private readonly Assembly sourceAssembly;

        private readonly IAssemblyResourceFilePathProvider assemblyResourceFilePathProvider;

        public AssemblyFileResourceProvider(Assembly sourceAssembly, IAssemblyResourceFilePathProvider assemblyResourceFilePathProvider)
        {
            this.sourceAssembly = sourceAssembly;
            this.assemblyResourceFilePathProvider = assemblyResourceFilePathProvider;
        }

        public Assembly SourceAssembly
        {
            get
            {
                return this.sourceAssembly;
            }
        }

        public AssemblyFileResourceInfo[] GetAllAssemblyResourceInfos()
        {
            string assemblyNamespace = this.sourceAssembly.GetName().FullName;
            string[] resourceNames = this.sourceAssembly.GetManifestResourceNames();

            return
                resourceNames.Select(
                    resourceName =>
                    new AssemblyFileResourceInfo(
                        resourceName, this.assemblyResourceFilePathProvider.GetRelativeFilePath(assemblyNamespace, resourceName))).ToArray();
        }
    }
}