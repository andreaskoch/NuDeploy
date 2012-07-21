using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class HelpCommandTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();
            var commandProvider = new Mock<ICommandProvider>();

            // Act
            var helpCommand = new HelpCommand(helpProvider.Object, commandProvider.Object);

            // Assert
            Assert.IsNotNull(helpCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();
            var commandProvider = new Mock<ICommandProvider>();

            // Act
            var helpCommand = new HelpCommand(helpProvider.Object, commandProvider.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(helpCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HelpProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var commandProvider = new Mock<ICommandProvider>();

            // Act
            new HelpCommand(null, commandProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CommandProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();

            // Act
            new HelpCommand(helpProvider.Object, null);
        }

        #endregion

        #region Execute

        [Test]
        public void Execute_NoCommandNameArgumentSupplied_GeneralHelpIsCalled()
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();
            var commandProvider = new Mock<ICommandProvider>();

            var commands = new List<ICommand> { new Mock<ICommand>().Object };
            commandProvider.Setup(p => p.GetAvailableCommands()).Returns(commands);

            var helpCommand = new HelpCommand(helpProvider.Object, commandProvider.Object);

            // Act
            helpCommand.Execute();

            // Assert
            helpProvider.Verify(h => h.ShowHelpOverview(It.IsAny<IEnumerable<ICommand>>()), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_NoValidCommandNameIsSupplied_GeneralHelpIsCalled(string commandName)
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();
            var commandProvider = new Mock<ICommandProvider>();

            var commands = new List<ICommand> { new Mock<ICommand>().Object };
            commandProvider.Setup(p => p.GetAvailableCommands()).Returns(commands);

            var helpCommand = new HelpCommand(helpProvider.Object, commandProvider.Object);

            helpCommand.Arguments.Add(HelpCommand.ArgumentNameCommandName, commandName);

            // Act
            helpCommand.Execute();

            // Assert
            helpProvider.Verify(h => h.ShowHelpOverview(It.IsAny<IEnumerable<ICommand>>()), Times.Once());
        }

        [Test]
        public void Execute_SuppliedCommandNameDoesNotMatchAnyOfTheAvailableCommands_GeneralHelpIsCalled()
        {
            // Arrange
            string commandName = "SomeUnknownCommand";

            var helpProvider = new Mock<IHelpProvider>();
            var commandProvider = new Mock<ICommandProvider>();

            var commands = new List<ICommand> { CommandTestUtilities.GetCommand("test"), CommandTestUtilities.GetCommand("pause") };
            commandProvider.Setup(p => p.GetAvailableCommands()).Returns(commands);

            var helpCommand = new HelpCommand(helpProvider.Object, commandProvider.Object);

            helpCommand.Arguments.Add(HelpCommand.ArgumentNameCommandName, commandName);

            // Act
            helpCommand.Execute();

            // Assert
            helpProvider.Verify(h => h.ShowHelpOverview(It.IsAny<IEnumerable<ICommand>>()), Times.Once());
        }

        [Test]
        public void Execute_SuppliedCommandNameIsRecognized_CommandHelpIsCalled()
        {
            // Arrange
            string commandName = "knowncommand";

            var helpProvider = new Mock<IHelpProvider>();
            var commandProvider = new Mock<ICommandProvider>();

            var knownCommands = CommandTestUtilities.GetCommand(commandName);
            var commands = new List<ICommand> { knownCommands, CommandTestUtilities.GetCommand("pause") };
            commandProvider.Setup(p => p.GetAvailableCommands()).Returns(commands);

            var helpCommand = new HelpCommand(helpProvider.Object, commandProvider.Object);

            helpCommand.Arguments.Add(HelpCommand.ArgumentNameCommandName, commandName);

            // Act
            helpCommand.Execute();

            // Assert
            helpProvider.Verify(h => h.ShowHelp(knownCommands), Times.Once());
        }

        #endregion         
    }
}