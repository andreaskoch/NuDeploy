using System;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;
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
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            // Act
            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Assert
            Assert.IsNotNull(configFileSourceRepositoryProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            // Act
            new ConfigFileSourceRepositoryProvider(null, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SourceRepositoryConfigurationFactoryParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            // Act
            new ConfigFileSourceRepositoryProvider(applicationInformation, null, filesystemPersistence.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemPersistenceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();

            // Act
            new ConfigFileSourceRepositoryProvider(applicationInformation, sourceRepositoryConfigurationFactory.Object, null);
        }

        #endregion
    }
}