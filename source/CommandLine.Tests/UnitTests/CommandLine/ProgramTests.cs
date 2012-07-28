using System;

using Moq;

using NuDeploy.CommandLine;
using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.CommandLine.Infrastructure.Console;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.CommandLine
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
            var helpCommand = new HelpCommand(new Mock<IHelpProvider>().Object);
            
            // Act
            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand);

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
            var helpCommand = new HelpCommand(new Mock<IHelpProvider>().Object);

            // Act
            new Program(null, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var logger = new Mock<IActionLogger>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new HelpCommand(new Mock<IHelpProvider>().Object);

            // Act
            new Program(applicationInformation, null, logger.Object, commandLineArgumentInterpreter.Object, helpCommand);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ActionLoggerParameterIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var userInterface = new Mock<IUserInterface>();
            var commandLineArgumentInterpreter = new Mock<ICommandLineArgumentInterpreter>();
            var helpCommand = new HelpCommand(new Mock<IHelpProvider>().Object);

            // Act
            new Program(applicationInformation, userInterface.Object, null, commandLineArgumentInterpreter.Object, helpCommand);
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
            var helpCommand = new HelpCommand(new Mock<IHelpProvider>().Object);

            var program = new Program(applicationInformation, userInterface.Object, logger.Object, commandLineArgumentInterpreter.Object, helpCommand);

            // Act
            program.Run(commandlineArgument);
        }

        #endregion
    }
}