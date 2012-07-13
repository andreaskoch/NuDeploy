using System;

using Moq;

using NuDeploy.Core.Services.Installation.Repositories;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Repositories
{
    [TestFixture]
    public class PackageRepositoryBrowserTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SourceRepositoryProviderParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISourceRepositoryProvider sourceRepositoryProvider = null;
            IPackageRepositoryFactory packageRepositoryFactory = new Mock<IPackageRepositoryFactory>().Object;

            // Act
            new PackageRepositoryBrowser(sourceRepositoryProvider, packageRepositoryFactory);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageRepositoryFactoryParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ISourceRepositoryProvider sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>().Object;
            IPackageRepositoryFactory packageRepositoryFactory = null;

            // Act
            new PackageRepositoryBrowser(sourceRepositoryProvider, packageRepositoryFactory);
        }
    }
}