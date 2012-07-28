using System.IO;
using System.Linq;
using System.Threading;

using NuDeploy.CommandLine;
using NuDeploy.CommandLine.Commands;
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

            this.Cleanup();

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

        #region utility methods

        private void Cleanup()
        {
            var appInfo = ApplicationInformationProvider.GetApplicationInformation();

            this.DeleteFolder(appInfo.LogFolder);
            this.DeleteFolder(appInfo.BuildFolder);
            this.DeleteFolder(appInfo.PrePackagingFolder);
            this.DeleteFolder(appInfo.PackagingFolder);
        }

        private void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }            
        }

        #endregion
    }
}