using System;
using System.IO;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;
using NuDeploy.Core.Common.Serialization;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.IntegrationTests.Publishing
{
    [TestFixture]
    public class PublishingServiceTests
    {
        #region setup

        private const string SamplePackageFilename = "Sample.1.0.0.nupkg";

        private IPublishingService publishingService;

        private ApplicationInformation applicationInformation;

        private IPublishConfigurationAccessor publishConfigurationAccessor;

        private string localPublishingFolder;

        private string samplePackageFilepath;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.applicationInformation = ApplicationInformationProvider.GetApplicationInformation();
            IEncodingProvider encodingProvider = new DefaultFileEncodingProvider();
            IFilesystemAccessor filesystemAccessor = new PhysicalFilesystemAccessor(encodingProvider);
            IPackageServerFactory packageServerFactory = new PackageServerFactory();
            IPublishConfigurationFactory publishConfigurationFactory = new PublishConfigurationFactory();
            IObjectSerializer<PublishConfiguration[]> publishConfigurationSerializer = new JSONObjectSerializer<PublishConfiguration[]>();
            IFilesystemPersistence<PublishConfiguration[]> publishConfigurationPersistence = new FilesystemPersistence<PublishConfiguration[]>(filesystemAccessor, publishConfigurationSerializer);
            this.publishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(this.applicationInformation, publishConfigurationFactory, publishConfigurationPersistence);

            this.publishingService = new PublishingService(filesystemAccessor, packageServerFactory, this.publishConfigurationAccessor);

            this.localPublishingFolder = Path.Combine(this.applicationInformation.StartupFolder, "publish-target");
            this.samplePackageFilepath = Path.Combine(this.applicationInformation.StartupFolder, "IntegrationTests", "Publishing", SamplePackageFilename);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            CoreIntegrationTestUtilities.RemoveAllFilesAndFoldersWhichAreCreatedOnStartup(this.applicationInformation);

            if (Directory.Exists(this.localPublishingFolder))
            {
                Directory.Delete(this.localPublishingFolder, true);
            }

            Directory.CreateDirectory(this.localPublishingFolder);
        }

        #endregion

        [Test]
        public void PublishPackage_LocalPublishingTarget_PackagePathExist_PackageServerConfigurationExists_ResultIsTrue_FileIsCreatedInTarget()
        {
            // Arrange Publish Configuration
            string publishConfigurationName = "Local Folder";
            string publishLocation = this.localPublishingFolder;
            string apiKey = null;

            this.publishConfigurationAccessor.AddOrUpdatePublishConfiguration(publishConfigurationName, publishLocation, apiKey);

            // Arrange Nuget Package
            string packagePath = this.samplePackageFilepath;

            // Act
            var result = this.publishingService.PublishPackage(packagePath, publishConfigurationName);

            // Assert
            string targetPath = Path.Combine(publishLocation, SamplePackageFilename);

            Assert.AreEqual(ServiceResultType.Success, result.Status);
            Assert.IsTrue(File.Exists(targetPath));
        }

        [Test]
        public void PublishPackage_RemotePublishingTarget_CannotPublishToSpecifiedRemoteLocation_FailureResultIsReturned()
        {
            // Arrange Publish Configuration
            string publishConfigurationName = "Nuget Gallery";
            string publishLocation = "http://www.nuget.org/api/v2";
            string apiKey = Guid.NewGuid().ToString();

            this.publishConfigurationAccessor.AddOrUpdatePublishConfiguration(publishConfigurationName, publishLocation, apiKey);

            // Arrange Nuget Package
            string packagePath = this.samplePackageFilepath;

            // Act
            var result = this.publishingService.PublishPackage(packagePath, publishConfigurationName);

            // Assert
            Assert.AreEqual(ServiceResultType.Failure, result.Status);
        }
    }
}