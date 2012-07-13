using Moq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services.Installation.Repositories;

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

            var packageRepositoryFactory = new CommandLineRepositoryFactory();

            this.packageRepositoryBrowser = new PackageRepositoryBrowser(sourceRepositoryProviderMock.Object, packageRepositoryFactory);
        }
    }
}