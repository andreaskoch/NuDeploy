using System;

using NuDeploy.Core.Services.Filesystem;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Filesystem
{
    [TestFixture]
    public class RelativeFilePathInfoFactoryTests
    {
        private IRelativeFilePathInfoFactory relativeFilePathInfoFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.relativeFilePathInfoFactory = new RelativeFilePathInfoFactory();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRelativeFilePathInfo_AbsoluteFilePathIsInvalid_ArgumentExceptionIsThrown(string absoluteFilePath)
        {
            // Arrange
            string basePath = @"NuDeploy\Core\Resources";

            // Act
            this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(absoluteFilePath, basePath);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRelativeFilePathInfo_BasePathIsInvalid_ArgumentExceptionIsThrown(string basePath)
        {
            // Arrange
            string absoluteFilePath = @"NuDeploy\Core\Resources\File1.txt";

            // Act
            this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(absoluteFilePath, basePath);
        }

        [TestCase(@"NuDeploy\Core\Resources\File1.txt", @"Resources\Core")]
        public void GetRelativeFilePathInfo_BasePathIsNotPartOfTheAbsoluteFilePath_ResultIsNull(string absoluteFilePath, string basePath)
        {
            // Act
            var result = this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(absoluteFilePath, basePath);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase(@"NuDeploy\Core\Resources\File1.txt", @"NuDeploy\Core", @"Resources\File1.txt")]
        [TestCase(@"NuDeploy\Core\Resources\File1.txt", @"nudeploy\core", @"Resources\File1.txt")]
        [TestCase(@"NuDeploy\Core\Resources\File1.txt", @"NuDeploy\", @"Core\Resources\File1.txt")]
        [TestCase(@"NuDeploy\File1.txt", @"NuDeploy", @"File1.txt")]
        public void GetRelativeFilePathInfo_BasePathIsValid_AbsolutePathIsValid_ResultIsNotNull(string absoluteFilePath, string basePath, string expectedResult)
        {
            // Act
            var result = this.relativeFilePathInfoFactory.GetRelativeFilePathInfo(absoluteFilePath, basePath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result.RelativeFilePath);
            Assert.AreEqual(absoluteFilePath, result.AbsoluteFilePath);
        }
    }
}