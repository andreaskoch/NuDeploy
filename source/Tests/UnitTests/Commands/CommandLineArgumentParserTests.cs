using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandLineArgumentParserTests
    {
        private ICommandLineArgumentParser commandLineArgumentParser;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.commandLineArgumentParser = new CommandLineArgumentParser();
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayIsNull_NoCommandIsReturned()
        {
            // Arrange
            string[] commandLineArguments = null;

            // Act
            ICommand result = this.commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayIsEmpty_NoCommandIsReturned()
        {
            // Arrange
            string[] commandLineArguments = new string[] { };

            // Act
            ICommand result = this.commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.IsNull(result);
        }
    }
}
