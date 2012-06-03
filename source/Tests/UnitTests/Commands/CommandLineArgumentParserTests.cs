using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandLineArgumentParserTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseCommandLineArguments_ArgumentsArrayIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var commandProviderMock = new Mock<ICommandProvider>();
            ICommandLineArgumentParser commandLineArgumentParser = new CommandLineArgumentParser(commandProviderMock.Object);

            // Act
            commandLineArgumentParser.ParseCommandLineArguments(null);
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayIsEmpty_ResultIsNull()
        {
            // Arrange
            var commandProviderMock = new Mock<ICommandProvider>();
            ICommandLineArgumentParser commandLineArgumentParser = new CommandLineArgumentParser(commandProviderMock.Object);

            string[] commandLineArguments = new string[] { };

            // Act
            ICommand result = commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayIsNotEmpty_CommandProviderReturnsNoCommands_ResultIsNull()
        {
            // Arrange
            var commandProviderMock = new Mock<ICommandProvider>();
            commandProviderMock.Setup(c => c.GetAvailableCommands()).Returns(new List<ICommand>());

            ICommandLineArgumentParser commandLineArgumentParser = new CommandLineArgumentParser(commandProviderMock.Object);

            string[] commandLineArguments = new[] { "commandName1" };

            // Act
            ICommand result = commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayContainsKnownCommandName_CommandIsReturned()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(new CommandAttributes
                {
                    CommandName = "help", 
                    AlternativeCommandNames = new[] { "?" }
                });

            var commandProviderMock = new Mock<ICommandProvider>();
            commandProviderMock.Setup(c => c.GetAvailableCommands()).Returns(new List<ICommand> { command.Object });

            ICommandLineArgumentParser commandLineArgumentParser = new CommandLineArgumentParser(commandProviderMock.Object);

            string[] commandLineArguments = new[] { "help" };

            // Act
            ICommand result = commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.AreEqual(command.Object.Attributes.CommandName, result.Attributes.CommandName);
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayContainsDifferentCasedCommandName_CommandIsReturned()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(new CommandAttributes
                {
                    CommandName = "help",
                    AlternativeCommandNames = new[] { "?" }
                });

            var commandProviderMock = new Mock<ICommandProvider>();
            commandProviderMock.Setup(c => c.GetAvailableCommands()).Returns(new List<ICommand> { command.Object });

            ICommandLineArgumentParser commandLineArgumentParser = new CommandLineArgumentParser(commandProviderMock.Object);

            string[] commandLineArguments = new[] { "HELP" };

            // Act
            ICommand result = commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.AreEqual(command.Object.Attributes.CommandName, result.Attributes.CommandName);
        }

        [Test]
        public void ParseCommandLineArguments_ArgumentsArrayContainsAlternativeCommandName_CommandIsReturned()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(new CommandAttributes
            {
                CommandName = "help",
                AlternativeCommandNames = new[] { "?" }
            });

            var commandProviderMock = new Mock<ICommandProvider>();
            commandProviderMock.Setup(c => c.GetAvailableCommands()).Returns(new List<ICommand> { command.Object });

            ICommandLineArgumentParser commandLineArgumentParser = new CommandLineArgumentParser(commandProviderMock.Object);

            string[] commandLineArguments = new[] { "help" };

            // Act
            ICommand result = commandLineArgumentParser.ParseCommandLineArguments(commandLineArguments);

            // Assert
            Assert.AreEqual(command.Object.Attributes.CommandName, result.Attributes.CommandName);
        }
    }
}
