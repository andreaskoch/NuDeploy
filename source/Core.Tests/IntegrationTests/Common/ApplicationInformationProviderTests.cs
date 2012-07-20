using System.IO;

using NUnit.Framework;

using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Tests.IntegrationTests.Common
{
    [TestFixture]
    public class ApplicationInformationProviderTests
    {
        [Test]
        public void GetApplicationInformation_ApplicationNameIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.ApplicationName);
        }

        [Test]
        public void GetApplicationInformation_NameOfExecutableIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.NameOfExecutable);
        }

        [Test]
        public void GetApplicationInformation_ApplicationVersionIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNull(result.ApplicationVersion);
        }

        [Test]
        public void GetApplicationInformation_StartupFolderIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.StartupFolder);
        }

        [Test]
        public void GetApplicationInformation_StartupFolderIsExistingDirectory()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsTrue(Directory.Exists(result.StartupFolder));
        }

        [Test]
        public void GetApplicationInformation_ConfigurationFileFolderIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.ConfigurationFileFolder);
        }

        [Test]
        public void GetApplicationInformation_ConfigurationFileFolderIsExistingDirectory()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsTrue(Directory.Exists(result.ConfigurationFileFolder));
        }

        [Test]
        public void GetApplicationInformation_LogFolderIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.LogFolder);
        }

        [Test]
        public void GetApplicationInformation_BuildFolderIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.BuildFolder);
        }

        [Test]
        public void GetApplicationInformation_PrePackagingFolderIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.PrePackagingFolder);
        }

        [Test]
        public void GetApplicationInformation_PackagingFolderIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.PackagingFolder);
        }

        [Test]
        public void GetApplicationInformation_ExecutingUserIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNull(result.ExecutingUser);
        }

        [Test]
        public void GetApplicationInformation_MachineNameIsSet()
        {
            // Act
            var result = ApplicationInformationProvider.GetApplicationInformation();

            // Assert
            Assert.IsNotNullOrEmpty(result.MachineName);
        }
    }
}
