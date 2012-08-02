using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Publishing
{
    [TestFixture]
    public class PublishConfigurationTests
    {
        #region IsValid

        [Test]
        public void IsValid_NameIsEmpty_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = string.Empty, PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsNull_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = null, PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsWhitespace_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = " ", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_PublishLocationIsNull_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = null };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsSet_PublishLocationIsSet_ResultIsTrue()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration.IsValid;

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_PropertyIsNotSerialed()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);

            // Assert
            string isValidPropertyName =
                typeof(PublishConfiguration).GetProperties().First(p => p.Name.Equals("isvalid", StringComparison.OrdinalIgnoreCase)).Name;

            Assert.IsFalse(json.ToLower().Contains(isValidPropertyName.ToLower()));
        }

        #endregion

        #region Serialization

        [Test]
        public void IsSerializable()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);

            // Assert
            Assert.IsTrue(json.Contains(repositoryConfiguration.Name));
            Assert.IsTrue(json.Contains(repositoryConfiguration.PublishLocation));
        }

        [Test]
        public void CanBeDeserialized()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);
            var deserializedRepositoryConfiguration = JsonConvert.DeserializeObject<PublishConfiguration>(json);

            // Assert
            Assert.AreEqual(repositoryConfiguration.Name, deserializedRepositoryConfiguration.Name);
            Assert.AreEqual(repositoryConfiguration.PublishLocation, deserializedRepositoryConfiguration.PublishLocation);
        }

        [Test]
        public void Serialization_IsValidPropertyIsNotSerialized()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(repositoryConfiguration);

            // Assert
            Assert.IsFalse(json.Contains("IsValid"));
            Assert.IsFalse(json.Contains(repositoryConfiguration.IsValid.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion

        #region ToString

        [Test]
        public void ToString_ContainsRepositoryName()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(repositoryConfiguration.Name));
        }

        [Test]
        public void ToString_ContainsRepositoryPublishLocation()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(repositoryConfiguration.PublishLocation));
        }

        [Test]
        public void ToString_ContainsPublishLocationPlaceHolder_IfNotSet()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = null, ApiKey = "Some Api Key" };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains("not-set"));
        }

        [Test]
        public void ToString_ContainsRepositoryApiKey()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2", ApiKey = "Some Api Key" };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(repositoryConfiguration.ApiKey));
        }

        [Test]
        public void ToString_ContainsApiKeyPlaceHolder_IfNotSet()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2", ApiKey = null };

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains("not-set"));
        }


        [Test]
        public void ToString_PropertiesAreNotSet_ResultIsTypeName()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration();

            // Act
            string result = repositoryConfiguration.ToString();

            // Assert
            Assert.AreEqual(typeof(PublishConfiguration).Name, result);
        }

        [Test]
        public void ToString_PublishLocationIsNull_ResultContainsName()
        {
            // Arrange
            var repositoryConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = null };

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
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var repositoryConfiguration2 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentNameCasing_ResultIsTrue()
        {
            // Arrange
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var repositoryConfiguration2 = new PublishConfiguration { Name = "some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentWhitespaces_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var repositoryConfiguration2 = new PublishConfiguration { Name = " Some Repository ", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNull_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            PublishConfiguration repositoryConfiguration2 = null;

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNotInitialized_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var repositoryConfiguration2 = new PublishConfiguration();

            // Act
            bool result = repositoryConfiguration1.Equals(repositoryConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsOfOtherType_ResultIsFalse()
        {
            // Arrange
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
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
            var repositoryConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var repositoryConfiguration2 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

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
            var repositoryConfiguration1 = new PublishConfiguration();
            var repositoryConfiguration2 = new PublishConfiguration();

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
            string repositoryPublishLocation = "http://example.com";
            var repositoryConfiguration = new PublishConfiguration { Name = repositoryName, PublishLocation = repositoryPublishLocation };

            int expectedHashcode = repositoryConfiguration.GetHashCode();

            for (var i = 0; i < 100; i++)
            {
                // Act
                repositoryConfiguration.Name = repositoryName;
                repositoryConfiguration.PublishLocation = repositoryPublishLocation;
                int generatedHashCode = repositoryConfiguration.GetHashCode();

                // Assert
                Assert.AreEqual(expectedHashcode, generatedHashCode);
            }
        }

        [Test]
        public void GetHashCode_ForAllUniqueObject_AUniqueHashCodeIsReturned()
        {
            var hashCodes = new Dictionary<int, PublishConfiguration>();

            for (var i = 0; i < 10000; i++)
            {
                // Act
                var repositoryConfiguration = new PublishConfiguration { Name = Guid.NewGuid().ToString(), PublishLocation = "http://" + Guid.NewGuid().ToString() };

                int generatedHashCode = repositoryConfiguration.GetHashCode();

                // Assert
                Assert.IsFalse(hashCodes.ContainsKey(generatedHashCode));
                hashCodes.Add(generatedHashCode, repositoryConfiguration);
            }
        }

        #endregion         
    }
}