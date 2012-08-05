using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Publishing
{
    [TestFixture]
    public class PublishConfigurationTests
    {
        #region IsValid

        [Test]
        public void IsValid_NameIsEmpty_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = string.Empty, PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsNull_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = null, PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsWhitespace_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = " ", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_PublishLocationIsNull_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = null };

            // Act
            bool result = publishConfiguration.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsValid_NameIsSet_PublishLocationIsSet_ResultIsTrue()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration.IsValid;

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_PropertyIsNotSerialed()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(publishConfiguration);

            // Assert
            string isValidPropertyName =
                typeof(PublishConfiguration).GetProperties().First(p => p.Name.Equals("isvalid", StringComparison.OrdinalIgnoreCase)).Name;

            Assert.IsFalse(json.ToLower().Contains(isValidPropertyName.ToLower()));
        }

        #endregion

        #region IsLocal

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void IsLocal_PublishLocationIsEmpty_ResultIsTrue(string publishLocation)
        {
            // Arrage
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = publishLocation };

            // Act
            var result = publishConfiguration.IsLocal;

            // Assert
            Assert.IsTrue(result);
        }

        [TestCase("/api/v2")]
        [TestCase(@".\relative-folder")]
        public void IsLocal_PublishLocationIsRelative_ResultIsTrue(string publishLocation)
        {
            // Arrage
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = publishLocation };

            // Act
            var result = publishConfiguration.IsLocal;

            // Assert
            Assert.IsTrue(result);
        }

        [TestCase("http://nuget.org/api/v2")]
        [TestCase("https://nuget.org/api/v2")]
        public void IsLocal_PublishLocationIsAbsolute_IsRemoteLocation_ResultIsFalse(string publishLocation)
        {
            // Arrage
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = publishLocation };

            // Act
            var result = publishConfiguration.IsLocal;

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase(@"\\unc-path\repsository")]
        public void IsLocal_PublishLocationIsAbsolute_IsUncPath_ResultIsTrue(string publishLocation)
        {
            // Arrage
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = publishLocation };

            // Act
            var result = publishConfiguration.IsLocal;

            // Assert
            Assert.True(result);
        }

        [TestCase(@"C:\local-repsository")]
        public void IsLocal_PublishLocationIsAbsolute_IsLocal_ResultIsTrue(string publishLocation)
        {
            // Arrage
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = publishLocation };

            // Act
            var result = publishConfiguration.IsLocal;

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Serialization

        [Test]
        public void IsSerializable()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(publishConfiguration);

            // Assert
            Assert.IsTrue(json.Contains(publishConfiguration.Name));
            Assert.IsTrue(json.Contains(publishConfiguration.PublishLocation));
        }

        [Test]
        public void CanBeDeserialized()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(publishConfiguration);
            var deserializedpublishConfiguration = JsonConvert.DeserializeObject<PublishConfiguration>(json);

            // Assert
            Assert.AreEqual(publishConfiguration.Name, deserializedpublishConfiguration.Name);
            Assert.AreEqual(publishConfiguration.PublishLocation, deserializedpublishConfiguration.PublishLocation);
        }

        [Test]
        public void Serialization_IsValidPropertyIsNotSerialized()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string json = JsonConvert.SerializeObject(publishConfiguration);

            // Assert
            Assert.IsFalse(json.Contains("IsValid"));
            Assert.IsFalse(json.Contains(publishConfiguration.IsValid.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion

        #region ToString

        [Test]
        public void ToString_ContainsRepositoryName()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(publishConfiguration.Name));
        }

        [Test]
        public void ToString_ContainsRepositoryPublishLocation()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(publishConfiguration.PublishLocation));
        }

        [Test]
        public void ToString_ContainsPublishLocationPlaceHolder_IfNotSet()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = null, ApiKey = "Some Api Key" };

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains("not-set"));
        }

        [Test]
        public void ToString_ContainsRepositoryApiKey()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2", ApiKey = "Some Api Key" };

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(publishConfiguration.ApiKey));
        }

        [Test]
        public void ToString_ContainsApiKeyPlaceHolder_IfNotSet()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2", ApiKey = null };

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains("not-set"));
        }


        [Test]
        public void ToString_PropertiesAreNotSet_ResultIsTypeName()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration();

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.AreEqual(typeof(PublishConfiguration).Name, result);
        }

        [Test]
        public void ToString_PublishLocationIsNull_ResultContainsName()
        {
            // Arrange
            var publishConfiguration = new PublishConfiguration { Name = "Some Repository", PublishLocation = null };

            // Act
            string result = publishConfiguration.ToString();

            // Assert
            Assert.IsTrue(result.Contains(publishConfiguration.Name));
        }

        #endregion

        #region Equals

        [Test]
        public void Equals_TwoIdenticalConfigurations_ResultIsTrue()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var publishConfiguration2 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration1.Equals(publishConfiguration2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentNameCasing_ResultIsTrue()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var publishConfiguration2 = new PublishConfiguration { Name = "some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration1.Equals(publishConfiguration2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentWhitespaces_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var publishConfiguration2 = new PublishConfiguration { Name = " Some Repository ", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            bool result = publishConfiguration1.Equals(publishConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNull_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            PublishConfiguration publishConfiguration2 = null;

            // Act
            bool result = publishConfiguration1.Equals(publishConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNotInitialized_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var publishConfiguration2 = new PublishConfiguration();

            // Act
            bool result = publishConfiguration1.Equals(publishConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsOfOtherType_ResultIsFalse()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            object publishConfiguration2 = new object();

            // Act
            bool result = publishConfiguration1.Equals(publishConfiguration2);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetHashCode

        [Test]
        public void GetHashCode_TwoIdenticalObjects_BothInitialized_HashCodesAreEqual()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };
            var publishConfiguration2 = new PublishConfiguration { Name = "Some Repository", PublishLocation = "http://nuget.org/api/v2" };

            // Act
            int hashCodeObject1 = publishConfiguration1.GetHashCode();
            int hashCodeObject2 = publishConfiguration2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_TwoIdenticalObjects_BothUninitialized_HashCodesAreEqual()
        {
            // Arrange
            var publishConfiguration1 = new PublishConfiguration();
            var publishConfiguration2 = new PublishConfiguration();

            // Act
            int hashCodeObject1 = publishConfiguration1.GetHashCode();
            int hashCodeObject2 = publishConfiguration2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_SameHashCodeIsReturnedEveryTimeTheMethodIsCalled_AsLongAsTheObjectDoesNotChange()
        {
            // Arrange
            string repositoryName = "Some Repository";
            string repositoryPublishLocation = "http://example.com";
            var publishConfiguration = new PublishConfiguration { Name = repositoryName, PublishLocation = repositoryPublishLocation };

            int expectedHashcode = publishConfiguration.GetHashCode();

            for (var i = 0; i < 100; i++)
            {
                // Act
                publishConfiguration.Name = repositoryName;
                publishConfiguration.PublishLocation = repositoryPublishLocation;
                int generatedHashCode = publishConfiguration.GetHashCode();

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
                var publishConfiguration = new PublishConfiguration { Name = Guid.NewGuid().ToString(), PublishLocation = "http://" + Guid.NewGuid().ToString() };

                int generatedHashCode = publishConfiguration.GetHashCode();

                // Assert
                Assert.IsFalse(hashCodes.ContainsKey(generatedHashCode));
                hashCodes.Add(generatedHashCode, publishConfiguration);
            }
        }

        #endregion         
    }
}