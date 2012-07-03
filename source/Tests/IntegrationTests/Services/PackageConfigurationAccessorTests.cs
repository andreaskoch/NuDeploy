using System;
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
        private IPackageConfigurationAccessor packageConfigurationAccessor;

        [TestFixtureSetUp]
        public void Setup()
        {
            var applicationInformation = new ApplicationInformation
                {
                    ApplicationName = "NuDeploy.Tests",
                    ApplicationVersion = new Version(1, 0),
                    NameOfExecutable = "NuDeploy.Tests.exe",
                    StartupFolder = Environment.CurrentDirectory
                };

            this.packageConfigurationAccessor = new PackageConfigurationAccessor(applicationInformation);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            File.Delete(PackageConfigurationAccessor.PackageConfigurationFileName);
        }

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

        private void CreateConfigFile(PackageInfo[] packageInfos)
        {
            string json = JsonConvert.SerializeObject(packageInfos);
            File.WriteAllText(PackageConfigurationAccessor.PackageConfigurationFileName, json, Encoding.UTF8);
        }
    }
}