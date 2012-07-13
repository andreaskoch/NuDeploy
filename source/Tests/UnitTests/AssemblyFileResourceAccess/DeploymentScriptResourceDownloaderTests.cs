using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.AssemblyResourceAccess;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.AssemblyFileResourceAccess
{
    public class DeploymentScriptResourceDownloaderTests
    {
        #region constructor

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_AssemblyFileResourceProviderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IAssemblyFileResourceProvider assemblyFileResourceProvider = null;
            var filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new DeploymentScriptResourceDownloader(assemblyFileResourceProvider, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var assemblyFileResourceProvider = new Mock<IAssemblyFileResourceProvider>().Object;
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new DeploymentScriptResourceDownloader(assemblyFileResourceProvider, filesystemAccessor);
        }

        #endregion

        #region Download

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Download_TargetFolderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string targetFolder = null;

            var assemblyFileResourceProviderMock = new Mock<IAssemblyFileResourceProvider>();
            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            var deploymentScriptResourceDownloader = new DeploymentScriptResourceDownloader(assemblyFileResourceProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            deploymentScriptResourceDownloader.Download(targetFolder);
        }

        [Test]
        public void Download_GetAllAssemblyResourceInfos_GetsCalledWithTheBaseNamespace()
        {
            // Arrange
            bool getAllAssemblyResourceInfosGotCalled = false;
            string targetFolder = "target-folder";

            var assemblyFileResourceProviderMock = new Mock<IAssemblyFileResourceProvider>();
            assemblyFileResourceProviderMock.Setup(a => a.GetAllAssemblyResourceInfos(It.IsAny<string>())).Returns(
                () =>
                    {
                        getAllAssemblyResourceInfosGotCalled = true;
                        return new List<AssemblyFileResourceInfo>();
                    });

            var filesystemAccessorMock = new Mock<IFilesystemAccessor>();
            var deploymentScriptResourceDownloader = new DeploymentScriptResourceDownloader(assemblyFileResourceProviderMock.Object, filesystemAccessorMock.Object);

            // Act
            deploymentScriptResourceDownloader.Download(targetFolder);

            // Assert
            Assert.IsTrue(getAllAssemblyResourceInfosGotCalled);
        }

        #endregion
    }
}