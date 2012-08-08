using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands.Console
{
    public class PublishingTargetConfigurationCommandTests
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
            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            // Act
            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                userInterface.Object, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Assert
            Assert.IsNotNull(publishingTargetConfigurationCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            // Act
            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                userInterface.Object, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(publishingTargetConfigurationCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();

            // Act
            new PublishingTargetConfigurationCommand(null, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishingTargetConfigurationCommandParserParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            // Act
            new PublishingTargetConfigurationCommand(userInterface.Object, null, publishConfigurationAccessor.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishConfigurationAccessorParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();

            // Act
            new PublishingTargetConfigurationCommand(userInterface.Object, publishingTargetConfigurationCommandParser.Object, null);
        }

        #endregion

        #region Execute

        [Test]
        public void Execute_ActionArgumentIsNotSet_EmptyStringIsPassedToCommandActionParser()
        {
            // Arrange
            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishingTargetConfigurationCommandParser.Verify(p => p.ParseAction(string.Empty), Times.Once());
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
            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNameAction, actionParameter);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishingTargetConfigurationCommandParser.Verify(p => p.ParseAction(actionParameter), Times.Once());
        }

        [Test]
        public void Execute_ActionArgumentIs_Unrecognized_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Unrecognized;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #region Add

        [Test]
        public void Execute_ActionArgumentIs_Add_NoArgumentsSupplied_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Add;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_NoArgumentsSupplied_AddOrUpdatePublishConfigurationIsNotExecuted()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Add;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishConfigurationAccessor.Verify(s => s.AddOrUpdatePublishConfiguration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_RepositoryNameArgumentMissing_AddOrUpdatePublishConfigurationIsNotExecuted()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Add;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            string url = "http://nuget.org./api/v2";
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishLocation, url);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishConfigurationAccessor.Verify(s => s.AddOrUpdatePublishConfiguration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_PublishLocationArgumentMissing_AddOrUpdatePublishConfigurationIsNotExecuted()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Add;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            string publishConfigurationName = "Nuget Gallery";
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishConfigurationName, publishConfigurationName);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishConfigurationAccessor.Verify(s => s.AddOrUpdatePublishConfiguration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_AddOrUpdatePublishConfigurationFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Add;
            string publishConfigurationName = "Nuget Gallery";
            string publishLocation = "http://nuget.org/api/v2";

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(s => s.AddOrUpdatePublishConfiguration(publishConfigurationName, publishLocation, It.IsAny<string>())).Returns(new FailureResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishConfigurationName, publishConfigurationName);
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishLocation, publishLocation);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("fail"));
        }

        [Test]
        public void Execute_ActionArgumentIs_Add_AddOrUpdatePublishConfigurationSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Add;
            string publishConfigurationName = "Nuget Gallery";
            string publishLocation = "http://nuget.org/api/v2";

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(s => s.AddOrUpdatePublishConfiguration(publishConfigurationName, publishLocation, It.IsAny<string>())).Returns(new SuccessResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishConfigurationName, publishConfigurationName);
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishLocation, publishLocation);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("success") || this.loggingUserInterface.UserInterfaceOutput.Contains("suceeded"));
        }

        #endregion

        #region Delete

        [Test]
        public void Execute_ActionArgumentIs_Delete_NoArgumentsSupplied_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Delete;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_Delete_NoArgumentsSupplied_DeletePublishConfigurationIsNotCalled()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Delete;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments = new Dictionary<string, string>();

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishConfigurationAccessor.Verify(s => s.DeletePublishConfiguration(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_ActionArgumentIs_Delete_DeletePublishConfigurationFails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string publishConfigurationName = "Nuget Gallery";
            var commandAction = PublishingTargetConfigurationCommandAction.Delete;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(s => s.DeletePublishConfiguration(publishConfigurationName)).Returns(new FailureResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishConfigurationName, publishConfigurationName);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_Delete_DeletePublishConfigurationSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string publishConfigurationName = "Nuget Gallery";
            var commandAction = PublishingTargetConfigurationCommandAction.Delete;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(s => s.DeletePublishConfiguration(publishConfigurationName)).Returns(new SuccessResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // prepare command arguments
            publishingTargetConfigurationCommand.Arguments.Add(PublishingTargetConfigurationCommand.ArgumentNamePublishConfigurationName, publishConfigurationName);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion

        #region List

        [Test]
        public void Execute_ActionArgumentIs_List_NoPublishingTargetsConfigured_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.List;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var configurations = new List<PublishConfiguration>();
            publishConfigurationAccessor.Setup(s => s.GetPublishConfigurations()).Returns(configurations);

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_ActionArgumentIs_List_AllConfiguredPublishingTargetsAreListed()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.List;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();

            var configurations = new List<PublishConfiguration>
                {
                    new PublishConfiguration { Name = "Test Repo 1", PublishLocation = "http://test.repo1.com/api/v2" },
                    new PublishConfiguration { Name = "Test Repo 2", PublishLocation = "http://test.repo2.com/api/v2" },
                    new PublishConfiguration { Name = "Test Repo 3", PublishLocation = "http://test.repo3.com/api/v2" }
                };

            publishConfigurationAccessor.Setup(s => s.GetPublishConfigurations()).Returns(configurations);

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            foreach (var repositoryConfiguration in configurations)
            {
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(repositoryConfiguration.Name));
                Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains(repositoryConfiguration.PublishLocation));
            }
        }

        #endregion

        #region Reset

        [Test]
        public void Execute_ActionArgumentIs_Reset_ResetIsCalled()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Reset;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(p => p.ResetPublishConfiguration()).Returns(new SuccessResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            publishConfigurationAccessor.Verify(s => s.ResetPublishConfiguration(), Times.Once());
        }

        [Test]
        public void Execute_ActionArgumentIs_Reset_Fails_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Reset;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(s => s.ResetPublishConfiguration()).Returns(new FailureResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("fail"));
        }

        [Test]
        public void Execute_ActionArgumentIs_Reset_Succeeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandAction = PublishingTargetConfigurationCommandAction.Reset;

            var publishingTargetConfigurationCommandParser = new Mock<IPublishingTargetConfigurationCommandActionParser>();
            publishingTargetConfigurationCommandParser.Setup(r => r.ParseAction(It.IsAny<string>())).Returns(commandAction);

            var publishConfigurationAccessor = new Mock<IPublishConfigurationAccessor>();
            publishConfigurationAccessor.Setup(s => s.ResetPublishConfiguration()).Returns(new SuccessResult());

            var publishingTargetConfigurationCommand = new PublishingTargetConfigurationCommand(
                this.loggingUserInterface.UserInterface, publishingTargetConfigurationCommandParser.Object, publishConfigurationAccessor.Object);

            // Act
            publishingTargetConfigurationCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.Contains("success") || this.loggingUserInterface.UserInterfaceOutput.Contains("succeed"));
        }

        #endregion

        #endregion
    }
}