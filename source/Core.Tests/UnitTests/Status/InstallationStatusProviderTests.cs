using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Installation;

using NUnit.Framework;

using NuDeploy.Core.Services.Installation.Status;

namespace NuDeploy.Tests.UnitTests.Status
{
    [TestFixture]
    public class InstallationStatusProviderTests
    {
        #region Constructor
        
        [Test]
        public void Constructor_AllParametersAreValid_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IPackageConfigurationAccessor packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>().Object;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            var result = new InstallationStatusProvider(applicationInformation, packageConfigurationAccessor, filesystemAccessor);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ApplicationInformation applicationInformation = null;
            IPackageConfigurationAccessor packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>().Object;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new InstallationStatusProvider(applicationInformation, packageConfigurationAccessor, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageConfigurationAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IPackageConfigurationAccessor packageConfigurationAccessor = null;
            IFilesystemAccessor filesystemAccessor = new Mock<IFilesystemAccessor>().Object;

            // Act
            new InstallationStatusProvider(applicationInformation, packageConfigurationAccessor, filesystemAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_FilesystemAccessorParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            IPackageConfigurationAccessor packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>().Object;
            IFilesystemAccessor filesystemAccessor = null;

            // Act
            new InstallationStatusProvider(applicationInformation, packageConfigurationAccessor, filesystemAccessor);
        }

        #endregion

        #region GetPackageInfo

        [Test]
        public void GetPackageInfo_PackageConfigurationAccessorReturnNoResult_NoResultsAreReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
                {
                    StartupFolder = Environment.CurrentDirectory
                };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(new List<PackageInfo>());

            var filesystemAccessor = new Mock<IFilesystemAccessor>();

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetPackageInfo_PackageConfigurationAccessorReturnsAListOfPackages_PackagesDontHaveAnyDirectories_NoResultsAreReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package1", Version = "1.0.0" };
            var installedPackage2 = new PackageInfo { Id = "Package2", Version = "1.0.1" };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(new List<PackageInfo>
                {
                    installedPackage1,
                    installedPackage2
                });

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(new List<DirectoryInfo>());

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetPackageInfo_PackagesDirectoriesExist_ButThePackageConfigurationAccessorReturnsNoResults_EmptyListIsReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(new List<PackageInfo>());

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package1.1.0.0"));
            var packageDirectory2 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package2.1.0.1"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1, packageDirectory2 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetPackageInfo_PackageConfigurationAccessorReturnsAListOfTwoPackages_AllPackagesHaveDirectories_ListWithTwoEntriesIsReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package1", Version = "1.0.0" };
            var installedPackage2 = new PackageInfo { Id = "Package2", Version = "1.0.1" };
            var installedPackages = new List<PackageInfo> { installedPackage1, installedPackage2 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(
                installedPackages.Select(p => new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, string.Format("{0}.{1}", p.Id, p.Version)))));

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo();

