using System;

using Moq;

using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Installation.PowerShell;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Installation
{
    [TestFixture]
    public class PowerShellExecutorTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var powerShellSessionFactory = new Mock<IPowerShellSessionFactory>();

            // Act
            var powerShellExecutor = new PowerShellExecutor(powerShellSessionFactory.Object);

            // Assert
            Assert.IsNotNull(powerShellExecutor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PowerShellSessionFactoryParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Act
            new PowerShellExecutor(null);
        }

        #endregion

        #region ExecuteScript

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ExecuteScript_ScriptPathIsInvalid_ResultIsFalse(string scriptPath)
        {
            // Arrange
            var powerShellSessionFactory = new Mock<IPowerShellSessionFactory>();
            var powerShellExecutor = new PowerShellExecutor(powerShellSessionFactory.Object);

            // Act
            var result = powerShellExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }
        
        [Test]
        public void ExecuteScript_ScriptPathIsSet_PowerShellSessionFactoryReturnsNull_ResultIsFalse()
        {
            // Arrange
            string scriptPath = "some-script.ps1";

            var powerShellSessionFactory = new Mock<IPowerShellSessionFactory>();
            var powerShellExecutor = new PowerShellExecutor(powerShellSessionFactory.Object);

            IPowerShellSession powerShellSession = null;
            powerShellSessionFactory.Setup(s => s.GetSession()).Returns(powerShellSession);

            // Act
            var result = powerShellExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }

        [Test]
        public void ExecuteScript_ScriptPathIsSet_PowerShellSessionThrowsException_ResultIsFalse()
        {
            // Arrange
            string scriptPath = "some-script.ps1";

            var powerShellSessionFactory = new Mock<IPowerShellSessionFactory>();
            var powerShellExecutor = new PowerShellExecutor(powerShellSessionFactory.Object);

            var powerShellSessionMock = new Mock<IPowerShellSession>();
            powerShellSessionMock.Setup(p => p.ExecuteScript(It.IsAny<string>())).Throws(new Exception());

            powerShellSessionFactory.Setup(s => s.GetSession()).Returns(powerShellSessionMock.Object);

            // Act
            var result = powerShellExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Failure);
        }

        [Test]
        public void ExecuteScript_ScriptPathIsSet_PowerShellSessionExecutesScript_ResultIsTrue()
        {
            // Arrange
            string scriptPath = "some-script.ps1";

            var powerShellSessionFactory = new Mock<IPowerShellSessionFactory>();
            var powerShellExecutor = new PowerShellExecutor(powerShellSessionFactory.Object);

            var powerShellSessionMock = new Mock<IPowerShellSession>();
            powerShellSessionFactory.Setup(s => s.GetSession()).Returns(powerShellSessionMock.Object);

            // Act
            var result = powerShellExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual(result.Status, ServiceResultType.Success);
        }

        #endregion
    }
}