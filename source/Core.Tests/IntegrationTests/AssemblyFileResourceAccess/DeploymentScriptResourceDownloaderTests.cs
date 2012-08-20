using System.IO;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.DeploymentScripts.PowerShell.WebServer;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.AssemblyFileResourceAccess
{
    [TestFixture]
    public class DeploymentScriptResourceDownloaderTests
    {
        private const string DownloadTempTargetFolder = "temp-downloads";

        private IAssemblyResourceDownloader deploymentScriptResourceDownloader;

        private IAssemblyFileResourceProvider assemblyFileResourceProvider;

        [TestFixtureSetUp] 
        public void Setup()
        {
            var encodingProvider = new DefaultFileEncodingProvider();
            var assemblyResourceFilePathProvider = new AssemblyResourceFilePathProvider();
            var sourceAssembly = typeof(WebServerDeploymentResouceInfo).Assembly;
            var filesystemAccessor = new PhysicalFilesystemAccessor(encodingProvider);

            this.assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly, assemblyResourceFilePathProvider);
            this.deploymentScriptResourceDownloader = new DeploymentScriptResourceDownloader(this.assemblyFileResourceProvider, filesystemAccessor);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            string tempFolderPath = DownloadTempTargetFolder;

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }

            Directory.CreateDirectory(tempFolderPath);
        }

        [Test]
        public void Download_FilesAreDownloaded()
        {
            // Arrange
            string targetFolder = DownloadTempTargetFolder;

            // Act
            this.deploymentScriptResourceDownloader.Download(targetFolder);

            // Assert
            var files = Directory.GetFiles(targetFolder, "*", SearchOption.AllDirectories);
            Assert.IsTrue(files.Length > 0);
        }

        [Test]
        public void Download_AllFilesThatAreReturnedByTheAssemblyFileResourceProvider_AreDownloaded()
        {
            // Arrange
            var resources = this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos(DeploymentScriptResourceDownloader.DeploymentScriptNamespace);
            string targetFolder = DownloadTempTargetFolder;

            // Act
            this.deploymentScriptResourceDownloader.Download(targetFolder);

            // Assert
            foreach (var assemblyFileResourceInfo in resources)
            {
                string filePath = Path.Combine(targetFolder, assemblyFileResourceInfo.ResourcePath);
                Assert.IsTrue(File.Exists(filePath));
            }
        }
    }
}