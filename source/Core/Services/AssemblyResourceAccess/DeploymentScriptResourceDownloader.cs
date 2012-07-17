using System;
using System.IO;

using NuDeploy.Core.Common.FilesystemAccess;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class DeploymentScriptResourceDownloader : IAssemblyResourceDownloader
    {
        public const string DeploymentScriptNamespace = "NuDeploy.Core.Resources.DeploymentScripts";

        private readonly IAssemblyFileResourceProvider assemblyFileResourceProvider;

        private readonly IFilesystemAccessor filesystemAccessor;

        public DeploymentScriptResourceDownloader(IAssemblyFileResourceProvider assemblyFileResourceProvider, IFilesystemAccessor filesystemAccessor)
        {
            if (assemblyFileResourceProvider == null)
            {
                throw new ArgumentNullException("assemblyFileResourceProvider");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.assemblyFileResourceProvider = assemblyFileResourceProvider;
            this.filesystemAccessor = filesystemAccessor;
        }

        public void Download(string targetFolder)
        {
            if (targetFolder == null)
            {
                throw new ArgumentNullException("targetFolder");
            }

            var assemblyResourceInfos = this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos(DeploymentScriptNamespace);

            foreach (var assemblyFileResourceInfo in assemblyResourceInfos)
            {
                using (Stream resourceStream = this.assemblyFileResourceProvider.SourceAssembly.GetManifestResourceStream(assemblyFileResourceInfo.ResourceName))
                {
                    string targetPath = Path.Combine(targetFolder, assemblyFileResourceInfo.ResourcePath);
                    using (Stream fileStream = this.filesystemAccessor.GetWriteStream(targetPath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }                
            }
        }
    }
}