using System;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Installation.Repositories;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Repositories
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

        #region Delete

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Delete_RepositoryNameIsInvalid_ArgumentExceptionIsThrown(string repositoryName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);
        }

        [Test]
        public void Delete_RepositoryDoesNotExist_FailureResultIsReturned()
        {
            // Arrange
            string repositoryName = "Some Repository";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            filesystemPersistence.Setup(f => f.Load(It.IsAny<string>())).Returns(new SourceRepositoryConfiguration[] { });

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            var result = configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [TestCase("Non Existing Repo 1")]
        [TestCase("Non Existing Repo 2")]
        public void Delete_RepositoryDoesNotExist_FailureResultMessageContainsRepositoryName(string repositoryName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            filesystemPersistence.Setup(f => f.Load(It.IsAny<string>())).Returns(new SourceRepositoryConfiguration[] { });

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            var result = configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            Assert.IsTrue(result.Message.Contains(repositoryName), "The failure result should contain the name of the repository that shall be deleted.");
        }

        [Test]
        public void Delete_RepositoryExists_TheSpecificRepostoryIsExcludedFromTheRepositoryList()
        {
            // Arrange
            string repositoryName = "Repo B";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            var existingRepositories = new[]
                {
                    new SourceRepositoryConfiguration { Name = "Repo A", Url = new Uri("http://1.example.com/api/v2") },
                    new SourceRepositoryConfiguration { Name = "Repo B", Url = new Uri("http://2.example.com/api/v2") }
                };

            filesystemPersistence.Setup(f => f.Load(It.IsAny<string>())).Returns(existingRepositories);
            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(true);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            filesystemPersistence.Verify(
                f =>
                f.Save(
                    It.Is<SourceRepositoryConfiguration[]>(configurations => configurations.Length == 1 && configurations.First().Name == "Repo A"),
                    It.IsAny<string>()),
                Times.Once());
        }

        [TestCase("repo b")]
        [TestCase("REPO b")]
        [TestCase("REPO B")]
        [TestCase("Repo B")]
        public void Delete_RepositoryExists_TheSpecificRepostoryIsExcludedFromTheRepositoryList_CaseIsIgnored(string repositoryName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            var existingRepositories = new[]
                {
                    new SourceRepositoryConfiguration { Name = "Repo A", Url = new Uri("http://1.example.com/api/v2") },
                    new SourceRepositoryConfiguration { Name = "Repo B", Url = new Uri("http://2.example.com/api/v2") }
                };

            filesystemPersistence.Setup(f => f.Load(It.IsAny<string>())).Returns(existingRepositories);
            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(true);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            filesystemPersistence.Verify(
                f =>
                f.Save(
                    It.Is<SourceRepositoryConfiguration[]>(configurations => configurations.Length == 1 && configurations.First().Name == "Repo A"),
                    It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void Delete_RepositoryExists_SaveFails_FailureResultIsReturned()
        {
            // Arrange
            string repositoryName = "Repo B";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            var existingRepositories = new[]
                {
                    new SourceRepositoryConfiguration { Name = "Repo A", Url = new Uri("http://1.example.com/api/v2") },
                    new SourceRepositoryConfiguration { Name = "Repo B", Url = new Uri("http://2.example.com/api/v2") }
                };

            filesystemPersistence.Setup(f => f.Load(It.IsAny<string>())).Returns(existingRepositories);
            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(false);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            var result = configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Delete_RepositoryExists_SaveSucceeds_SuccessResultIsReturned()
        {
            // Arrange
            string repositoryName = "Repo B";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            var existingRepositories = new[]
                {
                    new SourceRepositoryConfiguration { Name = "Repo A", Url = new Uri("http://1.example.com/api/v2") },
                    new SourceRepositoryConfiguration { Name = "Repo B", Url = new Uri("http://2.example.com/api/v2") }
                };

            filesystemPersistence.Setup(f => f.Load(It.IsAny<string>())).Returns(existingRepositories);
            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(true);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            var result = configFileSourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        #endregion

        #region Reset

        [Test]
        public void Reset_SaveFails_FailureResultIsReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(false);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            var result = configFileSourceRepositoryProvider.ResetRepositoryConfiguration();

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }

        [Test]
        public void Reset_SaveSucceeds_SuccessResultIsReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(true);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            var result = configFileSourceRepositoryProvider.ResetRepositoryConfiguration();

            // Assert
            Assert.AreEqual(ServiceResultType.Success, result.Status);
        }

        [Test]
        public void Reset_DefaultRepositoryIsSaved()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var sourceRepositoryConfigurationFactory = new Mock<ISourceRepositoryConfigurationFactory>();
            var filesystemPersistence = new Mock<IFilesystemPersistence<SourceRepositoryConfiguration[]>>();

            filesystemPersistence.Setup(f => f.Save(It.IsAny<SourceRepositoryConfiguration[]>(), It.IsAny<string>())).Returns(true);

            var configFileSourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(
                applicationInformation, sourceRepositoryConfigurationFactory.Object, filesystemPersistence.Object);

            // Act
            configFileSourceRepositoryProvider.ResetRepositoryConfiguration();

            // Assert
            filesystemPersistence.Verify(
                f =>
                f.Save(
                    It.Is<SourceRepositoryConfiguration[]>(
                        configurations => configurations.Length == 1 && configurations.First().Name == ConfigFileSourceRepositoryProvider.DefaultRepositoryName),
                    It.IsAny<string>()),
                Times.Once());
        }

        #endregion
    }
}