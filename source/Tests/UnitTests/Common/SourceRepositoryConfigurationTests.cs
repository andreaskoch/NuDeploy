using System;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Common;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Common
{
    [TestFixture]
    public class SourceRepositoryConfigurationTests
    {
        [Test]
        public void IsValid_NameIsEmpty_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = string.Empty, Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsNull_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = null, Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsWhitespace_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = " ", Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_UrlIsNull_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = null };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsSet_UrlIsSet_ResultIsTrue()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_PropertyIsNotSerialed()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);

            // Assert
            string isValidPropertyName =
                typeof(SourceRepositoryConfiguration).GetProperties().First(p => p.Name.Equals("isvalid", StringComparison.OrdinalIgnoreCase)).Name;

            Assert.IsFalse(json.ToLower().Contains(isValidPropertyName.ToLower()));
        }

        [Test]
        public void IsSerializable()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);

            // Assert
            Assert.IsTrue(json.Contains(repositoryConfiguration.Name));
            Assert.IsTrue(json.Contains(repositoryConfiguration.Url.ToString()));
        }

        [Test]
        public void CanBeDeserialized()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);
            var deserializedRepositoryConfiguration = JsonConvert.DeserializeObject<SourceRepositoryConfiguration>(json);

            // Assert
            Assert.AreEqual(repositoryConfiguration.Name, deserializedRepositoryConfiguration.Name);
            Assert.AreEqual(repositoryConfiguration.Url, deserializedRepositoryConfiguration.Url);
        }

        [Test]
        public void ToString_ContainsRepositoryName()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(repositoryConfiguration.Name));
        }

        [Test]
        public void ToString_ContainsRepositoryUrl()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(repositoryConfiguration.Url.ToString()));
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_ResultIsTrue()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            var repositoryConfiguration2 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentNameCasing_ResultIsTrue()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            var repositoryConfiguration2 = new SourceRepositoryConfiguration { Name = "some Repository", Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsTrue(result);
        }
    }
}