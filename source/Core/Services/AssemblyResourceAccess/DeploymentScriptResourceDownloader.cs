using System.IO;
using System.Linq;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class DeploymentScriptResourceDownloader : IDeploymentScriptResourceDownloader
    {
        private const string DeploymentScriptNamespace = "NuDeploy.Core.Resources.DeploymentScripts.";

        private readonly IAssemblyFileResourceProvider assemblyFileResourceProvider;

        public DeploymentScriptResourceDownloader(IAssemblyFileResourceProvider assemblyFileResourceProvider)
        {
            this.assemblyFileResourceProvider = assemblyFileResourceProvider;
        }

        public void Download(string targetFolder)
        {
            var assemblyResourceInfos =
                this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos().Where(r => r.ResourceName.StartsWith(DeploymentScriptNamespace));

            foreach (var assemblyFileResourceInfo in assemblyResourceInfos)
            {
                using (Stream resourceStream = this.assemblyFileResourceProvider.SourceAssembly.GetManifestResourceStream(assemblyFileResourceInfo.ResourceName))
                {
                    string targetPath = Path.Combine(targetFolder, assemblyFileResourceInfo.ResourcePath);
                    using (Stream fileStream = File.OpenWrite(targetPath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }                
            }
        }
    }
}