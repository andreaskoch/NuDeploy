using System;
using System.Linq;
using System.Runtime.InteropServices;

using Moq;

using NuDeploy.Core.Services.AssemblyResourceAccess;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.AssemblyFileResourceAccess
{
    [TestFixture]
    public class AssemblyFileResourceProviderTests
    {
        private const string AssemblyResourcePattern = "{0}.Folder{1}.SuFolder.{1}.File1.ext";

        #region constructor
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SourceAssemblyParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            _Assembly sourceAssembly = null;
            var assemblyResourceFilePathProvider = new Mock<IAssemblyResourceFilePathProvider>().Object;

            // Act
            new AssemblyFileResourceProvider(sourceAssembly, assemblyResourceFilePathProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_AssemblyResourceFilePathProviderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var sourceAssembly = new Mock<_Assembly>().Object;
            IAssemblyResourceFilePathProvider assemblyResourceFilePathProvider = null;

            // Act
            new AssemblyFileResourceProvider(sourceAssembly, assemblyResourceFilePathProvider);
        }

        #endregion

        #region SourceAssembly Property

        [Test]
        public void SourceAssembly_PropertyReturnsTheAssemblySuppliedToTheConstructor()
        {
            // Arrange
            var sourceAssembly = new Mock<_Assembly>().Object;
            var assemblyResourceFilePathProvider = new Mock<IAssemblyResourceFilePathProvider>().Object;

            // Act
            IAssemblyFileResourceProvider assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly, assemblyResourceFilePathProvider);            

            // Assert.
            Assert.AreEqual(sourceAssembly, assemblyFileResourceProvider.SourceAssembly);
        }

        #endregion

        #region GetAllAssemblyResourceInfos

        [Test]
        public void GetAllAssemblyResourceInfos_AssemblyContainsTheResourcesWithSuppliedBaseNameSpace_ThreeMatchingResultsAreReturned()
        {
            // Arrange
            string baseNamespace = "Assembly.Resource";

            var assemblyResourceNames = new[]
                {
                    string.Format(AssemblyResourcePattern, baseNamespace, 1),
                    string.Format(AssemblyResourcePattern, baseNamespace, 2),
                    string.Format(AssemblyResourcePattern, baseNamespace, 3),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 1),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace2", 1)
                };

            var sourceAssembly = new Mock<_Assembly>();
            sourceAssembly.Setup(a => a.GetManifestResourceNames()).Returns(assemblyResourceNames);

            var assemblyResourceFilePathProvider = new Mock<IAssemblyResourceFilePathProvider>().Object;
            IAssemblyFileResourceProvider assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly.Object, assemblyResourceFilePathProvider);

            // Act
            var results = assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);

            // Assert.
            Assert.AreEqual(3, results.Count());
        }

        [Test]
        public void GetAllAssemblyResourceInfos_AssemblyResourceFilePathProvider_GetRelativeFilePath_GetCalledForEachResource()
        {
            // Arrange
            int numberOfTimesGetRelativeFilePathGotCalled = 0;
            string baseNamespace = "Assembly.Resource";

            var assemblyResourceNames = new[]
                {
                    string.Format(AssemblyResourcePattern, baseNamespace, 1),
                    string.Format(AssemblyResourcePattern, baseNamespace, 2),
                    string.Format(AssemblyResourcePattern, baseNamespace, 3)
                };

            var sourceAssembly = new Mock<_Assembly>();
            sourceAssembly.Setup(a => a.GetManifestResourceNames()).Returns(assemblyResourceNames);

            var assemblyResourceFilePathProvider = new Mock<IAssemblyResourceFilePathProvider>();
            assemblyResourceFilePathProvider.Setup(p => p.GetRelativeFilePath(It.IsAny<string>(), It.IsAny<string>())).Returns(
                () =>
                    {
                        numberOfTimesGetRelativeFilePathGotCalled++;
                        return "Some\\Path\\" + numberOfTimesGetRelativeFilePathGotCalled;
                    });

            IAssemblyFileResourceProvider assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly.Object, assemblyResourceFilePathProvider.Object);

            // Act
            var results = assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);

            // Assert.
            Assert.AreEqual(results.Count(), numberOfTimesGetRelativeFilePathGotCalled);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetAllAssemblyResourceInfos_SuppliedBaseNamespaceIsNotSet_ArgumentExceptionIsThrown(string baseNamespace)
        {
            // Arrange
            var assemblyResourceNames = new[]
                {
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 1),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 2),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 3),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace2", 1)
                };

            var sourceAssembly = new Mock<_Assembly>();
            sourceAssembly.Setup(a => a.GetManifestResourceNames()).Returns(assemblyResourceNames);

            var assemblyResourceFilePathProvider = new Mock<IAssemblyResourceFilePathProvider>().Object;
            IAssemblyFileResourceProvider assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly.Object, assemblyResourceFilePathProvider);

            // Act
            assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);
        }

        [TestCase("Unknown.BaseNamespace")]
        [TestCase("Unknown.BaseNamespace2")]
        public void GetAllAssemblyResourceInfos_AssemblyContainsNoResourcesTheSuppliedBaseNameSpace_NoResultsAreReturned(string baseNamespace)
        {
            // Arrange
            var assemblyResourceNames = new[]
                {
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 1),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 2),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace1", 3),
                    string.Format(AssemblyResourcePattern, "Some.Other.Namespace2", 1)
                };

            var sourceAssembly = new Mock<_Assembly>();
            sourceAssembly.Setup(a => a.GetManifestResourceNames()).Returns(assemblyResourceNames);

            var assemblyResourceFilePathProvider = new Mock<IAssemblyResourceFilePathProvider>().Object;
            IAssemblyFileResourceProvider assemblyFileResourceProvider = new AssemblyFileResourceProvider(sourceAssembly.Object, assemblyResourceFilePathProvider);

            // Act
            var results = assemblyFileResourceProvider.GetAllAssemblyResourceInfos(baseNamespace);

            // Assert.
            Assert.AreEqual(0, results.Count());
        }

        #endregion
    }
}