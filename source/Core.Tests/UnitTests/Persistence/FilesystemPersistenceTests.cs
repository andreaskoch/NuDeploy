using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Persistence;
using NuDeploy.Core.Common.Serialization;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Persistence
{
    [TestFixture]
    public class FilesystemPersistenceTests
    {
        #region constructor

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
    }
}