using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Common;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Common
{
    [TestFixture]
    public class SourceRepositoryConfigurationTests
    {
        #region IsValid

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

        #endregion

        #region serialization

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

        #endregion

        #region ToString

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
        public void ToString_PropertiesAreNotSet_ResultIsTypeName()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration();

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.AreEqual(typeof(SourceRepositoryConfiguration).Name, result);
        }

        [Test]
        public void ToString_UrlIsNull_ResultContainsName()
        {
            // Arrange
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = "Some Repository", Url = null };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(repositoryConfiguration.Name));
        }

        #endregion

        #region Equals

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

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentWhitespaces_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            var repositoryConfiguration2 = new SourceRepositoryConfiguration { Name = " Some Repository ", Url = new Uri("http://example.com") };

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNull_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            SourceRepositoryConfiguration repositoryConfiguration2 = null;

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNotInitialized_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            var repositoryConfiguration2 = new SourceRepositoryConfiguration();

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsOfOtherType_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            object repositoryConfiguration2 = new object();

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetHashCode

        [Test]
        public void GetHashCode_TwoIdenticalObjects_BothInitialized_HashCodesAreEqual()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };
            var repositoryConfiguration2 = new SourceRepositoryConfiguration { Name = "Some Repository", Url = new Uri("http://example.com") };

            // Act
            int hashCodeObject1 = repositoryConfiguration1.GetHashCode();
            int hashCodeObject2 = repositoryConfiguration2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_TwoIdenticalObjects_BothUninitialized_HashCodesAreEqual()
        {
            // Arrange
            var repositoryConfiguration1 = new SourceRepositoryConfiguration();
            var repositoryConfiguration2 = new SourceRepositoryConfiguration();

            // Act
            int hashCodeObject1 = repositoryConfiguration1.GetHashCode();
            int hashCodeObject2 = repositoryConfiguration2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_SameHashCodeIsReturnedEveryTimeTheMethodIsCalled_AsLongAsTheObjectDoesNotChange()
        {
            // Arrange
            string repositoryName = "Some Repository";
            string repositoryUrl = "http://example.com";
            var repositoryConfiguration = new SourceRepositoryConfiguration { Name = repositoryName, Url = new Uri(repositoryUrl) };

            int expectedHashcode = repositoryConfiguration.GetHashCode();

            for (var i = 0; i < 100; i++)
            {
                // Act
                repositoryConfiguration.Name = repositoryName;
                repositoryConfiguration.Url = new Uri(repositoryUrl);
                int generatedHashCode = repositoryConfiguration.GetHashCode();

                // Assert
                Assert.AreEqual(expectedHashcode, generatedHashCode);                
            }
        }

        [Test]
        public void GetHashCode_ForAllUniqueObject_AUniqueHashCodeIsReturned()
        {
            var hashCodes = new Dictionary<int, SourceRepositoryConfiguration>();

            for (var i = 0; i < 10000; i++)
            {
                // Act
                var repositoryConfiguration = new SourceRepositoryConfiguration
                    { Name = Guid.NewGuid().ToString(), Url = new Uri("http://" + Guid.NewGuid().ToString()) };

                int generatedHashCode = repositoryConfiguration.GetHashCode();

                // Assert
                Assert.IsFalse(hashCodes.ContainsKey(generatedHashCode));
                hashCodes.Add(generatedHashCode, repositoryConfiguration);
            }
        }

        #endregion
    }
}