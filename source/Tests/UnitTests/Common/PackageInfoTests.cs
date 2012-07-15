using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;

using NuDeploy.Core.Common;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Common
{
    [TestFixture]
    public class PackageInfoTests
    {
        #region IsValid

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("Package", null)]
        [TestCase(null, "1.0.0")]
        [TestCase("Package", "")]
        [TestCase("", "1.0.0")]
        [TestCase("Package", " ")]
        [TestCase(" ", "1.0.0")]
        public void IsValid_IdAndVersionPropertyAreNotSet_ResultIsFalse(string id, string version)
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = id, Version = version };

            // Act
            bool result = packageInfo.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase("Package", "1.0.0")]
        public void IsValid_IdAndVersionPropertyAreSet_ResultIsTrue(string id, string version)
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = id, Version = version };

            // Act
            bool result = packageInfo.IsValid;

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region Serialization

        [Test]
        public void IsSerializable()
        {
            // Arrange
            var package = new PackageInfo { Id = "Package", Version = "1.0.0" };

            // Act
            string json = JsonConvert.SerializeObject(package);

            // Assert
            Assert.IsTrue(json.Contains(package.Id));
            Assert.IsTrue(json.Contains(package.Version));
        }

        [Test]
        public void CanBeDeserialized()
        {
            // Arrange
            var package = new PackageInfo { Id = "Package", Version = "1.0.0" };

            // Act
            string json = JsonConvert.SerializeObject(package);
            var deserializedpackage = JsonConvert.DeserializeObject<PackageInfo>(json);

            // Assert
            Assert.AreEqual(package.Id, deserializedpackage.Id);
            Assert.AreEqual(package.Version, deserializedpackage.Version);
        }

        [Test]
        public void Serialization_IsValidPropertyIsNotSerialized()
        {
            // Arrange
            var package = new PackageInfo { Id = "Package", Version = "1.0.0" };

            // Act
            string json = JsonConvert.SerializeObject(package);

            // Assert
            Assert.IsFalse(json.Contains("IsValid"));
            Assert.IsFalse(json.Contains(package.IsValid.ToString(CultureInfo.InvariantCulture)));
        }

        #endregion

        #region ToString

        [Test]
        public void ToString_ContainsId()
        {
            // Arrange
            var package = new PackageInfo { Id = "Package", Version = "1.0.0" };

            // Act
            string result = package.ToString();

            // Assert
            Assert.IsTrue(result.Contains(package.Id));
        }

        [Test]
        public void ToString_ContainsVersion()
        {
            // Arrange
            var package = new PackageInfo { Id = "Package", Version = "1.0.0" };

            // Act
            string result = package.ToString();

            // Assert
            Assert.IsTrue(result.Contains(package.Version));
        }

        [Test]
        public void ToString_PropertiesAreNotSet_ResultIsTypeName()
        {
            // Arrange
            var packageInfo = new PackageInfo();

            // Act
            string result = packageInfo.ToString();

            // Assert
            Assert.AreEqual(typeof(PackageInfo).Name, result);
        }

        [Test]
        public void ToString_IdIsSet_VersionIsNotSet_ResultContainsId()
        {
            // Arrange
            var packageInfo = new PackageInfo { Id = "Package" };

            // Act
            string result = packageInfo.ToString();

            // Assert
            Assert.IsTrue(result.Contains(packageInfo.Id));
        }

        [Test]
        public void ToString_IdIsNotSet_VersionIsSet_ResultIsTypeName()
        {
            // Arrange
            var packageInfo = new PackageInfo { Version = "1.0.0" };

            // Act
            string result = packageInfo.ToString();

            // Assert
            Assert.AreEqual(typeof(PackageInfo).Name, result);
        }

        #endregion

        #region Equals

        [Test]
        public void Equals_TwoIdenticalPackages_ResultIsTrue()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "Package1", Version = "1.0.0" };

            // Act
            bool result = package1.Equals(package2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalPackages_WithDifferentNameCasing_ResultIsTrue()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "package1", Version = "1.0.0" };

            // Act
            bool result = package1.Equals(package2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_TwoIdenticalConfigurations_WithDifferentWhitespaces_ResultIsFalse()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1 ", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = " Package1", Version = "1.0.0" };

            // Act
            bool result = package1.Equals(package2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNull_ResultIsFalse()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1 ", Version = "1.0.0" };
            PackageInfo package2 = null;

            // Act
            bool result = package1.Equals(package2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNotInitialized_ResultIsFalse()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1 ", Version = "1.0.0" };
            var package2 = new PackageInfo();

            // Act
            bool result = package1.Equals(package2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsOfOtherType_ResultIsFalse()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1 ", Version = "1.0.0" };
            object package2 = new object();

            // Act
            bool result = package1.Equals(package2);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetHashCode

        [Test]
        public void GetHashCode_TwoIdenticalObjects_BothInitialized_HashCodesAreEqual()
        {
            // Arrange
            var package1 = new PackageInfo { Id = "Package1", Version = "1.0.0" };
            var package2 = new PackageInfo { Id = "Package1", Version = "1.0.0" };

            // Act
            int hashCodeObject1 = package1.GetHashCode();
            int hashCodeObject2 = package2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_TwoIdenticalObjects_BothUninitialized_HashCodesAreEqual()
        {
            // Arrange
            var package1 = new PackageInfo();
            var package2 = new PackageInfo();

            // Act
            int hashCodeObject1 = package1.GetHashCode();
            int hashCodeObject2 = package2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_SameHashCodeIsReturnedEveryTimeTheMethodIsCalled_AsLongAsTheObjectDoesNotChange()
        {
            // Arrange
            var packageId = "Package";
            var packageVersion = "1.0.0";
            var package = new PackageInfo { Id = packageId, Version = packageVersion };

            int expectedHashcode = package.GetHashCode();

            for (var i = 0; i < 100; i++)
            {
                // Act
                package.Id = packageId;
                package.Version = packageVersion;
                int generatedHashCode = package.GetHashCode();

                // Assert
                Assert.AreEqual(expectedHashcode, generatedHashCode);
            }
        }

        [Test]
        public void GetHashCode_ForAllUniqueObject_AUniqueHashCodeIsReturned()
        {
            var hashCodes = new Dictionary<int, PackageInfo>();

            for (var i = 0; i < 10000; i++)
            {
                // Act
                var packageInfo = new PackageInfo { Id = Guid.NewGuid().ToString(), Version = Guid.NewGuid().ToString() };

                int generatedHashCode = packageInfo.GetHashCode();

                // Assert
                Assert.IsFalse(hashCodes.ContainsKey(generatedHashCode));
                hashCodes.Add(generatedHashCode, packageInfo);
            }
        }

        #endregion
    }
}