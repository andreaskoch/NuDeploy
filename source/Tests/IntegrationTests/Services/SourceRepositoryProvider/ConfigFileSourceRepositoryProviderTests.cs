using System;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Services.SourceRepositoryProvider
{
    [TestFixture]
    public class ConfigFileSourceRepositoryProviderTests
    {
        private ConfigFileSourceRepositoryProvider sourceRepositoryProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.sourceRepositoryProvider = new ConfigFileSourceRepositoryProvider();
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
            var result = this.sourceRepositoryProvider.GetRepositories().ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(NuDeployConstants.DefaultFeedUrl, result.First().Url);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveRepository_ParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.SaveRepository(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveRepository_SuppliedRepositoryIsInvalid_ArgumentExceptionIsThrown()
        {
            // Arrange
            var repository = new SourceRepository { Name = string.Empty, Url = string.Empty };

            // Act
            this.sourceRepositoryProvider.SaveRepository(repository);
        }

        [Test]
        public void SaveRepository_EntryAlreadyExists_ResultContainsOnlyOneEntry()
        {
            // Arrange
            var repository = this.sourceRepositoryProvider.GetRepositories().First();

            // Act
            this.sourceRepositoryProvider.SaveRepository(repository);

            // Assert
            var result = this.sourceRepositoryProvider.GetRepositories().ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(NuDeployConstants.DefaultFeedUrl, result.First().Url);
        }

        [Test]
        public void SaveRepository_SuppliedRepositoryIsNewEntry_ResultContainsTheSuppliedRepository()
        {
            // Arrange
            string repositoryName = "Test Repository";
            string repositoryUrl = "http://sample-url.com";
            var repository = new SourceRepository { Name = repositoryName, Url = repositoryUrl };

            // Act
            this.sourceRepositoryProvider.SaveRepository(repository);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositories().ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Any(r => r.Name.Equals(repositoryName)));
        }

        [Test]
        public void SaveRepository_TwoRepositoriesWithTheSameName_OnlyTheLastRepositoryIsSaved()
        {
            // Arrange
            string repositoryName = "Test Repository";
            string repository1Url = "http://sample-url.com/1";
            string repository2Url = "http://sample-url.com/2";

            var repository1 = new SourceRepository { Name = repositoryName, Url = repository1Url };
            var repository2 = new SourceRepository { Name = repositoryName, Url = repository2Url };

            // Act
            this.sourceRepositoryProvider.SaveRepository(repository1);
            this.sourceRepositoryProvider.SaveRepository(repository2);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositories().ToList();
            var savedRepo = results.FirstOrDefault(r => r.Name.Equals(repositoryName));

            Assert.IsNotNull(savedRepo);
            Assert.AreEqual(repository2Url, savedRepo.Url);
        }

        [Test]
        public void DeleteRepository_RemoveNonExistingRepository_NoRepositoryIsRemoved()
        {
            // Arrange
            var repositoryName = "Non existing Repo";

            // Act
            this.sourceRepositoryProvider.DeleteRepository(repositoryName);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositories().ToList();
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void DeleteRepository_RemoveDefaultRepository_ResultIsEmpty()
        {
            // Arrange
            var initialRepository = this.sourceRepositoryProvider.GetRepositories().First();
            var repositoryName = initialRepository.Name;

            // Act
            this.sourceRepositoryProvider.DeleteRepository(repositoryName);

            // Assert
            var results = this.sourceRepositoryProvider.GetRepositories().ToList();

            Assert.AreEqual(0, results.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteRepository_InvalidRepositoryName_Null_ArgumentExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.DeleteRepository(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteRepository_InvalidRepositoryName_Empty_ArgumentExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.DeleteRepository(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteRepository_InvalidRepositoryName_Whitespace_ArgumentExceptionIsThrown()
        {
            // Act
            this.sourceRepositoryProvider.DeleteRepository(" ");
        }
    }
}
