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

            // Act
            var helpCommand = new HelpCommand(helpProvider.Object);

            // Assert
            Assert.IsNotNull(helpCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();

            // Act
            var helpCommand = new HelpCommand(helpProvider.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(helpCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HelpProviderParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Act
            new HelpCommand(null);
        }

        #endregion

        #region Execute

        [Test]
        public void Execute_NoCommandNameArgumentSupplied_GeneralHelpIsCalled()
        {
            // Arrange
            var helpProvider = new Mock<IHelpProvider>();

            var helpCommand = new HelpCommand(helpProvider.Object);

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

            var helpCommand = new HelpCommand(helpProvider.Object);

            helpCommand.Arguments.Add(HelpCommand.ArgumentNameCommandName, commandName);

            // Act
            helpCommand.Execute();

            // Assert
            helpProvider.Verify(h => h.ShowHelpOverview(It.IsAny<IEnumerable<ICommand>>()), Times.Once());
        }

        #endregion         
    }
}