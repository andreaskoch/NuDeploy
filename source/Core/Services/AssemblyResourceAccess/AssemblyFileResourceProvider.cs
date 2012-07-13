using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class AssemblyFileResourceProvider : IAssemblyFileResourceProvider
    {
        private readonly _Assembly sourceAssembly;

        private readonly IAssemblyResourceFilePathProvider assemblyResourceFilePathProvider;

        public AssemblyFileResourceProvider(_Assembly sourceAssembly, IAssemblyResourceFilePathProvider assemblyResourceFilePathProvider)
        {
            if (sourceAssembly == null)
            {
                throw new ArgumentNullException("sourceAssembly");
            }

            if (assemblyResourceFilePathProvider == null)
            {
                throw new ArgumentNullException("assemblyResourceFilePathProvider");
            }

            this.sourceAssembly = sourceAssembly;
            this.assemblyResourceFilePathProvider = assemblyResourceFilePathProvider;
        }

        public _Assembly SourceAssembly
        {
            get
            {
                return this.sourceAssembly;
            }
        }

        public IEnumerable<AssemblyFileResourceInfo> GetAllAssemblyResourceInfos(string baseNamespace)
        {
            if (string.IsNullOrWhiteSpace(baseNamespace))
            {
                throw new ArgumentException("baseNamespace");
            }

            var resourceNames = this.sourceAssembly.GetManifestResourceNames().Where(r => r.StartsWith(baseNamespace));

            return
                resourceNames.Select(
                    resourceName =>
                    new AssemblyFileResourceInfo(
                        resourceName, this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, resourceName))).ToList();
        }
    }
}