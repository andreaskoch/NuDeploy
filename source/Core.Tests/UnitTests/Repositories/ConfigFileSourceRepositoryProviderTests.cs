using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Installation.Repositories;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Repositories
{
    [TestFixture]
    public class ConfigFileSourceRepositoryProviderTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();

            // Act
            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, filesystemAccessor.Object, sourceRepositoryConfigurationFactory.Object);

            // Assert
            Assert.IsNotNull(configFileSourceRepositoryProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();

            // Act
            new ConfigFileSourceRepositoryProvider(null, filesystemAccessor.Object, sourceRepositoryConfigurationFactory.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();

            // Act
            new ConfigFileSourceRepositoryProvider(applicationInformation, null, sourceRepositoryConfigurationFactory.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SourceRepositoryConfigurationFactoryParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new ConfigFileSourceRepositoryProvider(applicationInformation, filesystemAccessor.Object, null);
        }

        #endregion
    }
}