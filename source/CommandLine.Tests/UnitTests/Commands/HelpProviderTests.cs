using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class HelpProviderTests
    {
        private LoggingUserInterface loggingUserInterface;

        [SetUp]
        public void BeforeEachTest()
        {
            this.loggingUserInterface = new LoggingUserInterface();
        }

        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();

            // Act
            var helpProvider = new HelpProvider(applicationInformation, userInterface.Object);

            // Assert
            Assert.IsNotNull(helpProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();

            // Act
            new HelpProvider(null, userInterface.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();

            // Act
            new HelpProvider(applicationInformation, null);
        }

        #endregion

        #region ShowHelp

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShowHelp_SuppliedCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ICommand command = null;

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();

            var helpProvider = new HelpProvider(applicationInformation, userInterface.Object);

            // Act
            helpProvider.ShowHelp(command);
        }

        [Test]
        public void ShowHelp_CommandName_IsWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(command.Attributes.CommandName));
        }

        [Test]
        public void ShowHelp_CommandDescription_IsWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(command.Attributes.Description));
        }

        [Test]
        public void ShowHelp_Examples_AreWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            foreach (var example in command.Attributes.Examples)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(example.Key));
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(example.Value));
            }
        }

        [Test]
        public void ShowHelp_ArgumentDescriptions_AreWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            foreach (var argumentDescription in command.Attributes.ArgumentDescriptions)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(argumentDescription.Key));
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(argumentDescription.Value));
            }
        }

        #endregion

        #region ShowHelpOverview

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShowHelpOverview_CommandsParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            List<ICommand> commands = null;

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();

            var helpProvider = new HelpProvider(applicationInformation, userInterface.Object);

            // Act
            helpProvider.ShowHelpOverview(commands);
        }

        [Test]
        public void ShowHelpOverview_ApplicationName_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>();

            var applicationInformation = new ApplicationInformation { ApplicationName = "App Name" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(applicationInformation.ApplicationName));
        }

        [Test]
        public void ShowHelpOverview_ApplicationVersion_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>();

            var applicationInformation = new ApplicationInformation { ApplicationVersion = new Version(3, 1, 4, 1) };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(applicationInformation.ApplicationVersion.ToString()));
        }

        [Test]
        public void ShowHelpOverview_NameOfExecutable_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>();

            var applicationInformation = new ApplicationInformation { NameOfExecutable = "AppName.exe" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(applicationInformation.NameOfExecutable));
        }

        [Test]
        public void ShowHelpOverview_CommandNameOfEachAvailableCommand_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>
                { CommandTestUtilities.GetCommand("test"), CommandTestUtilities.GetCommand("update"), CommandTestUtilities.GetCommand("delete") };

            var applicationInformation = new ApplicationInformation { NameOfExecutable = "AppName.exe" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            foreach (var command in commands)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(command.Attributes.CommandName));
            }
        }

        [Test]
        public void ShowHelpOverview_CommandDescriptionOfEachAvailableCommand_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand> { CommandTestUtilities.GetCommand("test"), CommandTestUtilities.GetCommand("update"), CommandTestUtilities.GetCommand("delete") };

            var applicationInformation = new ApplicationInformation { NameOfExecutable = "AppName.exe" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.UserInterface);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            foreach (var command in commands)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(command.Attributes.Description));
            }
        }

        #endregion
    }
}