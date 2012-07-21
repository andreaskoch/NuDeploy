using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private Mock<IUserInterface> loggingUserInterface;

        private StringBuilder output = new StringBuilder();

        [SetUp]
        public void BeforeEachTest()
        {
            this.output = new StringBuilder();
            this.loggingUserInterface = new Mock<IUserInterface>();

            this.loggingUserInterface.Setup(u => u.Write(It.IsAny<string>())).Callback((string text) => this.output.Append(text));

            this.loggingUserInterface.Setup(u => u.WriteLine(It.IsAny<string>())).Callback((string text) => this.output.AppendLine(text));

            this.loggingUserInterface.Setup(u => u.ShowIndented(It.IsAny<string>(), It.IsAny<int>())).Callback((string text, int indent) => this.output.AppendLine(text));

            this.loggingUserInterface.Setup(u => u.ShowKeyValueStore(It.IsAny<IDictionary<string, string>>(), It.IsAny<int>())).Callback(
                (IDictionary<string, string> keyValueStore, int distanceBetweenColumns) =>
                {
                    var flatList = keyValueStore.Select(pair => pair.Key + " " + pair.Value).ToList();
                    this.output.AppendLine(string.Join(Environment.NewLine, flatList));
                });

            this.loggingUserInterface.Setup(u => u.ShowKeyValueStore(It.IsAny<IDictionary<string, string>>(), It.IsAny<int>(), It.IsAny<int>())).Callback(
                (IDictionary<string, string> keyValueStore, int distanceBetweenColumns, int indentation) =>
                    {
                        var flatList = keyValueStore.Select(pair => pair.Key + " " + pair.Value).ToList();
                        this.output.AppendLine(string.Join(Environment.NewLine, flatList));
                    });

            this.loggingUserInterface.Setup(u => u.ShowLabelValuePair(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Callback(
                (string label, string value, int distanceBetweenLabelAndValue) => this.output.AppendLine(label + " " + value));
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

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            Assert.IsTrue(this.output.ToString().Contains(command.Attributes.CommandName));
        }

        [Test]
        public void ShowHelp_CommandDescription_IsWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            Assert.IsTrue(this.output.ToString().Contains(command.Attributes.Description));
        }

        [Test]
        public void ShowHelp_Examples_AreWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            foreach (var example in command.Attributes.Examples)
            {
                Assert.IsTrue(this.output.ToString().Contains(example.Key));
                Assert.IsTrue(this.output.ToString().Contains(example.Value));
            }
        }

        [Test]
        public void ShowHelp_ArgumentDescriptions_AreWrittenToTheUserInterface()
        {
            // Arrange
            ICommand command = CommandTestUtilities.GetCommand("SomeCommand");

            var applicationInformation = new ApplicationInformation();

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelp(command);

            // Assert
            foreach (var argumentDescription in command.Attributes.ArgumentDescriptions)
            {
                Assert.IsTrue(this.output.ToString().Contains(argumentDescription.Key));
                Assert.IsTrue(this.output.ToString().Contains(argumentDescription.Value));
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

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            Assert.IsTrue(this.output.ToString().Contains(applicationInformation.ApplicationName));
        }

        [Test]
        public void ShowHelpOverview_ApplicationVersion_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>();

            var applicationInformation = new ApplicationInformation { ApplicationVersion = new Version(3, 1, 4, 1) };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            Assert.IsTrue(this.output.ToString().Contains(applicationInformation.ApplicationVersion.ToString()));
        }

        [Test]
        public void ShowHelpOverview_NameOfExecutable_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>();

            var applicationInformation = new ApplicationInformation { NameOfExecutable = "AppName.exe" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            Assert.IsTrue(this.output.ToString().Contains(applicationInformation.NameOfExecutable));
        }

        [Test]
        public void ShowHelpOverview_CommandNameOfEachAvailableCommand_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand>
                { CommandTestUtilities.GetCommand("test"), CommandTestUtilities.GetCommand("update"), CommandTestUtilities.GetCommand("delete") };

            var applicationInformation = new ApplicationInformation { NameOfExecutable = "AppName.exe" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            foreach (var command in commands)
            {
                Assert.IsTrue(this.output.ToString().Contains(command.Attributes.CommandName));
            }
        }

        [Test]
        public void ShowHelpOverview_CommandDescriptionOfEachAvailableCommand_IsWrittenToTheUserInterface()
        {
            // Arrange
            var commands = new List<ICommand> { CommandTestUtilities.GetCommand("test"), CommandTestUtilities.GetCommand("update"), CommandTestUtilities.GetCommand("delete") };

            var applicationInformation = new ApplicationInformation { NameOfExecutable = "AppName.exe" };

            var helpProvider = new HelpProvider(applicationInformation, this.loggingUserInterface.Object);

            // Act
            helpProvider.ShowHelpOverview(commands);

            // Assert
            foreach (var command in commands)
            {
                Assert.IsTrue(this.output.ToString().Contains(command.Attributes.Description));
            }
        }

        #endregion
    }
}