using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Infrastructure.Console;

namespace CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandLineArgumentInterpreterTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCommand_ArgumentsArrayIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var commandProviderMock = new Mock<ICommandProvider>();
            var commandNameMatcher = new Mock<ICommandNameMatcher>();
            var commandArgumentParser = new Mock<ICommandArgumentParser>();
            var commandArgumentNameMatcher = new Mock<ICommandArgumentNameMatcher>();

            ICommandLineArgumentInterpreter commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProviderMock.Object, commandNameMatcher.Object, commandArgumentParser.Object, commandArgumentNameMatcher.Object);

            // Act
            commandLineArgumentInterpreter.GetCommand(null);
        }

        [Test]
        public void GetCommand_ArgumentsArrayIsEmpty_ResultIsNull()
        {
            // Arrange
            var commandProviderMock = new Mock<ICommandProvider>();
            var commandNameMatcher = new Mock<ICommandNameMatcher>();
            var commandArgumentParser = new Mock<ICommandArgumentParser>();
            var commandArgumentNameMatcher = new Mock<ICommandArgumentNameMatcher>();

            ICommandLineArgumentInterpreter commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProviderMock.Object, commandNameMatcher.Object, commandArgumentParser.Object, commandArgumentNameMatcher.Object);

            var commandLineArguments = new string[] { };

            // Act
            ICommand result = commandLineArgumentInterpreter.GetCommand(commandLineArguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetCommand_ArgumentsArrayIsNotEmpty_CommandProviderReturnsNoCommands_ResultIsNull()
        {
            // Arrange
            var commandProviderMock = new Mock<ICommandProvider>();
            commandProviderMock.Setup(c => c.GetAvailableCommands()).Returns(new List<ICommand>());

            var commandNameMatcher = new Mock<ICommandNameMatcher>();
            var commandArgumentParser = new Mock<ICommandArgumentParser>();
            var commandArgumentNameMatcher = new Mock<ICommandArgumentNameMatcher>();

            ICommandLineArgumentInterpreter commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProviderMock.Object, commandNameMatcher.Object, commandArgumentParser.Object, commandArgumentNameMatcher.Object);

            var commandLineArguments = new[] { "commandName1" };

            // Act
            ICommand result = commandLineArgumentInterpreter.GetCommand(commandLineArguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetCommand_ArgumentsArrayContainsKnownCommandName_CommandIsReturned()
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

            var commandNameMatcher = new Mock<ICommandNameMatcher>();
            commandNameMatcher.Setup(c => c.IsMatch(command.Object, It.IsAny<string>())).Returns(true);

            var commandArgumentParser = new Mock<ICommandArgumentParser>();
            var commandArgumentNameMatcher = new Mock<ICommandArgumentNameMatcher>();

            ICommandLineArgumentInterpreter commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProviderMock.Object, commandNameMatcher.Object, commandArgumentParser.Object, commandArgumentNameMatcher.Object);

            var commandLineArguments = new[] { "help" };

            // Act
            ICommand result = commandLineArgumentInterpreter.GetCommand(commandLineArguments);

            // Assert
            Assert.AreEqual(command.Object.Attributes.CommandName, result.Attributes.CommandName);
        }

        [Test]
        public void GetCommand_ArgumentsArrayContainsDifferentCasedCommandName_CommandIsReturned()
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

            var commandNameMatcher = new Mock<ICommandNameMatcher>();
            commandNameMatcher.Setup(c => c.IsMatch(command.Object, It.IsAny<string>())).Returns(true);

            var commandArgumentParser = new Mock<ICommandArgumentParser>();
            var commandArgumentNameMatcher = new Mock<ICommandArgumentNameMatcher>();

            ICommandLineArgumentInterpreter commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProviderMock.Object, commandNameMatcher.Object, commandArgumentParser.Object, commandArgumentNameMatcher.Object);

            var commandLineArguments = new[] { "HELP" };

            // Act
            ICommand result = commandLineArgumentInterpreter.GetCommand(commandLineArguments);

            // Assert
            Assert.AreEqual(command.Object.Attributes.CommandName, result.Attributes.CommandName);
        }

        [Test]
        public void GetCommand_ArgumentsArrayContainsAlternativeCommandName_CommandIsReturned()
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

            var commandNameMatcher = new Mock<ICommandNameMatcher>();
            commandNameMatcher.Setup(c => c.IsMatch(command.Object, It.IsAny<string>())).Returns(true);

            var commandArgumentParser = new Mock<ICommandArgumentParser>();
            var commandArgumentNameMatcher = new Mock<ICommandArgumentNameMatcher>();

            ICommandLineArgumentInterpreter commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProviderMock.Object, commandNameMatcher.Object, commandArgumentParser.Object, commandArgumentNameMatcher.Object);

            var commandLineArguments = new[] { "help" };

            // Act
            ICommand result = commandLineArgumentInterpreter.GetCommand(commandLineArguments);

            // Assert
            Assert.AreEqual(command.Object.Attributes.CommandName, result.Attributes.CommandName);
        }
    }
}
