using System;

using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Installation.Repositories;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    public class PackageRepositoryBrowserTests
    {
        private IPackageRepositoryBrowser packageRepositoryBrowser;

        [TestFixtureSetUp]
        public void Setup()
        {
            var sourceRepositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProviderMock.Setup(r => r.GetRepositoryConfigurations()).Returns(
                new[] { new SourceRepositoryConfiguration { Name = "Default Nuget Repository", Url = NuDeployConstants.DefaultFeedUrl } });

            Func<Uri, IHttpClient> httpClientFactory = u => new RedirectedHttpClient(u);
            var packageRepositoryFactory = new CommandLineRepositoryFactory(httpClientFactory);

            this.packageRepositoryBrowser = new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactory);
        }

        [TestCase("jQuery")]
        [TestCase("Nuget.Core")]
        [TestCase("NuGet.CommandLine")]
        [TestCase("Newtonsoft.Json")]
        public void FindPackage_PackageIdIsAvailableInRepository_ResultIsNotNull_PackageRepositoryIsNotNull(string packageId)
        {
            // Arrange
            IPackageRepository packageRepository;

            // Act
            IPackage result = this.packageRepositoryBrowser.FindPackage(packageId, out packageRepository);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(packageRepository);
        }

        [Test]
        public void FindPackage_PackageIdIsNotAvailableInRepository_ResultIsNull_PackageRepositoryIsNull()
        {
            // Arrange
            string packageId = "Some-Non-Existing-Package-Id-312312312312";
            IPackageRepository packageRepository;

            // Act
            IPackage result = this.packageRepositoryBrowser.FindPackage(packageId, out packageRepository);

            // Assert
            Assert.IsNull(result);
            Assert.IsNull(packageRepository);
        }

        [Test]
        public void FindPackage_FirstPackageIsFound_SecondPackageNot_ResultIsNull_PackageRepositoryIsNull()
        {
            // Arrange
            string existingPackageId = "jQuery";
            string nonExistingPackageId = "Some-Non-Existing-Package-Id-312312312312";

            IPackageRepository packageRepository;
            this.packageRepositoryBrowser.FindPackage(existingPackageId, out packageRepository);

            // Act
            IPackage result = this.packageRepositoryBrowser.FindPackage(nonExistingPackageId, out packageRepository);

            // Assert
            Assert.IsNull(result);
            Assert.IsNull(packageRepository);
        }
    }
}