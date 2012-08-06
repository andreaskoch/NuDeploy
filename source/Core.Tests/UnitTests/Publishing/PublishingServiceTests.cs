using System;
using System.IO;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Publishing;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Publishing
{
    [TestFixture]
    public class PublishingServiceTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParameterAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            // Act
            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Assert
            Assert.IsNotNull(publishingService);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            // Act
            new PublishingService(null, packageServerFactory.Object, publishConfigurationAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageServerFactoryParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            // Act
            new PublishingService(filesystemAccessor.Object, null, publishConfigurationAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishConfigurationAccessorParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();

            // Act
            new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, null);
        }

        #endregion

        #region PublishPackage

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void PublishPackage_PackagePathParameterIsInvalid_ArgumentExceptionIsThrown(string packagePath)
        {
            // Arrange
            string packageServerConfigurationName = "Nuget.org Publish Config";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            publishingService.PublishPackage(packagePath, packageServerConfigurationName);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void PublishPackage_PackageServerConfigurationNameParameterIsInvalid_ArgumentExceptionIsThrown(string packageServerConfigurationName)
        {
            // Arrange
            string packagePath = @"C:\local-nuget-repo\package.nupkg";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            filesystemAccessor.Setup(f => f.FileExists(packagePath)).Returns(true);

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            publishingService.PublishPackage(packagePath, packageServerConfigurationName);
        }

        [Test]
        public void PublishPackage_PackagePathParameterIsValidButDoesNotExist_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-nuget-repo\package.nupkg";
            string packageServerConfigurationName = "Nuget.org Publish Config";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            filesystemAccessor.Setup(f => f.FileExists(packagePath)).Returns(false);

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            var result = publishingService.PublishPackage(packagePath, packageServerConfigurationName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void PublishPackage_PublishConfigurationAccessorReturnsNull_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-nuget-repo\package.nupkg";
            string packageServerConfigurationName = "Nuget.org Publish Config";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            filesystemAccessor.Setup(f => f.FileExists(packagePath)).Returns(true);

            PublishConfiguration publishConfiguration = null;
            publishConfigurationAccessor.Setup(p => p.GetPublishConfiguration(packageServerConfigurationName)).Returns(publishConfiguration);

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            var result = publishingService.PublishPackage(packagePath, packageServerConfigurationName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void PublishPackage_PublishConfigurationAccessorReturnsInvalidConfiguration_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-nuget-repo\package.nupkg";
            string packageServerConfigurationName = "Nuget.org Publish Config";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            filesystemAccessor.Setup(f => f.FileExists(packagePath)).Returns(true);

            var publishConfiguration = new PublishConfiguration();
            publishConfigurationAccessor.Setup(p => p.GetPublishConfiguration(packageServerConfigurationName)).Returns(publishConfiguration);

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            var result = publishingService.PublishPackage(packagePath, packageServerConfigurationName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void PublishPackage_PackageServerFactoryReturnsNull_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-nuget-repo\package.nupkg";
            string packageServerConfigurationName = "Nuget.org Publish Config";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            filesystemAccessor.Setup(f => f.FileExists(packagePath)).Returns(true);

            var publishConfiguration = new PublishConfiguration { Name = packageServerConfigurationName, PublishLocation = "http://nuget.org/api/v2" };
            publishConfigurationAccessor.Setup(p => p.GetPublishConfiguration(packageServerConfigurationName)).Returns(publishConfiguration);

            PackageServer packageServer = null;
            packageServerFactory.Setup(p => p.GetPackageServer(It.IsAny<string>())).Returns(packageServer);

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            var result = publishingService.PublishPackage(packagePath, packageServerConfigurationName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void PublishPackage_PackageStreamIsNull_ResultIsFalse()
        {
            // Arrange
            string packagePath = @"C:\local-nuget-repo\package.nupkg";
            string packageServerConfigurationName = "Nuget.org Publish Config";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var packageServerFactory = new Mock<IPackageServerFactory>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            Stream packageStream = null;
            filesystemAccessor.Setup(f => f.FileExists(packagePath)).Returns(true);
            filesystemAccessor.Setup(f => f.GetReadStream(packagePath)).Returns(packageStream);

            var publishConfiguration = new PublishConfiguration { Name = packageServerConfigurationName, PublishLocation = "http://nuget.org/api/v2" };
            publishConfigurationAccessor.Setup(p => p.GetPublishConfiguration(packageServerConfigurationName)).Returns(publishConfiguration);

            var packageServer = new PackageServer(publishConfiguration.PublishLocation, NuDeployConstants.NuDeployCommandLinePackageId);
            packageServerFactory.Setup(p => p.GetPackageServer(It.IsAny<string>())).Returns(packageServer);

            var publishingService = new PublishingService(filesystemAccessor.Object, packageServerFactory.Object, publishConfigurationAccessor.Object);

            // Act
            var result = publishingService.PublishPackage(packagePath, packageServerConfigurationName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        #endregion
    }
}
