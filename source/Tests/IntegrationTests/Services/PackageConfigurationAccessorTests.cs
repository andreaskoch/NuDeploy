using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class PackageConfigurationAccessorTests
    {
        #region setup
        
        private IPackageConfigurationAccessor packageConfigurationAccessor;

        [TestFixtureSetUp]
        public void Setup()
        {
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            var encodingProvider = new DefaultFileEncodingProvider();
            var fileSystemAccessor = new PhysicalFilesystemAccessor(encodingProvider);
            this.packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation, fileSystemAccessor);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            File.Delete(PackageConfigurationAccessor.PackageConfigurationFileName);
        }

        #endregion

        #region GetInstalledPackages

        [Test]
        public void GetInstalledPackages_ConfigFileDoesNotExist_ResultIsEmptyList()
        {
            // Act
            var result = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsOneEntry_ResultIsTheSameObjectAsInserted()
        {
            // Arrange
            var package1Id = "ID1";
            var package1Version = "1.0.0";
            var package1 = new PackageInfo { Id = package1Id, Version = package1Version };
            var packages = new[] { package1 };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(package1Id, results.First().Id);
            Assert.AreEqual(package1Version, results.First().Version);
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsTwoIdenticalEntries_ResultContainsOnlyOneEntry()
        {
            // Arrange
            var package1Id = "ID1";
            var package1Version = "1.0.0";
            var package1 = new PackageInfo { Id = package1Id, Version = package1Version };
            var packages = new[] { package1, package1 };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(package1Id, results.First().Id);
            Assert.AreEqual(package1Version, results.First().Version);
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsThreeDistinctEntries_ResultContainsThreeEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(3, results.Count());
            Assert.IsTrue(results.Contains(package1));
            Assert.IsTrue(results.Contains(package2));
            Assert.IsTrue(results.Contains(package3));
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsSusortedEntries_ResultSetIsSortedById()
        {
            // Arrange
            var packageC = new PackageInfo { Id = "C", Version = "1.0.0" };
            var packageB = new PackageInfo { Id = "B", Version = "1.0.0" };
            var packageA = new PackageInfo { Id = "A", Version = "1.0.0" };
            var packageD = new PackageInfo { Id = "D", Version = "1.0.0" };

            var packages = new[] { packageC, packageB, packageA, packageD };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(4, results.Count());
            Assert.IsTrue(results.Skip(0).Take(1).First().Equals(packageA));
            Assert.IsTrue(results.Skip(1).Take(1).First().Equals(packageB));
            Assert.IsTrue(results.Skip(2).Take(1).First().Equals(packageC));
            Assert.IsTrue(results.Skip(3).Take(1).First().Equals(packageD));
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsThreeDistinctEntries_OneEntryIsInvalid_VersionNumberIsEmpty_ResultContainsTwoEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = string.Empty };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.Contains(package1));
            Assert.IsTrue(results.Contains(package2));
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsThreeDistinctEntries_OneEntryIsInvalid_IdIsEmpty_ResultContainsTwoEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = string.Empty, Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.Contains(package1));
            Assert.IsTrue(results.Contains(package2));
        }

        [Test]
        public void GetInstalledPackages_ConfigFileContainsThreeDistinctInvalidEntries_ResultContainsZeroEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = null };
            var package2 = new PackageInfo { Id = "ID2", Version = string.Empty };
            var package3 = new PackageInfo { Id = string.Empty, Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            // Assert
            Assert.AreEqual(0, results.Count());
        }

        #endregion

        #region AddOrUpdate

        [Test]
        public void AddOrUpdate_ConfigFileDoesNotExist_NewPackage_ResultContainsTheNewEntry()
        {
            // Arrange
            var newPackage = new PackageInfo { Id = "ID1", Version = "1.0.0" };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(newPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(newPackage, results.First());
        }

        [Test]
        public void AddOrUpdate_ConfigFileIsEmpty_NewPackage_ResultContainsTheNewEntry()
        {
            // Arrange
            this.CreateConfigFile(new PackageInfo[] { });

            var newPackage = new PackageInfo { Id = "ID1", Version = "1.0.0" };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(newPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(newPackage, results.First());
        }

        [Test]
        public void AddOrUpdate_ConfigFileContainsOneEntry_NewPackage_ResultContainsTwoEntries()
        {
            // Arrange
            var previousPackage = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            this.CreateConfigFile(new[] { previousPackage });

            var newPackage = new PackageInfo { Id = "ID2", Version = "1.0.0" };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(newPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(2, results.Count());
            Assert.AreEqual(previousPackage, results.Skip(0).Take(1).First());
            Assert.AreEqual(newPackage, results.Skip(1).Take(1).First());
        }

        [Test]
        public void AddOrUpdate_ConfigFileContainsOneEntry_NewPackageIsUpdatedVersion_ResultContainsOneEntryWithNewVersionNumber()
        {
            // Arrange
            string packageId = "Sample.Package";
            var package1 = new PackageInfo { Id = packageId, Version = "1.0.0" };
            this.CreateConfigFile(new[] { package1 });

            string newVersionNumber = "1.0.1";
            var updatedPackage = new PackageInfo { Id = packageId, Version = newVersionNumber };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(updatedPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(newVersionNumber, results.First().Version);
        }

        [Test]
        public void AddOrUpdate_ConfigFileContainsOneEntry_NewPackageHasEmptyVersionNumber_ResultContainsOnlyTheOldEntry()
        {
            // Arrange
            string packageId = "Sample.Package";
            var previousPackage = new PackageInfo { Id = packageId, Version = "1.0.0" };
            this.CreateConfigFile(new[] { previousPackage });

            string newVersionNumber = string.Empty;
            var updatedPackage = new PackageInfo { Id = packageId, Version = newVersionNumber };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(updatedPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(previousPackage, results.First());
        }

        [Test]
        public void AddOrUpdate_ConfigFileContainsOneEntry_NewPackageHasVersionNumberThatIsNull_ResultContainsOnlyTheOldEntry()
        {
            // Arrange
            string packageId = "Sample.Package";
            var previousPackage = new PackageInfo { Id = packageId, Version = "1.0.0" };
            this.CreateConfigFile(new[] { previousPackage });

            string newVersionNumber = null;
            var updatedPackage = new PackageInfo { Id = packageId, Version = newVersionNumber };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(updatedPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(previousPackage, results.First());
        }

        [Test]
        public void AddOrUpdate_ConfigFileContainsOneEntry_NewPackageHasVersionNumberThatIsWhitespace_ResultContainsOnlyTheOldEntry()
        {
            // Arrange
            string packageId = "Sample.Package";
            var previousPackage = new PackageInfo { Id = packageId, Version = "1.0.0" };
            this.CreateConfigFile(new[] { previousPackage });

            string newVersionNumber = " ";
            var updatedPackage = new PackageInfo { Id = packageId, Version = newVersionNumber };

            // Act
            this.packageConfigurationAccessor.AddOrUpdate(updatedPackage);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(previousPackage, results.First());
        }

        #endregion

        #region Remove

        [Test]
        public void Remove_ConfigFileContainsThreeEntries_LastEntryIsRemoved_ResultContainsOnlyTwoEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            this.packageConfigurationAccessor.Remove(package3.Id);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(2, results.Count());
            Assert.AreEqual(package1, results.Skip(0).Take(1).First());
            Assert.AreEqual(package2, results.Skip(1).Take(1).First());
        }

        [Test]
        public void Remove_ConfigFileContainsThreeEntries_SecondEntryIsRemoved_ResultContainsOnlyTwoEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            this.packageConfigurationAccessor.Remove(package2.Id);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(2, results.Count());
            Assert.AreEqual(package1, results.Skip(0).Take(1).First());
            Assert.AreEqual(package3, results.Skip(1).Take(1).First());
        }

        [Test]
        public void Remove_ConfigFileContainsThreeEntries_FirstEntryIsRemoved_ResultContainsOnlyTwoEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            this.packageConfigurationAccessor.Remove(package1.Id);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(2, results.Count());
            Assert.AreEqual(package2, results.Skip(0).Take(1).First());
            Assert.AreEqual(package3, results.Skip(1).Take(1).First());
        }

        [Test]
        public void Remove_ConfigFileContainsThreeEntries_AllThreeEntriesGetRemoved_ResultContainsZeroEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            this.packageConfigurationAccessor.Remove(package1.Id);
            this.packageConfigurationAccessor.Remove(package2.Id);
            this.packageConfigurationAccessor.Remove(package3.Id);

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public void Remove_ConfigFileContainsThreeEntries_NonExistingEntryIsRemoved_ResultContainsAllThreeEntries()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "ID1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "ID2", Version = "1.0.0" };
            var package3 = new PackageInfo { Id = "ID3", Version = "1.0.0" };

            var packages = new[] { package1, package2, package3 };

            this.CreateConfigFile(packages);

            // Act
            this.packageConfigurationAccessor.Remove("Some non existing package id");

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(3, results.Count());
            Assert.AreEqual(package1, results.Skip(0).Take(1).First());
            Assert.AreEqual(package2, results.Skip(1).Take(1).First());
            Assert.AreEqual(package3, results.Skip(2).Take(1).First());
        }

        [Test]
        public void Remove_ConfigFileDoesNotExist_NonExistingEntryIsRemoved_ResultContainsZeroEntries()
        {
            // Act
            this.packageConfigurationAccessor.Remove("Some non existing package id");

            // Assert
            var results = this.packageConfigurationAccessor.GetInstalledPackages();

            Assert.AreEqual(0, results.Count());
        }

        #endregion

        #region help methods

        private void CreateConfigFile(PackageInfo[] packageInfos)
        {
            string json = JsonConvert.SerializeObject(packageInfos);
            File.WriteAllText(PackageConfigurationAccessor.PackageConfigurationFileName, json, Encoding.UTF8);
        }

        #endregion
    }
}