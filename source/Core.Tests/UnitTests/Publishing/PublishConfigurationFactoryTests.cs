using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Publishing
{
    [TestFixture]
    public class PublishConfigurationFactoryTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetPublishConfiguration_ConfigurationNameParameterIsInvalid_ResultIsNull(string configurationName)
        {
            // Arrange
            string publishLocation = "http://nuget.org/api/v2";
            string apiKey = null;

            var publishConfigurationFactory = new PublishConfigurationFactory();

            // Act
            var result = publishConfigurationFactory.GetPublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetPublishConfiguration_PublishLocationParameterIsInvalid_ResultIsNull(string publishLocation)
        {
            // Arrange
            string configurationName = "Nuget.org";
            string apiKey = null;

            var publishConfigurationFactory = new PublishConfigurationFactory();

            // Act
            var result = publishConfigurationFactory.GetPublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("23298265-79b3-497f-a0d5-ab7b9daefc6a")]
        public void GetPublishConfiguration_ParameterAreValid_ResultIsNotNull(string apiKey)
        {
            // Arrange
            string publishLocation = "Nuget.org";
            string configurationName = "http://nuget.org/api/v2";

            var publishConfigurationFactory = new PublishConfigurationFactory();

            // Act
            var result = publishConfigurationFactory.GetPublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestCase("Nuget.org", "http://nuget.org/api/v2", null)]
        [TestCase("Nuget.org", "http://nuget.org/api/v2", "")]
        [TestCase("Nuget.org", "http://nuget.org/api/v2", " ")]
        [TestCase("Nuget.org", "http://nuget.org/api/v2", "23298265-79b3-497f-a0d5-ab7b9daefc6a")]
        [TestCase("Nuget.org", "C:\\local-repository", "23298265-79b3-497f-a0d5-ab7b9daefc6a")]
        [TestCase("Nuget.org", @"\\unc-path\repo", "23298265-79b3-497f-a0d5-ab7b9daefc6a")]
        public void GetPublishConfiguration_ParameterAreValid_ResultContainsParametersAsTheyWerePassedIn(string publishLocation, string configurationName, string apiKey)
        {
            // Arrange
            var publishConfigurationFactory = new PublishConfigurationFactory();

            // Act
            var result = publishConfigurationFactory.GetPublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            Assert.AreEqual(publishLocation, result.PublishLocation);
            Assert.AreEqual(configurationName, result.Name);
            Assert.AreEqual(apiKey, result.ApiKey);
        }
    }
}