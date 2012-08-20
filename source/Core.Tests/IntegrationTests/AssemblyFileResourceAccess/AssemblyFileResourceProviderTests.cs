using System;
using System.Linq;

using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.DeploymentScripts.PowerShell.WebServer;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.AssemblyFileResourceAccess
{
    [TestFixture]
    public class AssemblyFileResourceProviderTests
    {
        private IAssemblyFileResourceProvider assemblyFileResourceProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var sourceAssembly = typeof(WebServerDeploymentResouceInfo).Assembly;
            var assemblyResourceFilePathProvider = new AssemblyResourceFilePathProvider();
            this.assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly, assemblyResourceFilePathProvider);
        }

        [Test]
        public void GetAllAssemblyResourceInfos_BaseNamespaceIsPartOfAssembly_DeploymentScriptResourcesAreReturned()
        {
            // Arrange
            string baseNamespace = DeploymentScriptResourceDownloader.DeploymentScriptNamespace;

            // Act
            var results = this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);

            // Assert
            Assert.IsTrue(results.Count() > 0);

            foreach (var assemblyFileResourceInfo in results)
            {
                Console.WriteLine(assemblyFileResourceInfo.ResourceName);
            }
        }

        [Test]
        public void GetAllAssemblyResourceInfos_BaseNamespaceIsNotPartOfTheAssembly_ResultIsEmpty()
        {
            // Arrange
            string baseNamespace = "Some.Unknown.Namespace";

            // Act
            var results = this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);

            // Assert.
            Assert.AreEqual(0, results.Count());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetAllAssemblyResourceInfos_BaseNamespaceIsInvalid_ArgumentExceptionIsThrown(string baseNamespace)
        {
            // Act
            this.assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);
        }
    }
}