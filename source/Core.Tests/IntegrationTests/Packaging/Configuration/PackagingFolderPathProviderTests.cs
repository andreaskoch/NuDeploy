using System.IO;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Packaging.Configuration;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.Packaging.Configuration
{
    [TestFixture]
    public class PackagingFolderPathProviderTests
    {
        private ApplicationInformation applicationInformation;

        private IFilesystemAccessor fileSystemAccessor;

        private IPackagingFolderPathProvider packagingFolderPathProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            this.fileSystemAccessor = new PhysicalFilesystemAccessor(new DefaultFileEncodingProvider());
            this.packagingFolderPathProvider = new PackagingFolderPathProvider(this.applicationInformation, this.fileSystemAccessor);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            string tempFolderPath = this.applicationInformation.PackagingFolder;

            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
        }

        #region GetPackagingFolderPath

        [Test]
        public void GetPackagingFolderPath_ThePathThatIsReturnedIsCreated()
        {
            // Act
            var result = this.packagingFolderPathProvider.GetPackagingFolderPath();

            // Assert
            Assert.IsTrue(Directory.Exists(result));
        }

        #endregion
    }
}