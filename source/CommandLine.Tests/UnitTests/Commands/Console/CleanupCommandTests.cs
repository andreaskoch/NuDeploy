using System;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Cleanup;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class CleanupCommandTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var cleanupService = new Mock<ICleanupService>();

            // Act
            var cleanupCommand = new CleanupCommand(userInterface.Object, cleanupService.Object);

            // Assert
            Assert.IsNotNull(cleanupCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var cleanupService = new Mock<ICleanupService>();

            // Act
            var cleanupCommand = new CleanupCommand(userInterface.Object, cleanupService.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(cleanupCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var cleanupService = new Mock<ICleanupService>();

            // Act
            new CleanupCommand(null, cleanupService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CleanupServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();

            // Act
            new CleanupCommand(userInterface.Object, null);
        }

        #endregion

        #region Execute

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_NoValidPackageIdIsSupplied_GenealCleanupIsCalled(string packageId)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var cleanupService = new Mock<ICleanupService>();
            cleanupService.Setup(c => c.Cleanup()).Returns(new SuccessResult());

            var cleanupCommand = new CleanupCommand(userInterface.Object, cleanupService.Object);

            cleanupCommand.Arguments.Add(CleanupCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            cleanupCommand.Execute();

            // Assert
            cleanupService.Verify(c => c.Cleanup(), Times.Once());
        }

        [Test]
        public void Execute_PackageIdIsSupplied_PackageSpecificCleanupIsCalled()
        {
            // Arrange
            string packageId = "Package.A";

            var userInterface = new Mock<IUserInterface>();
            var cleanupService = new Mock<ICleanupService>();
            cleanupService.Setup(c => c.Cleanup(It.IsAny<string>())).Returns(new SuccessResult());

            var cleanupCommand = new CleanupCommand(userInterface.Object, cleanupService.Object);

            cleanupCommand.Arguments.Add(CleanupCommand.ArgumentNameNugetPackageId, packageId);

            // Act
            cleanupCommand.Execute();

            // Assert
            cleanupService.Verify(c => c.Cleanup(packageId), Times.Once());
        }

        #endregion
    }
}