using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Services
{
    [TestFixture]
    public class PackageConfigurationFileReaderTests
    {
        private IPackageConfigurationFileReader packageConfigurationFileReader;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.packageConfigurationFileReader = new PackageConfigurationFileReader();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetInstalledPackages_ConfigugrationFilePathIsEmpty_ArgumentExceptionIsThrown()
        {
            // Arrange
            string configurationFilePath = string.Empty;

            // Act
            this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetInstalledPackages_ConfigugrationFilePathIsNull_ArgumentExceptionIsThrown()
        {
            // Arrange
            string configurationFilePath = null;

            // Act
            this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetInstalledPackages_ConfigugrationFilePathIsWhitespace_ArgumentExceptionIsThrown()
        {
            // Arrange
            string configurationFilePath = " ";

            // Act
            this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath);
        }

        [Test]
        public void GetInstalledPackages_ConfigugrationFilePathDoesNotExist_EmptyListIsReturned()
        {
            // Arrange
            string configurationFilePath = this.GetConfigurationFilePath("Test-Packages-Does-Not-Exist.txt");

            // Act
            var result = this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetInstalledPackages_ConfigContainsFiveEntries_FiveentriesAreReturned()
        {
            // Arrange
            string configurationFilePath = this.GetConfigurationFilePath("Test-Packages-1-Five-Entries.txt");

            // Act
            IList<PackageInfo> result = this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath).ToList();

            // Assert
            Assert.AreEqual(5, result.Count);
        }

        [Test]
        public void GetInstalledPackages_ConfigContainsNoEntries_EmptyListIsReturned()
        {
            // Arrange
            string configurationFilePath = this.GetConfigurationFilePath("Test-Packages-2-Zero-Entries.txt");

            // Act
            IList<PackageInfo> result = this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [ExpectedException(typeof(XmlException))]
        public void GetInstalledPackages_ConfigContainsInvalidXml_XmlExceptionIsThrown()
        {
            // Arrange
            string configurationFilePath = this.GetConfigurationFilePath("Test-Packages-3-InvalidXml.txt");

            // Act
            this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath);
        }

        [Test]
        [ExpectedException(typeof(XmlException))]
        public void GetInstalledPackages_ConfigFileIsEmpty_XmlExceptionIsThrown()
        {
            // Arrange
            string configurationFilePath = this.GetConfigurationFilePath("Test-Packages-4-Empty.txt");

            // Act
            this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath);
        }

        [Test]
        public void GetInstalledPackages_ConfigContainsEntryWithInvalidVersionNumberFormat_EmptyListIsReturned()
        {
            // Arrange
            string configurationFilePath = this.GetConfigurationFilePath("Test-Packages-5-Invalid-Version-Number-Format.txt");

            // Act
            var exception = Assert.Throws<ArgumentException>(() => this.packageConfigurationFileReader.GetInstalledPackages(configurationFilePath).ToList());

            // Assert
            Assert.That(exception.Message.Contains("1.0.a"));
        }

        #region utility methods
        
        private string GetConfigurationFilePath(string filename)
        {
            return Path.Combine(Environment.CurrentDirectory, "IntegrationTests\\Services", filename);
        }

        #endregion
    }
}