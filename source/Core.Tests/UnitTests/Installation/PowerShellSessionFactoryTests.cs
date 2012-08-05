using System;

using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Services.Installation.PowerShell;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Installation
{
    [TestFixture]
    public class PowerShellSessionFactoryTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var powerShellHost = new Mock<IPowerShellHost>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            var powerShellSessionFactory = new PowerShellSessionFactory(powerShellHost.Object, filesystemAccessor.Object);

            // Assert
            Assert.IsNotNull(powerShellSessionFactory);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PowerShellHostParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            // Act
            new PowerShellSessionFactory(null, filesystemAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var powerShellHost = new Mock<IPowerShellHost>();

            // Act
            new PowerShellSessionFactory(powerShellHost.Object, null);
        }

        #endregion
    }
}