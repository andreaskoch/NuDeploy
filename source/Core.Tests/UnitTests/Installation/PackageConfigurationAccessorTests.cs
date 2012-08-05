using System;
using System.Linq;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;
using NuDeploy.Core.Services.Installation;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Installation
{
    [TestFixture]
    public class PackageConfigurationAccessorTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            // Act
            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Assert
            Assert.IsNotNull(packageConfigurationAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            // Act
            new PackageConfigurationAccessor(null, packageInfoFilesystemPersistence.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemPersistenceParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };

            // Act
            new PackageConfigurationAccessor(applicationInformation, null);
        }

        #endregion

        #region GetInstalledPackages

        [Test]
        public void GetInstalledPackages_ListIsEmpty_ResultIsEmptyList()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            PackageInfo[] packages = null;
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetInstalledPackages_ListContainsOnlyOneInvalidPackage_ResultIsEmptyList()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo()
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetInstalledPackages_ListContainsTwoIdenticalPackages_ResultContainsOnlyOneEntry()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" },
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void GetInstalledPackages_ListContainsTwoPackages_ResultContainsTwoEntries()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" },
                    new PackageInfo { Id = "Package.B", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void GetInstalledPackages_ListContainsUnsortedPackages_ResultIsSorted()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Z", Version = "1.0.0.0" },
                    new PackageInfo { Id = "J", Version = "1.0.0.0" },
                    new PackageInfo { Id = "G", Version = "1.0.0.0" },
                    new PackageInfo { Id = "C", Version = "1.0.0.0" },
                    new PackageInfo { Id = "B", Version = "1.0.0.0" },
                    new PackageInfo { Id = "A", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual("A", result.First().Id);
            Assert.AreEqual("Z", result.Last().Id);
        }

        #endregion

        #region AddOrUpdate

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdate_PackageInfoParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            PackageInfo packageInfo = null;

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.AddOrUpdate(packageInfo);
        }

        [Test]
        public void AddOrUpdate_PackageInfoIsInvalid_ResultIsFalse()
        {
            // Arrange
            var packageInfo = new PackageInfo();

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.AddOrUpdate(packageInfo);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AddOrUpdate_PackageListIsEmpty_AddNewPackage_OnePackageIsSaved()
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = "Package.A", Version = "1.0.0.0" };

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new PackageInfo[] { };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.AddOrUpdate(packageInfo);

            // Assert
            packageInfoFilesystemPersistence.Verify(
                p => p.Save(It.Is<PackageInfo[]>(packageList => packageList.Count() == 1 && packageList.Contains(packageInfo)), It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void AddOrUpdate_PackageListIsNotEmpty_AddNewPackage_NewPackageIsAdded()
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = "Package.A", Version = "1.0.0.0" };

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.B", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.AddOrUpdate(packageInfo);

            // Assert
            packageInfoFilesystemPersistence.Verify(
                p => p.Save(It.Is<PackageInfo[]>(packageList => packageList.Count() == 2 && packageList.Contains(packageInfo)), It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void AddOrUpdate_PackageListIsNotEmpty_AddNewPackage_NewPackageListIsSorted()
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = "A", Version = "1.0.0.0" };

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Z", Version = "1.0.0.0" },
                    new PackageInfo { Id = "B", Version = "1.0.0.0" },
                    new PackageInfo { Id = "C", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.AddOrUpdate(packageInfo);

            // Assert
            packageInfoFilesystemPersistence.Verify(
                p => p.Save(It.Is<PackageInfo[]>(packageList => packageList.First().Id == "A" && packageList.Last().Id == "Z"), It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void AddOrUpdate_PackageListIsNotEmpty_UpdatePackage_ExistingPackageIsUpdated()
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = "Package.A", Version = "1.0.0.1" };

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.AddOrUpdate(packageInfo);

            // Assert
            packageInfoFilesystemPersistence.Verify(
                p =>
                p.Save(
                    It.Is<PackageInfo[]>(packageList => packageList.Count() == 1 && packageList.First().Version.Equals(packageInfo.Version)), It.IsAny<string>()),
                Times.Once());
        }

        #endregion

        #region Remove

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Remove_PackageIdIsInvalid_ArgumentExceptionIsThrown(string packageId)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.Remove(packageId);
        }

        [Test]
        public void Remove_PackageListIsEmpty_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new PackageInfo[] { };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.Remove(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Remove_PackageListDoesNotContainSuppliedPackageId_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.B", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.Remove(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Remove_PackageListContainsSuppliedPackageId_EmptyListIsSaved()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            packageConfigurationAccessor.Remove(packageId);

            // Assert
            packageInfoFilesystemPersistence.Verify(p => p.Save(It.Is<PackageInfo[]>(packageList => !packageList.Any()), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Remove_PackageListContainsSuppliedPackageId_SaveFails_ResultIsFalse()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            packageInfoFilesystemPersistence.Setup(p => p.Save(It.IsAny<PackageInfo[]>(), It.IsAny<string>())).Returns(false);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.Remove(packageId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Remove_PackageListContainsSuppliedPackageId_SaveSucceeds_ResultIsTrue()
        {
            // Arrange
            string packageId = "Package.A";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var packageInfoFilesystemPersistence = new Mock<IFilesystemPersistence<PackageInfo[]>>();

            var packages = new[]
                {
                    new PackageInfo { Id = "Package.A", Version = "1.0.0.0" }
                };
            packageInfoFilesystemPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(packages);

            packageInfoFilesystemPersistence.Setup(p => p.Save(It.IsAny<PackageInfo[]>(), It.IsAny<string>())).Returns(true);

            var packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, packageInfoFilesystemPersistence.Object);

            // Act
            var result = packageConfigurationAccessor.Remove(packageId);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}