            // Assert
            Assert.AreEqual(installedPackages.Count, result.Count());
        }

        [Test]
        public void GetPackageInfo_PackageConfigurationAccessorReturnsOnePackage_PackageHasDirectoriesForMultipleVersions_OneEntryForEachVersionIsReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package", Version = "4.0.0" };
            var installedPackages = new List<PackageInfo> { installedPackage1 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package.1.0.0"));
            var packageDirectory2 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package.2.0.0"));
            var packageDirectory3 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package.3.0.0"));
            var packageDirectory4 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package.4.0.0"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1, packageDirectory2, packageDirectory3, packageDirectory4 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo();

            // Assert
            Assert.AreEqual(packageDirectories.Count, result.Count());
            Assert.AreEqual(1, result.Count(p => p.IsInstalled));
            Assert.AreEqual(packageDirectories.Count - 1, result.Count(p => p.IsInstalled == false));
        }

        [Test]
        public void GetPackageInfo_PackageConfigurationAccessorReturnsOnePackage_PackageHasDirectory_OneItemIsReturned_ResultHasAllPropertiesSet()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package", Version = "1.0.0" };
            var installedPackages = new List<PackageInfo> { installedPackage1 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package.1.0.0"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo().First();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(installedPackage1.Id, result.Id);
            Assert.AreEqual(installedPackage1.Version, result.Version.ToString());
            Assert.AreEqual(packageDirectory1.FullName, result.Folder);
            Assert.IsTrue(result.IsInstalled);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetPackageInfo_PackageIdIsInvalid_EmptyListIsReturned(string packageId)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();

            var installedPackages = new List<PackageInfo>();
            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectories = new List<DirectoryInfo>();
            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo(packageId);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetPackageInfo_PackageIdIsUnknown_EmptyListIsReturned()
        {
            // Arrange
            string packageId = "UnknownPackage";

            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package-A", Version = "1.0.0" };
            var installedPackage2 = new PackageInfo { Id = "Package-B", Version = "1.0.0" };
            var installedPackage3 = new PackageInfo { Id = "Package-C", Version = "1.0.0" };
            var installedPackages = new List<PackageInfo> { installedPackage1, installedPackage2, installedPackage3 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.1.0.0"));
            var packageDirectory2 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-B.1.0.0"));
            var packageDirectory3 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-C.1.0.0"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1, packageDirectory2, packageDirectory3 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo(packageId);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestCase("Package-A ")]
        [TestCase(" Package-A")]
        public void GetPackageInfo_PackageIdIsKnownButContainsInvalidWhitespace_EmptyListIsReturned(string packageId)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package-A", Version = "1.0.0" };
            var installedPackages = new List<PackageInfo> { installedPackage1 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.1.0.0"));
            var packageDirectory2 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.2.0.0"));
            var packageDirectory3 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.3.0.0"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1, packageDirectory2, packageDirectory3 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo(packageId);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestCase("Package-A")]
        [TestCase("Package-a")]
        [TestCase("package-a")]
        [TestCase("PACKAGE-A")]
        public void GetPackageInfo_PackageIdKnown_PackageIsInstalled_ListWithOneItemIsReturned(string packageId)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package-A", Version = "1.0.0" };
            var installedPackage2 = new PackageInfo { Id = "Package-B", Version = "1.0.0" };
            var installedPackage3 = new PackageInfo { Id = "Package-C", Version = "1.0.0" };
            var installedPackages = new List<PackageInfo> { installedPackage1, installedPackage2, installedPackage3 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.1.0.0"));
            var packageDirectory2 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-B.1.0.0"));
            var packageDirectory3 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-C.1.0.0"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1, packageDirectory2, packageDirectory3 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo(packageId).First();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(installedPackage1.Id, result.Id);
            Assert.AreEqual(installedPackage1.Version, result.Version.ToString());
            Assert.AreEqual(packageDirectory1.FullName, result.Folder);
            Assert.IsTrue(result.IsInstalled);
        }

        [TestCase("Package-A")]
        [TestCase("Package-a")]
        [TestCase("package-a")]
        [TestCase("PACKAGE-A")]
        public void GetPackageInfo_PackageIdKnown_PackageIsInstalledInThreeVersions_ListWithThreeItemsIsReturned(string packageId)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation
            {
                StartupFolder = Environment.CurrentDirectory
            };

            var installedPackage1 = new PackageInfo { Id = "Package-A", Version = "1.0.0" };
            var installedPackages = new List<PackageInfo> { installedPackage1 };

            var packageConfigurationAccessor = new Mock<IPackageConfigurationAccessor>();
            packageConfigurationAccessor.Setup(p => p.GetInstalledPackages()).Returns(installedPackages);

            var packageDirectory1 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.1.0.0"));
            var packageDirectory2 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.2.0.0"));
            var packageDirectory3 = new DirectoryInfo(Path.Combine(applicationInformation.StartupFolder, "Package-A.3.0.0"));
            var packageDirectories = new List<DirectoryInfo> { packageDirectory1, packageDirectory2, packageDirectory3 };

            var filesystemAccessor = new Mock<IFilesystemAccessor>();
            filesystemAccessor.Setup(f => f.GetSubDirectories(applicationInformation.StartupFolder)).Returns(packageDirectories);

            var installationStatusProviderresult = new InstallationStatusProvider(
                applicationInformation, packageConfigurationAccessor.Object, filesystemAccessor.Object);

            // Act
            var result = installationStatusProviderresult.GetPackageInfo(packageId);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(1, result.Count(p => p.IsInstalled));
            Assert.AreEqual(result.Count() - 1, result.Count(p => p.IsInstalled == false));
        }

        #endregion
    }
}