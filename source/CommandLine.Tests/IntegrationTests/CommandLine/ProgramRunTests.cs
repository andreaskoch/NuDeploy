using System.IO;
using System.Linq;
using System.Threading;

using NuDeploy.CommandLine;
using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.CommandLine.DependencyResolution;
using NuDeploy.CommandLine.Infrastructure.Console;
using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

using StructureMap;

namespace CommandLine.Tests.IntegrationTests.CommandLine
{
    [TestFixture]
    public class ProgramRunTests
    {
        #region Test Setup
        
        private readonly Mutex sequentialTestExecutionMonitor;

        private Program program;

        private ApplicationInformation applicationInformation;

        private IUserInterface userInterface;

        private IActionLogger logger;

        private ICommandLineArgumentInterpreter commandLineArgumentInterpreter;

        private IHelpCommand helpCommand;

        private ICommandProvider commandProvider;

        private IEncodingProvider encodingProvider;

        public ProgramRunTests()
        {
            this.sequentialTestExecutionMonitor = new Mutex(false);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            this.sequentialTestExecutionMonitor.WaitOne();

            CommandLineIntegrationTestUtilities.RemoveAllFilesAndFoldersWhichAreCreatedOnStartup();

            StructureMapSetup.Setup();
            this.encodingProvider = ObjectFactory.GetInstance<IEncodingProvider>();
            this.applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            this.commandProvider = ObjectFactory.GetInstance<ICommandProvider>();
            this.userInterface = ObjectFactory.GetInstance<IUserInterface>();
            this.logger = ObjectFactory.GetInstance<IActionLogger>();
            this.commandLineArgumentInterpreter = ObjectFactory.GetInstance<ICommandLineArgumentInterpreter>();
            this.helpCommand = ObjectFactory.GetInstance<IHelpCommand>();

            this.program = new Program(this.applicationInformation, this.userInterface, this.logger, this.commandLineArgumentInterpreter, this.helpCommand);
        }

        [TearDown]
        public void TearDown()
        {
            this.sequentialTestExecutionMonitor.ReleaseMutex();
        }

        #endregion

        #region General

        [Test]
        public void NoCommandLineArgumentsSupplied_ResultIsZero()
        {
            // Arrange
            var commandlineArguments = new string[] { };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestCase("UnknownCommand")]
        [TestCase("asjdjsakdjsakldj")]
        [TestCase("help install")]
        [TestCase("help package")]
        [TestCase("install")]
        public void RandomCommandsArePassedToTheApplication_CommandIsLoggedToDisc(string commandLine)
        {
            // Arrange
            var commandlineArguments = commandLine.Split(' ');

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            var logFilePath = Directory.GetFiles(this.applicationInformation.LogFolder, "*.log").First();
            var logFileContent = File.ReadAllText(logFilePath, this.encodingProvider.GetEncoding());
            Assert.IsTrue(logFileContent.Contains(commandLine));
        }

        #endregion

        #region Help Overview

        [Test]
        public void NoCommandLineArguments_HelpOverviewIsDisplayed_HelpTextContainsAllCommandNames()
        {
            // Arrange
            var commandlineArguments = new string[] { };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            foreach (var command in this.commandProvider.GetAvailableCommands())
            {
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.CommandName));
            }
        }

        [TestCase("UnknownCommand")]
        [TestCase("asjdjsakdjsakldj")]
        [TestCase("")]
        [TestCase(" ")]
        public void CommandIsNotRecognized_HelpOverviewIsDisplayed_HelpTextContainsAllCommandNames(string commandName)
        {
            // Arrange
            var commandlineArguments = new[] { commandName };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            foreach (var command in this.commandProvider.GetAvailableCommands())
            {
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.CommandName));
            }
        }

        #endregion

        #region Help (Command specific)

        [TestCase(CleanupCommand.CommandName)]
        [TestCase(HelpCommand.CommandName)]
        [TestCase(InstallationStatusCommand.CommandName)]
        [TestCase(InstallCommand.CommandName)]
        [TestCase(PackageSolutionCommand.CommandName)]
        [TestCase(RepositorySourceConfigurationCommand.CommandName)]
        [TestCase(SelfUpdateCommand.CommandName)]
        [TestCase(UninstallCommand.CommandName)]
        public void HelpCommandIsCalled_ArgumentIsKnownCommand_CommandSpecificHelpIsDisplayed(string commandName)
        {
            // Arrange
            var commandlineArguments = new[] { "help", commandName };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            var command = this.commandProvider.GetAvailableCommands().FirstOrDefault(c => c.Attributes.CommandName.Equals(commandName));

            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.CommandName), string.Format("The command help should display the command name \"{0}\".", command.Attributes.CommandName));
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.Description), string.Format("The command help should display the command description text \"{0}\".", command.Attributes.Description));

            foreach (var argument in command.Attributes.RequiredArguments)
            {
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(argument), string.Format("The command help should display the command argument \"{0}\".", argument));
            }
        }

        #endregion
    }
}