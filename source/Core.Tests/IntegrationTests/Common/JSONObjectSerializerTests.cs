using System;

using NuDeploy.Core.Common.Serialization;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.Common
{
    [TestFixture]
    public class JSONObjectSerializerTests
    {
        #region Serialize

        [Test]
        public void Serialize_PublishConfiguration_JsonContainsAllProperties()
        {
            // Arrange
            var jsonObjectSerializer = new JSONObjectSerializer<PublishConfiguration>();

            var name = Guid.NewGuid().ToString();
            var location = Guid.NewGuid().ToString();
            var apiKey = Guid.NewGuid().ToString();
            var publishConfiguration = new PublishConfiguration { Name = name, PublishLocation = location, ApiKey = apiKey };

            // Act
            var result = jsonObjectSerializer.Serialize(publishConfiguration);

            // Assert
            Assert.IsTrue(result.Contains(name));
            Assert.IsTrue(result.Contains(location));
            Assert.IsTrue(result.Contains(apiKey));
        }

        #endregion

        #region Deserialize

        [Test]
        public void Deserialize_PublishConfiguration_SerializedObjectIsValidJson_ResultIsNotNull()
        {
            // Arrange
            var jsonObjectSerializer = new JSONObjectSerializer<PublishConfiguration>();

            var name = Guid.NewGuid().ToString();
            var location = Guid.NewGuid().ToString();
            var apiKey = Guid.NewGuid().ToString();
            string serializedObject = string.Format("{{ Name: \"{0}\", PublishLocation: \"{1}\", ApiKey: \"{2}\" }}", name, location, apiKey);

            // Act
            var result = jsonObjectSerializer.Deserialize(serializedObject);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Deserialize_PublishConfiguration_SerializedObjectIsValidJson_ResultContainsCorrectProperties()
        {
            // Arrange
            var jsonObjectSerializer = new JSONObjectSerializer<PublishConfiguration>();

            var name = Guid.NewGuid().ToString();
            var location = Guid.NewGuid().ToString();
            var apiKey = Guid.NewGuid().ToString();
            string serializedObject = string.Format("{{ Name: \"{0}\", PublishLocation: \"{1}\", ApiKey: \"{2}\" }}", name, location, apiKey);

            // Act
            var result = jsonObjectSerializer.Deserialize(serializedObject);

            // Assert
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(location, result.PublishLocation);
            Assert.AreEqual(apiKey, result.ApiKey);
        }

        #endregion
    }
}