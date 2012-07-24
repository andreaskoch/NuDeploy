using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Repositories;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands.Console
{
    public class RepositorySourceConfigurationCommandTests
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
            var userInterface = new Mock<IUserInterface>();
            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            // Act
            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                userInterface.Object, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Assert
            Assert.IsNotNull(repositorySourceConfigurationCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            // Act
            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                userInterface.Object, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(repositorySourceConfigurationCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();

            // Act
            new RepositorySourceConfigurationCommand(null, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_RepositoryConfigurationCommandActionParserParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            // Act
            new RepositorySourceConfigurationCommand(userInterface.Object, null, sourceRepositoryProvider.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SourceRepositoryProviderParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();

            // Act
            new RepositorySourceConfigurationCommand(userInterface.Object, repositoryConfigurationCommandActionParser.Object, null);
        }

        #endregion

        #region Execute

        [Test]
        public void Execute_ActionArgumentIsNotSet_EmptyStringIsPassedToCommandActionParser()
        {
            // Arrange
            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            repositoryConfigurationCommandActionParser.Verify(p => p.ParseAction(string.Empty), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("add")]
        [TestCase("delete")]
        [TestCase("list")]
        [TestCase("reset")]
        public void Execute_ActionArgumentIsSet_ArgumentIsPassedToCommandActionParser(string actionParameter)
        {
            // Arrange
            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameAction, actionParameter);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            repositoryConfigurationCommandActionParser.Verify(p => p.ParseAction(actionParameter), Times.Once());
        }

        [Test]
        public void Execute_ActionArgumentIs_Unrecognized_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Unrecognized;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #region Add

        [Test]
        public void Execute_ActionArgumentIs_Add_NoArgumentsSupplied_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Add;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_NoArgumentsSupplied_SaveRepositoryConfigurationIsNotExecuted()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Add;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            sourceRepositoryProvider.Verify(s => s.SaveRepositoryConfiguration(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_RepositoryNameArgumentMissing_SaveRepositoryConfigurationIsNotExecuted()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Add;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            string url = "http://nuget.org./api/v2";
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryUrl, url);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            sourceRepositoryProvider.Verify(s => s.SaveRepositoryConfiguration(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_RepositoryUrlArgumentMissing_SaveRepositoryConfigurationIsNotExecuted()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Add;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            string repositoryName = "Test Repository";
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryName, repositoryName);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            sourceRepositoryProvider.Verify(s => s.SaveRepositoryConfiguration(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_SaveRepositoryConfigurationFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Add;
            string repositoryName = "Test Repository";
            string repositoryUrl = "http://nuget.org/api/v2";

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProvider.Setup(s => s.SaveRepositoryConfiguration(repositoryName, repositoryUrl)).Returns(false);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryName, repositoryName);
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryUrl, repositoryUrl);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("fail"));
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_SaveRepositoryConfigurationSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Add;
            string repositoryName = "Test Repository";
            string repositoryUrl = "http://nuget.org/api/v2";

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProvider.Setup(s => s.SaveRepositoryConfiguration(repositoryName, repositoryUrl)).Returns(true);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryName, repositoryName);
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryUrl, repositoryUrl);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("success") || this.loggingUserInterface.UserInterfaceOutput.Contains("suceeded"));
        }

        #endregion

        #region Delete

        [Test]
        public void Execute_ActionArgumentIs_Delete_NoArgumentsSupplied_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Delete;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_Delete_NoArgumentsSupplied_DeleteRepositoryConfigurationIsNotCalled()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Delete;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            sourceRepositoryProvider.Verify(s => s.DeleteRepositoryConfiguration(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Delete_DeleteRepositoryConfigurationFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string repositoryName = "Test Repository";
            var commandAction = RepositoryConfigurationCommandAction.Delete;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProvider.Setup(s => s.DeleteRepositoryConfiguration(repositoryName)).Returns(false);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryName, repositoryName);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_Delete_DeleteRepositoryConfigurationSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string repositoryName = "Test Repository";
            var commandAction = RepositoryConfigurationCommandAction.Delete;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProvider.Setup(s => s.DeleteRepositoryConfiguration(repositoryName)).Returns(true);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // prepare command arguments
            repositorySourceConfigurationCommand.Arguments.Add(RepositorySourceConfigurationCommand.ArgumentNameRepositoryName, repositoryName);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion

        #region List

        [Test]
        public void Execute_ActionArgumentIs_List_NoRepositoriesConfigured_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.List;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var configurations = new List<SourceRepositoryConfiguration>();
            sourceRepositoryProvider.Setup(s => s.GetRepositoryConfigurations()).Returns(configurations);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_List_AllConfiguredRepositoriesAreListed()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.List;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var configurations = new List<SourceRepositoryConfiguration>
                {
                    new SourceRepositoryConfiguration { Name = "Test Repo 1", Url = new Uri("http://test.repo1.com/api/v2") },
                    new SourceRepositoryConfiguration { Name = "Test Repo 2", Url = new Uri("http://test.repo2.com/api/v2") },
                    new SourceRepositoryConfiguration { Name = "Test Repo 3", Url = new Uri("http://test.repo3.com/api/v2") }
                };

            sourceRepositoryProvider.Setup(s => s.GetRepositoryConfigurations()).Returns(configurations);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            foreach (var repositoryConfiguration in configurations)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(repositoryConfiguration.Name));
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(repositoryConfiguration.Url.ToString()));
            }
        }

        #endregion

        #region Reset

        [Test]
        public void Execute_ActionArgumentIs_Reset_ResetRepositoryConfigurationIsCalled()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Reset;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            sourceRepositoryProvider.Verify(s => s.ResetRepositoryConfiguration(), Times.Once());
        }

        [Test]
        public void Execute_ActionArgumentIs_Reset_ResetRepositoryConfigurationFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Reset;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProvider.Setup(s => s.ResetRepositoryConfiguration()).Returns(false);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("fail"));
        }

        [Test]
        public void Execute_ActionArgumentIs_Reset_ResetRepositoryConfigurationSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = RepositoryConfigurationCommandAction.Reset;

            var repositoryConfigurationCommandActionParser = new Mock<IRepositoryConfigurationCommandActionParser>();
            repositoryConfigurationCommandActionParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var sourceRepositoryProvider = new Mock<ISourceRepositoryProvider>();
            sourceRepositoryProvider.Setup(s => s.ResetRepositoryConfiguration()).Returns(true);

            var repositorySourceConfigurationCommand = new RepositorySourceConfigurationCommand(
                this.loggingUserInterface.UserInterface, repositoryConfigurationCommandActionParser.Object, sourceRepositoryProvider.Object);

            // Act
            repositorySourceConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("success") || this.loggingUserInterface.UserInterfaceOutput.Contains("succeed"));
        }

        #endregion

        #endregion
    }
}