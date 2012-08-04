using System;

using NuDeploy.Core.Common.Serialization;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Common
{
    [TestFixture]
    public class JSONObjectSerializerTests
    {
        #region Deserialize

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_ObjectToSerializeParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var jsonObjectSerializer = new JSONObjectSerializer<PublishConfiguration>();
            PublishConfiguration publishConfiguration = null;

            // Act
            jsonObjectSerializer.Serialize(publishConfiguration);
        }

        #endregion

        #region Deserialize

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_SerializedObjectParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var jsonObjectSerializer = new JSONObjectSerializer<PublishConfiguration>();
            string serializedObject = null;

            // Act
            jsonObjectSerializer.Deserialize(serializedObject);
        }

        #endregion
    }
}