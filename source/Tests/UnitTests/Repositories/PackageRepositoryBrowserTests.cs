using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Installation.Repositories;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Repositories
{
    [TestFixture]
    public class PackageRepositoryBrowserTests
    {
        #region constructor

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SourceRepositoryProviderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISourceRepositoryProvider sourceRepositoryProvider = null;
            IPackageRepositoryFactory packageRepositoryFactory = new Mock<IPackageRepositoryFactory>().Object;

            // Act
            new PackageRepositoryBrowser(sourceRepositoryProvider, packageRepositoryFactory);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageRepositoryFactoryParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISourceRepositoryProvider sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>().Object;
            IPackageRepositoryFactory packageRepositoryFactory = null;

            // Act
            new PackageRepositoryBrowser(sourceRepositoryProvider, packageRepositoryFactory);
        }

        [Test]
        public void Constructor_ISourceRepositoryProvider_GetRepositoryConfigurations_GetsCalled()
        {
            // Arrange
            bool getRepositoryConfigurationsGotCalled = false;

            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(
                () =>
                    {
                        getRepositoryConfigurationsGotCalled = true;
                        return new List<SourceRepositoryConfiguration>();
                    });

            var packageRepositoryFactoryMock = new Mock<IPackageRepositoryFactory>();

            // Act
            new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactoryMock.Object);

            // Assert
            Assert.IsTrue(getRepositoryConfigurationsGotCalled);
        }

        [Test]
        public void Constructor_IPackageRepositoryFactory_CreateRepository_GetsCalled_ForeachEntryInRepositoryConfigurations()
        {
            // Arrange
            bool config1GotCalled = false;
            bool config2GotCalled = false;

            var sourceRepositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Test Repo 1", Url = new Uri("https://example1.org/api/v2/") };
            var sourceRepositoryConfiguration2 = new SourceRepositoryConfiguration { Name = "Test Repo 2", Url = new Uri("https://example2.org/api/v2/") };

            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(new[] { sourceRepositoryConfiguration1, sourceRepositoryConfiguration2 });

            var packageRepositoryFactoryMock = new Mock<IPackageRepositoryFactory>();
            packageRepositoryFactoryMock.Setup(f => f.CreateRepository(sourceRepositoryConfiguration1.Url.ToString())).Returns(
                () =>
                    {
                        config1GotCalled = true;
                        return new Mock<IPackageRepository>().Object;
                    });

            packageRepositoryFactoryMock.Setup(f => f.CreateRepository(sourceRepositoryConfiguration2.Url.ToString())).Returns(
                () =>
                {
                    config2GotCalled = true;
                    return new Mock<IPackageRepository>().Object;
                });

            // Act
            new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactoryMock.Object);

            // Assert
            Assert.IsTrue(config1GotCalled);
            Assert.IsTrue(config2GotCalled);
        }

        #endregion

        #region RepositoryConfigurations

        [Test]
        public void RepositoryConfigurations_ReturnsAllConfigurationsProvidedByTheSourceRepositoryProvider()
        {
            // Arrange
            var sourceRepositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Test Repo 1", Url = new Uri("https://example1.org/api/v2/") };
            var sourceRepositoryConfiguration2 = new SourceRepositoryConfiguration { Name = "Test Repo 2", Url = new Uri("https://example2.org/api/v2/") };

            var repositoryConfigurations = new List<SourceRepositoryConfiguration> { sourceRepositoryConfiguration1, sourceRepositoryConfiguration2 };

            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(repositoryConfigurations);

            var packageRepositoryFactoryMock = new Mock<IPackageRepositoryFactory>();
            packageRepositoryFactoryMock.Setup(f => f.CreateRepository(It.IsAny<string>())).Returns(new Mock<IPackageRepository>().Object);

            // Act
            var packageRepositoryBrowser = new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactoryMock.Object);

            // Assert
            Assert.AreEqual(repositoryConfigurations, packageRepositoryBrowser.RepositoryConfigurations.ToList());
        }

        [Test]
        public void RepositoryConfigurations_ISourceRepositoryProvider_GetRepositoryConfigurations_ReturnedNoConfigurations_ResultIsEmptyList()
        {
            // Arrange
            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(new List<SourceRepositoryConfiguration>());

            var packageRepositoryFactoryMock = new Mock<IPackageRepositoryFactory>();
            packageRepositoryFactoryMock.Setup(f => f.CreateRepository(It.IsAny<string>())).Returns(new Mock<IPackageRepository>().Object);

            // Act
            var packageRepositoryBrowser = new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactoryMock.Object);

            // Assert
            Assert.AreEqual(0, packageRepositoryBrowser.RepositoryConfigurations.Count());
        }

        #endregion

        #region FindPackage

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void FindPackage_PackageIdParameterIsNotValid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(new List<SourceRepositoryConfiguration>());

            var packageRepositoryFactoryMock = new Mock<IPackageRepositoryFactory>();
            packageRepositoryFactoryMock.Setup(f => f.CreateRepository(It.IsAny<string>())).Returns(new Mock<IPackageRepository>().Object);

            var packageRepositoryBrowser = new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactoryMock.Object);

            // Act
            IPackageRepository packageRepository;
            packageRepositoryBrowser.FindPackage(packageId, out packageRepository);
        }

        [Test]
        public void FindPackage_PackageIdIsValid_NoRepositoriesAreConfigured_ResultIsNull_PackageRepositoryIsNull()
        {
            // Arrange
            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(new List<SourceRepositoryConfiguration>());

            var packageRepositoryFactoryMock = new Mock<IPackageRepositoryFactory>();
            packageRepositoryFactoryMock.Setup(f => f.CreateRepository(It.IsAny<string>())).Returns(new Mock<IPackageRepository>().Object);

            var packageRepositoryBrowser = new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactoryMock.Object);

            // Act
            IPackageRepository packageRepository;
            IPackage result = packageRepositoryBrowser.FindPackage("package", out packageRepository);

            // Assert
            Assert.IsNull(result);
            Assert.IsNull(packageRepository);
        }

        #endregion
    }
}