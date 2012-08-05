using System.Text;

using NuDeploy.Core.Common.FileEncoding;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Common
{
    [TestFixture]
    public class DefaultFileEncodingProviderTests
    {
        private IEncodingProvider encodingProvider;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.encodingProvider = new DefaultFileEncodingProvider();
        }

        [Test]
        public void GetEncoding_ResultIsNotNull()
        {
            // Act
            var result = this.encodingProvider.GetEncoding();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetEncoding_ResultIsUTF8()
        {
            // Act
            var result = this.encodingProvider.GetEncoding();

            // Assert
            Assert.AreEqual(Encoding.UTF8, result);
        }
    }
}
