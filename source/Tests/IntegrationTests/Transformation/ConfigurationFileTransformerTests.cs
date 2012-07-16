using Moq;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Transformation;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.Transformation
{
    [TestFixture]
    public class ConfigurationFileTransformerTests
    {
        private IFilesystemAccessor filesystemAccessor;

        private IConfigurationFileTransformer configurationFileTransformer;

        [TestFixtureSetUp]
        public void Setup()
        {
            IUserInterface userInterface = new Mock<IUserInterface>().Object;
            this.filesystemAccessor = new PhysicalFilesystemAccessor(new DefaultFileEncodingProvider());
            this.configurationFileTransformer = new ConfigurationFileTransformer(userInterface, this.filesystemAccessor);
        }
    }
}