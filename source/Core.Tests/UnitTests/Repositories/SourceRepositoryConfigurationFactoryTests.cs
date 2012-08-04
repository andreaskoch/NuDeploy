using NuDeploy.Core.Services.Installation.Repositories;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Repositories
{
    [TestFixture]
    public class SourceRepositoryConfigurationFactoryTests
    {
        private ISourceRepositoryConfigurationFactory sourceRepositoryConfigurationFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.sourceRepositoryConfigurationFactory = new SourceRepositoryConfigurationFactory();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetSourceRepositoryConfiguration_RepositoryNameIsInvalid_ResultIsNull(string repositoryName)
        {
            // Arrange
            string repositoryUrl = "http://nuget.org/api/v2";

            // Act
            var result = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetSourceRepositoryConfiguration_RepositoryUrlIsInvalid_ResultIsNull(string repositoryUrl)
        {
            // Arrange
            string repositoryName = "Test Repository";

            // Act
            var result = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase("some\\relative\\folder")]
        [TestCase("example.com")]
        public void GetSourceRepositoryConfiguration_RepositoryUrlCannotBeParsed_ResultIsNull(string repositoryUrl)
        {
            // Arrange
            string repositoryName = "Test Repository";

            // Act
            var result = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase("http://nuget.org/api/v2")]
        [TestCase(@"C:\local-nuget-repository")]
        [TestCase(@"\\unc-path\repository")]
        public void GetSourceRepositoryConfiguration_RepositoryUrlCanBeParsed_ResultIsNotNull(string repositoryUrl)
        {
            // Arrange
            string repositoryName = "Test Repository";

            // Act
            var result = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(repositoryName, result.Name);
        }
    }
}