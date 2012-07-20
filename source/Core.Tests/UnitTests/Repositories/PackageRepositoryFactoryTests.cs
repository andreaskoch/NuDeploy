using System;

using Moq;

using NuGet;

using NUnit.Framework;

using PackageRepositoryFactory = NuDeploy.Core.Services.Installation.Repositories.PackageRepositoryFactory;

namespace NuDeploy.Tests.UnitTests.Repositories
{
    [TestFixture]
    public class PackageRepositoryFactoryTests
    {
        #region constructor

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HttpClientFactoryParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            Func<Uri, IHttpClient> httpClientFactory = null;
            
            // Act
            new PackageRepositoryFactory(httpClientFactory);
        }

        [Test]
        public void Constructor_HttpClientFactoryParameterIsNotNull_ClassIsInstantiated()
        {
            // Arrange
            Func<Uri, IHttpClient> httpClientFactoryMock = u => new Mock<IHttpClient>().Object;

            // Act
            var packageRepositoryFactory = new PackageRepositoryFactory(httpClientFactoryMock);

            // Assert
            Assert.IsNotNull(packageRepositoryFactory);
        }

        #endregion

        #region HttpClientFactory

        [Test]
        public void HttpClientFactory_ReturnsTheHttpClientFactorySuppliedInTheConstructor()
        {
            // Arrange
            Func<Uri, IHttpClient> httpClientFactoryMock = u => new Mock<IHttpClient>().Object;
            var packageRepositoryFactory = new PackageRepositoryFactory(httpClientFactoryMock);

            // Act
            var result = packageRepositoryFactory.HttpClientFactory;

            // Assert
            Assert.AreEqual(httpClientFactoryMock, result);
        }

        #endregion

        #region CreateRepository

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateRepository_PackageSourceParameterIsInvalid_ArgumentExceptionIsThrown(string packageSource)
        {
            // Arrange
            Func<Uri, IHttpClient> httpClientFactoryMock = u => new Mock<IHttpClient>().Object;
            var packageRepositoryFactory = new PackageRepositoryFactory(httpClientFactoryMock);

            // Act
            packageRepositoryFactory.CreateRepository(packageSource);
        }

        [TestCase(@"C:\local-repo")]
        [TestCase(@"\\unc-path-repository\folder")]
        public void CreateRepository_PackageSourceParameterIsFileUri_LocalPackageRepositoryIsReturned(string packageSource)
        {
            // Arrange
            Func<Uri, IHttpClient> httpClientFactoryMock = u => new Mock<IHttpClient>().Object;
            var packageRepositoryFactory = new PackageRepositoryFactory(httpClientFactoryMock);

            // Act
            var result = packageRepositoryFactory.CreateRepository(packageSource);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(LocalPackageRepository), result.GetType());
        }


        [Test]
        public void CreateRepository_PackageSourceParameterIsARemoteUri_HttpClientFactoryIsCalled()
        {
            // Arrange
            bool httpClientFactoryGotCalled = false;

            var packageSource = "https://nuget.org/api/v2/";

            Func<Uri, IHttpClient> httpClientFactoryMock = u =>
                {
                    httpClientFactoryGotCalled = true;
                    return new Mock<IHttpClient>().Object;
                };

            var packageRepositoryFactory = new PackageRepositoryFactory(httpClientFactoryMock);

            // Act
            packageRepositoryFactory.CreateRepository(packageSource);

            // Assert
            Assert.IsTrue(httpClientFactoryGotCalled);
        }

        [Test]
        public void CreateRepository_PackageSourceParameterIsARemoteUri_DataServicePackageRepositoryIsReturned()
        {
            // Arrange
            bool httpClientFactoryGotCalled = false;

            var packageSource = "https://nuget.org/api/v2/";

            Func<Uri, IHttpClient> httpClientFactoryMock = u =>
            {
                httpClientFactoryGotCalled = true;
                return new Mock<IHttpClient>().Object;
            };

            var packageRepositoryFactory = new PackageRepositoryFactory(httpClientFactoryMock);

            // Act
            var result = packageRepositoryFactory.CreateRepository(packageSource);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(DataServicePackageRepository), result.GetType());
        }

        #endregion
    }
}