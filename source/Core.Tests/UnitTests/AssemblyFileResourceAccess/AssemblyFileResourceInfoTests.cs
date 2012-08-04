using System;
using System.Collections.Generic;

using NuDeploy.Core.Services.AssemblyResourceAccess;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.AssemblyFileResourceAccess
{
    [TestFixture]
    public class AssemblyFileResourceInfoTests
    {
        #region constructor
        
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("Resource Name", "some/path/file.txt")]
        [TestCase("Resource Name", null)]
        [TestCase(null, "some/path/file.txt")]
        [TestCase("Lorem ", "Ipsum ")]
        [TestCase(" Lorem", " Ipsum")]
        public void Constructor_PropertiesAreSetAsTheyArePassedIntoTheConstructor(string resourceName, string resourcePath)
        {
            // Act
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo(resourceName, resourcePath);

            // Assert
            Assert.AreEqual(resourceName, assemblyFileResourceInfo.ResourceName);
            Assert.AreEqual(resourcePath, assemblyFileResourceInfo.ResourcePath);
        }

        #endregion

        #region ToString

        [Test]
        public void ToString_NameIsSet_ContainsResourceName()
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo("Some Name", "Some\\Path");

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.IsTrue(result.Contains(assemblyFileResourceInfo.ResourceName));
        }

        [Test]
        public void ToString_PathIsSet_ContainsResourcePath()
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo("Some Name", "Some\\Path");

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.IsTrue(result.Contains(assemblyFileResourceInfo.ResourcePath));
        }

        [Test]
        public void ToString_NameIsSet_PathIsSet_ContainsResourceNameAndPath()
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo("Some Name", "Some\\Path");

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.IsTrue(result.Contains(assemblyFileResourceInfo.ResourceName));
            Assert.IsTrue(result.Contains(assemblyFileResourceInfo.ResourcePath));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ToString_NameIsSet_PathIsNotSet_ContainsResourceName(string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo("Some Name", resourcePath);

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.IsTrue(result.Contains(assemblyFileResourceInfo.ResourceName));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ToString_NameIsNotSet_PathIsSet_DoesNotContainPath(string resourceName)
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo(resourceName, "Some\\Path");

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.IsFalse(result.Contains(assemblyFileResourceInfo.ResourcePath));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ToString_NameIsNotSet_PathIsSet_ClassNameIsReturned(string resourceName)
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo(resourceName, "Some\\Path");

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.AreEqual(assemblyFileResourceInfo.GetType().Name, result);
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        public void ToString_NameIsNotSet_PathIsNotSet_ClassNameIsReturned(string resourceName, string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo(resourceName, resourcePath);

            // Act
            string result = assemblyFileResourceInfo.ToString();

            // Assert
            Assert.AreEqual(assemblyFileResourceInfo.GetType().Name, result);
        }

        #endregion

        #region Equals

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("Some Name", "Some\\Path")]
        [TestCase("Some Name", null)]
        [TestCase("Some Name", "")]
        [TestCase(null, "Some\\Path")]
        [TestCase("", "Some\\Path")]
        [TestCase(" ", "Some\\Path")]
        public void Equals_TwoIdenticalConfigurations_ResultIsTrue(string resourceName, string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo(resourceName, resourcePath);
            var assemblyFileResourceInfo2 = new AssemblyFileResourceInfo(resourceName, resourcePath);

            // Act
            bool result = assemblyFileResourceInfo1.Equals(assemblyFileResourceInfo2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestCase("Some Name", "Some\\Path")]
        [TestCase("Some Name ", "Some\\Path ")]
        [TestCase(" Some Name", " Some\\Path")]
        public void Equals_TwoIdenticalConfigurations_WithDifferentNameCasing_ResultIsTrue(string resourceName, string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo(resourceName, resourcePath);
            var assemblyFileResourceInfo2 = new AssemblyFileResourceInfo(resourceName.ToUpper(), resourcePath.ToUpper());

            // Act
            bool result = assemblyFileResourceInfo1.Equals(assemblyFileResourceInfo2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestCase("Some Name", "Some\\Path")]
        [TestCase("Some Name ", "Some\\Path ")]
        [TestCase(" ", " ")]
        public void Equals_TwoIdenticalConfigurations_WithDifferentWhitespacesAtTheEnd_ResultIsFalse(string resourceName, string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo(resourceName, resourcePath);
            var assemblyFileResourceInfo2 = new AssemblyFileResourceInfo(resourceName + " ", resourcePath + " ");

            // Act
            bool result = assemblyFileResourceInfo1.Equals(assemblyFileResourceInfo2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase("Some Name", "Some\\Path")]
        [TestCase(" Some Name", " Some\\Path")]
        [TestCase(" ", " ")]
        public void Equals_TwoIdenticalConfigurations_WithDifferentWhitespacesAtTheBeginning_ResultIsFalse(string resourceName, string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo(resourceName, resourcePath);
            var assemblyFileResourceInfo2 = new AssemblyFileResourceInfo(" " + resourceName, " " + resourcePath);

            // Act
            bool result = assemblyFileResourceInfo1.Equals(assemblyFileResourceInfo2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectIsNull_ResultIsFalse()
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo("Some Name", "Some\\Path");
            AssemblyFileResourceInfo assemblyFileResourceInfo2 = null;

            // Act
            bool result = assemblyFileResourceInfo1.Equals(assemblyFileResourceInfo2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_SuppliedObjectOfAnotherType_ResultIsFalse()
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo("Some Name", "Some\\Path");
            object assemblyFileResourceInfo2 = new object();

            // Act
            bool result = assemblyFileResourceInfo1.Equals(assemblyFileResourceInfo2);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetHashCode

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase(" ", " ")]
        [TestCase("Some Name", "Some\\Path")]
        [TestCase("Some Name", null)]
        [TestCase(null, "Some\\Path")]
        public void GetHashCode_TwoIdenticalObjects_HashCodesAreEqual(string resourceName, string resourcePath)
        {
            // Arrange
            var assemblyFileResourceInfo1 = new AssemblyFileResourceInfo(resourceName, resourcePath);
            var assemblyFileResourceInfo2 = new AssemblyFileResourceInfo(resourceName, resourcePath);

            // Act
            int hashCodeObject1 = assemblyFileResourceInfo1.GetHashCode();
            int hashCodeObject2 = assemblyFileResourceInfo2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeObject1, hashCodeObject2);
        }

        [Test]
        public void GetHashCode_SameHashCodeIsReturnedEveryTimeTheMethodIsCalled_AsLongAsTheObjectDoesNotChange()
        {
            // Arrange
            string resourceName = "Some Name";
            string resourcePath = "Some\\Path";
            var assemblyFileResourceInfo = new AssemblyFileResourceInfo(resourceName, resourcePath);

            int expectedHashcode = assemblyFileResourceInfo.GetHashCode();

            for (var i = 0; i < 100; i++)
            {
                // Act
                assemblyFileResourceInfo.ResourceName = resourceName;
                assemblyFileResourceInfo.ResourcePath = resourcePath;
                int generatedHashCode = assemblyFileResourceInfo.GetHashCode();

                // Assert
                Assert.AreEqual(expectedHashcode, generatedHashCode);
            }
        }

        [Test]
        public void GetHashCode_ForAllUniqueObject_AUniqueHashCodeIsReturned()
        {
            var hashCodes = new Dictionary<int, AssemblyFileResourceInfo>();

            for (var i = 0; i < 10000; i++)
            {
                // Act
                var assemblyFileResourceInfo = new AssemblyFileResourceInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

                int generatedHashCode = assemblyFileResourceInfo.GetHashCode();

                // Assert
                Assert.IsFalse(hashCodes.ContainsKey(generatedHashCode));
                hashCodes.Add(generatedHashCode, assemblyFileResourceInfo);
            }
        }

        #endregion
    }
}