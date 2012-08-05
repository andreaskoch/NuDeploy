using System;
using System.Text;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.CommandLine.Infrastructure.Console;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.CommandLine
{
    [TestFixture]
    public class ProgramTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new Mock<IHelpCommand>();
            
            // Act
            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Assert
            Assert.IsNotNull(program);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new Mock<IHelpCommand>();

            // Act
            new Program(null, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var logger = new Mock<IActionLogger>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new Mock<IHelpCommand>();

            // Act
            new Program(applicationInformation, null, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ActionLoggerParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new Mock<IHelpCommand>();

            // Act
            new Program(applicationInformation, userInterface.Object, null, commandLineArgumentInterpreter.Object, helpCommand.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CommandLineArgumentInterpreterParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var helpCommand = new HelpCommand(new Mock<IHelpProvider>().Object);

            // Act
            new Program(applicationInformation, userInterface.Object, logger.Object, null, helpCommand);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HelpCommandParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            // Act
            new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, null);
        }

        #endregion

        #region Run

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Run_CommandLineArgumentParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string[] commandlineArgument = null;

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new Mock<IHelpCommand>();

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            program.Run(commandlineArgument);
        }

        [Test]
        public void Run_AllCommandLineArgumentsAreWrittenToLogger()
        {
            // Arrange
            var commandlineArguments = new[] { "install", "-solutionPath=\"C:\\dev\\project\\solution.sln\"", "-buildConfiguration=DEV" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();

            var logMessages = new StringBuilder();
            var logger = new Mock<IActionLogger>();
            logger.Setup(l => l.Log(It.IsAny<string>(), It.IsAny<object[]>())).Callback(
                (string logMessage, object[] arguments) => logMessages.AppendLine(string.Format(logMessage, arguments)));

            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var command = new Mock<ICommand>();
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(commandlineArguments)).Returns(command.Object);

            var helpCommand = new Mock<IHelpCommand>();

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            program.Run(commandlineArguments);

            // Assert
            foreach (var commandlineArgument in commandlineArguments)
            {
                Assert.IsTrue(logMessages.ToString().Contains(commandlineArgument));
            }
        }

        [Test]
        public void Run_AllCommandLineArgumentsArePassedToTheCommandLineArgumentInterpreter()
        {
            // Arrange
            var commandlineArguments = new[] { "install", "C:\\dev\\project\\solution.sln", "DEV" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();

            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var command = new Mock<ICommand>();
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command.Object);

            var helpCommand = new Mock<IHelpCommand>();

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            program.Run(commandlineArguments);

            // Assert
            commandLineArgumentInterpreter.Verify(c => c.GetCommand(commandlineArguments), Times.Once());
        }

        [Test]
        public void Run_CommandLineArgumentInterpreterReturnsACommand_ExecuteIsCalledOnThisCommand()
        {
            // Arrange
            var commandlineArguments = new[] { "somecommand" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();

            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            var command = new Mock<ICommand>();
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command.Object);

            var helpCommand = new Mock<IHelpCommand>();

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            program.Run(commandlineArguments);

            // Assert
            command.Verify(c => c.Execute(), Times.Once());
        }

        [Test]
        public void Run_CommandLineArgumentInterpreterReturnsNull_HelpCommandIsExecuted()
        {
            // Arrange
            var commandlineArguments = new[] { "somecommand" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();

            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            ICommand command = null;
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command);

            var helpCommand = new Mock<IHelpCommand>();

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            program.Run(commandlineArguments);

            // Assert
            helpCommand.Verify(c => c.Execute(), Times.Once());
        }

        [Test]
        public void Run_ResultOfCommandExecutionIsTrue_ResultIsZero()
        {
            // Arrange
            var commandlineArguments = new[] { "somecommand" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var helpCommand = new Mock<IHelpCommand>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            var command = new Mock<ICommand>();
            command.Setup(c => c.Execute()).Returns(true);
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command.Object);

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            int result = program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Run_ResultOfCommandExecutionIsFalse_ResultIsOne()
        {
            // Arrange
            var commandlineArguments = new[] { "somecommand" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var helpCommand = new Mock<IHelpCommand>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            var command = new Mock<ICommand>();
            command.Setup(c => c.Execute()).Returns(false);
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command.Object);

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            int result = program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Run_CommandExecuteThrowsException_ResultIsOne()
        {
            // Arrange
            var commandlineArguments = new[] { "somecommand" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var helpCommand = new Mock<IHelpCommand>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            var command = new Mock<ICommand>();
            command.Setup(c => c.Execute()).Throws(new Exception());
            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command.Object);

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            int result = program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Run_CommandExecuteThrowsException_ExceptionMessageIsWrittenToUserInterface()
        {
            // Arrange
            var commandlineArguments = new[] { "somecommand" };

            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var logger = new Mock<IActionLogger>();
            var helpCommand = new Mock<IHelpCommand>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();

            var command = new Mock<ICommand>();

            var exception = new Exception("Exception message yada yada yada");
            command.Setup(c => c.Execute()).Throws(exception);

            commandLineArgumentInterpreter.Setup(c => c.GetCommand(It.IsAny<string[]>())).Returns(command.Object);

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand.Object);

            // Act
            program.Run(commandlineArguments);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.Is<string>(message => message.Contains(exception.Message))));
        }

        #endregion
    }
}