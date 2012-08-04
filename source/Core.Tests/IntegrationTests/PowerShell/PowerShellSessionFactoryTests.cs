using Moq;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.PowerShell;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.PowerShell
{
    [TestFixture]
    public class PowerShellSessionFactoryTests
    {
        #region GetSession

        [Test]
        public void GetSession_ResultIsNotNull()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var powerShellUserInterface = new NuDeployPowerShellUserInterface(userInterface.Object);
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            var powerShellHost = new PowerShellHost(powerShellUserInterface, applicationInformation);
            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var powerShellSessionFactory = new PowerShellSessionFactory(powerShellHost, filesystemAccessor.Object);

            // Act
            IPowerShellSession result = powerShellSessionFactory.GetSession();

            // Assert
            Assert.IsNotNull(result);
        }

        #endregion
    }
}