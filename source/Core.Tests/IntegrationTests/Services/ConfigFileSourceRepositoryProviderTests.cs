using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Installation.Repositories;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class ConfigFileSourceRepositoryProviderTests
    {
        private ConfigFileSourceRepositoryProvider sourceRepositoryProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            var encodingProvider = new DefaultFileEncodingProvider();
            var fileSystemAccessor = new PhysicalFilesystemAccessor(encodingProvider);
            var sourceRepositoryConfigurationFactory = new SourceRepositoryConfigurationFactory();

            this.sourceRepositoryProvider = new ConfigFileSourceRepositoryProvider(applicationInformation, fileSystemAccessor, sourceRepositoryConfigurationFactory);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            File.Delete(ConfigFileSourceRepositoryProvider.SourceRepositoryConfigurationFileName);
        }

        [Test]
        public void GetRepositories_ConfigFileDoesNotExist_ResultContainsDefaultRepository()
        {
            // Act
            var result = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(NuDeployConstants.DefaultFeedUrl, result.First().Url);
        }

        [Test]
        public void SaveRepository_SuppliedRepositoryIsInvalid_ResultIsFalse()
        {
            // Arrange
            string repositoryName = string.Empty;
            string repositoryUrl = "http://sample-url.com";

            // Act
            bool result = this.sourceRepositoryProvider.SaveRepositoryConfiguration(repositoryName, repositoryUrl);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SaveRepository_EntryAlreadyExists_ResultContainsOnlyOneEntry()
        {
            // Arrange
            var repository = this.sourceRepositoryProvider.GetRepositoryConfigurations().First();

            // Act
            this.sourceRepositoryProvider.SaveRepositoryConfiguration(repository.Name, repository.Url.ToString());

            // Assert
            var result = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(NuDeployConstants.DefaultFeedUrl, result.First().Url);
        }

        [Test]
        public void SaveRepository_SuppliedRepositoryIsNewEntry_ResultContainsTheSuppliedRepository()
        {
            // Arrange
            var repositoryName = "Test Repository";
            var repositoryUrl = "http://sample-url.com";

            // Act
            this.sourceRepositoryProvider.SaveRepositoryConfiguration(repositoryName, repositoryUrl);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Any(r => r.Name.Equals(repositoryName)));
        }

        [Test]
        public void SaveRepository_TwoRepositoriesWithTheSameName_OnlyTheLastRepositoryIsSaved()
        {
            // Arrange
            var repositoryName = "Test Repository";
            var repository1Url = "http://sample-url.com/1";
            var repository2Url = "http://sample-url.com/2";

            // Act
            this.sourceRepositoryProvider.SaveRepositoryConfiguration(repositoryName, repository1Url);
            this.sourceRepositoryProvider.SaveRepositoryConfiguration(repositoryName, repository2Url);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();
            var savedRepo = results.FirstOrDefault(r => r.Name.Equals(repositoryName));

            Assert.IsNotNull(savedRepo);
            Assert.AreEqual(repository2Url, savedRepo.Url.ToString());
        }

        [Test]
        public void DeleteRepository_RemoveNonExistingRepository_NoRepositoryIsRemoved()
        {
            // Arrange
            var repositoryName = "Non existing Repo";

            // Act
            this.sourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void DeleteRepository_RemoveDefaultRepository_ResultIsEmpty()
        {
            // Arrange
            var initialRepository = this.sourceRepositoryProvider.GetRepositoryConfigurations().First();
            var repositoryName = initialRepository.Name;

            // Act
            this.sourceRepositoryProvider.DeleteRepositoryConfiguration(repositoryName);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();

            Assert.AreEqual(0, results.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteRepository_InvalidRepositoryName_Null_ArgumentExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.DeleteRepositoryConfiguration(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteRepository_InvalidRepositoryName_Empty_ArgumentExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.DeleteRepositoryConfiguration(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteRepository_InvalidRepositoryName_Whitespace_ArgumentExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.DeleteRepositoryConfiguration(" ");
        }

        [Test]
        public void ResetRepositoryConfiguration()
        {
            // Arrange
            var config = new SourceRepositoryConfiguration { Name = "Test Repo j32kl4jkl12j4kl32j4klj32", Url = new Uri("C:\\local-test-repo") };
            this.sourceRepositoryProvider.SaveRepositoryConfiguration(config.Name, config.Url.ToString());
            string fileContentBefore = File.ReadAllText(ConfigFileSourceRepositoryProvider.SourceRepositoryConfigurationFileName);

            // Act
            this.sourceRepositoryProvider.ResetRepositoryConfiguration();

            // Assert
            string fileContentAfter = File.ReadAllText(ConfigFileSourceRepositoryProvider.SourceRepositoryConfigurationFileName);
            Assert.AreNotEqual(fileContentBefore, fileContentAfter);
            Assert.IsFalse(this.sourceRepositoryProvider.GetRepositoryConfigurations().Any(c => c.Name.Equals(config.Name)));
        }
    }
}
