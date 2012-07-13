using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Installation.Repositories;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    public class CommandLineRepositoryFactoryTests
    {
        private IPackageRepositoryFactory commandLineRepositoryFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.commandLineRepositoryFactory = new CommandLineRepositoryFactory();
        }

        [Test]
        public void CreateRepository_PackageSourceParameterIsValid_ResultIsNotNull()
        {
            // Arrange
            string packageSource = NuDeployConstants.DefaultFeedUrl.ToString();

            // Act
            IPackageRepository result = this.commandLineRepositoryFactory.CreateRepository(packageSource);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void CreateRepository_PackageSourceParameterIsValid_GetPackagesReturnsAPackage()
        {
            // Arrange
            string packageSource = NuDeployConstants.DefaultFeedUrl.ToString();
            IPackageRepository packageRepository = this.commandLineRepositoryFactory.CreateRepository(packageSource);

            // Act
            var package = packageRepository.GetPackages().First();
           
            // Assert
            Assert.IsNotNull(package);
        }
    }
}