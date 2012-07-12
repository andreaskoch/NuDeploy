using System.IO;
using System.Linq;

using NuDeploy.Core.Common.FilesystemAccess;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class DeploymentScriptResourceDownloader : IDeploymentScriptResourceDownloader
    {
        private const string DeploymentScriptNamespace = "NuDeploy.Core.Resources.DeploymentScripts.";

        private readonly IAssemblyFileResourceProvider assemblyFileResourceProvider;

        private readonly IFilesystemAccessor filesystemAccessor;

        public DeploymentScriptResourceDownloader(IAssemblyFileResourceProvider assemblyFileResourceProvider, IFilesystemAccessor filesystemAccessor)
        {
            this.assemblyFileResourceProvider = assemblyFileResourceProvider;
            this.filesystemAccessor = filesystemAccessor;
        }

        public void Download(string targetFolder)
        {
            var assemblyResourceInfos =
                this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos(DeploymentScriptNamespace).Where(r => r.ResourceName.StartsWith(DeploymentScriptNamespace));

            foreach (var assemblyFileResourceInfo in assemblyResourceInfos)
            {
                using (Stream resourceStream = this.assemblyFileResourceProvider.SourceAssembly.GetManifestResourceStream(assemblyFileResourceInfo.ResourceName))
                {
                    string targetPath = Path.Combine(targetFolder, assemblyFileResourceInfo.ResourcePath);
                    using (Stream fileStream = this.filesystemAccessor.GetNewFileStream(targetPath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }                
            }
        }
    }
}