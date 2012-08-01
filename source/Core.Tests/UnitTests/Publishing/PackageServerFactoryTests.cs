using System;

using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Publishing
{
    [TestFixture]
    public class PackageServerFactoryTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPackageServer_ServerLocationParameterIsInvalid_ArgumentExceptionIsThrown(string serverLocation)
        {
            // Arrange
            var packageServerFactory = new PackageServerFactory();

            // Act
            packageServerFactory.GetPackageServer(serverLocation);
        }

        [Test]
        public void GetPackageServer_ServerLocationParameterIsValid_ResultIsNotNull()
        {
            // Arrange
            var serverLocation = "http://nuget.org/api/v2";
            var packageServerFactory = new PackageServerFactory();

            // Act
            var result = packageServerFactory.GetPackageServer(serverLocation);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestCase("http://nuget.org/api/v2")]
        [TestCase(@"C:\nuget-repository")]
        [TestCase(@"\\unc-path\repository")]
        public void GetPackageServer_ServerLocationParameterIsValid_PackageServerSourceParameterEqualsSuppliedServerLocation(string serverLocation)
        {
            // Arrange
            var packageServerFactory = new PackageServerFactory();

            // Act
            var result = packageServerFactory.GetPackageServer(serverLocation);

            // Assert
            Assert.AreEqual(serverLocation, result.Source);
        }
    }
}