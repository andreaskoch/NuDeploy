using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Persistence;
using NuDeploy.Core.Common.Serialization;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Persistence
{
    [TestFixture]
    public class FilesystemPersistenceTests
    {
        #region constructor<PublishConfiguration>

        [Test]
        public void Constructor_PublishConfiguration_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            // Act
            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Assert
            Assert.IsNotNull(filesystemPersistence);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishConfiguration_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            // Act
            new FilesystemPersistence<PublishConfiguration>(null, objectSerializer.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishConfiguration_ObjectSerializerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, null);
        }

        #endregion

        #region Save<PublishConfiguration>

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_PublishConfiguration_ObjectToPersistIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            PublishConfiguration objectToPersist = null;
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            filesystemPersistence.Save(objectToPersist, storageLocation);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Save_PublishConfiguration_StorageLocationIsInvalid_ArgumentExceptionIsThrown(string storageLocation)
        {
            // Arrange
            var objectToPersist = new PublishConfiguration();

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            filesystemPersistence.Save(objectToPersist, storageLocation);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Save_PublishConfiguration_ObjectSerializerReturnInvalidResult_ResultIsFalse(string objectSerializerResult)
        {
            // Arrange
            var objectToPersist = new PublishConfiguration();
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            objectSerializer.Setup(o => o.Serialize(It.IsAny<PublishConfiguration>())).Returns(objectSerializerResult);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Save(objectToPersist, storageLocation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Save_PublishConfiguration_WriteSerializedJsonToStoreageLocation_Fails_ResultIsFalse()
        {
            // Arrange
            var objectToPersist = new PublishConfiguration();
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            objectSerializer.Setup(o => o.Serialize(It.IsAny<PublishConfiguration>())).Returns("[]");
            filesystemAccessor.Setup(f => f.WriteContentToFile(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Save(objectToPersist, storageLocation);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Save_PublishConfiguration_WriteSerializedJsonToStoreageLocation_Suceeeds_ResultIsTrue()
        {
            // Arrange
            var objectToPersist = new PublishConfiguration();
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            objectSerializer.Setup(o => o.Serialize(It.IsAny<PublishConfiguration>())).Returns("[]");
            filesystemAccessor.Setup(f => f.WriteContentToFile(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Save(objectToPersist, storageLocation);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region Load<PublishConfiguration>

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Load_PublishConfiguration_StorageLocationIsInvalid(string storageLocation)
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            filesystemPersistence.Load(storageLocation);            
        }

        [Test]
        public void Load_PublishConfiguration_StorageLocationDoesNotExist_ResultIsNull()
        {
            // Arrange
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            filesystemAccessor.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Load(storageLocation);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Load_PublishConfiguration_StorageLocationContentIsEmpty_ResultIsNull(string storageLocationContent)
        {
            // Arrange
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            filesystemAccessor.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);
            filesystemAccessor.Setup(f => f.GetFileContent(It.IsAny<string>())).Returns(storageLocationContent);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Load(storageLocation);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Load_PublishConfiguration_ObjectDeserializerReturnsNull_ResultIsNull()
        {
            // Arrange
            PublishConfiguration objectReturnedByDeserializer = null;
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            filesystemAccessor.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);
            filesystemAccessor.Setup(f => f.GetFileContent(It.IsAny<string>())).Returns("some content");
            objectSerializer.Setup(s => s.Deserialize(It.IsAny<string>())).Returns(objectReturnedByDeserializer);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Load(storageLocation);

            // Assert
            Assert.AreEqual(objectReturnedByDeserializer, result);
        }

        [Test]
        public void Load_PublishConfiguration_ObjectDeserializerReturnsEmptyPublishConfiguration_ResultIsEmptyPublishConfiguration()
        {
            // Arrange
            PublishConfiguration objectReturnedByDeserializer = new PublishConfiguration();
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            filesystemAccessor.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);
            filesystemAccessor.Setup(f => f.GetFileContent(It.IsAny<string>())).Returns("some content");
            objectSerializer.Setup(s => s.Deserialize(It.IsAny<string>())).Returns(objectReturnedByDeserializer);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Load(storageLocation);

            // Assert
            Assert.AreEqual(objectReturnedByDeserializer, result);
        }

        [Test]
        public void Load_PublishConfiguration_ObjectDeserializerReturnsPublishConfiguration_ResultIsPublishConfiguration()
        {
            // Arrange
            PublishConfiguration objectReturnedByDeserializer = new PublishConfiguration { Name = "Test", ApiKey = "Test Api Key", PublishLocation = @"C:\test-location" };
            string storageLocation = "temp-file";

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            var objectSerializer = new Mock<IObjectSerializer<PublishConfiguration>>();

            filesystemAccessor.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);
            filesystemAccessor.Setup(f => f.GetFileContent(It.IsAny<string>())).Returns("some content");
            objectSerializer.Setup(s => s.Deserialize(It.IsAny<string>())).Returns(objectReturnedByDeserializer);

            var filesystemPersistence = new FilesystemPersistence<PublishConfiguration>(filesystemAccessor.Object, objectSerializer.Object);

            // Act
            var result = filesystemPersistence.Load(storageLocation);

            // Assert
            Assert.AreEqual(objectReturnedByDeserializer, result);
        }

        #endregion
    }
}