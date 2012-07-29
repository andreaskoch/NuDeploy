using System.IO;
using System.Linq;
using System.Threading;

using NuDeploy.CommandLine;
using NuDeploy.CommandLine.Commands;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

using StructureMap;

namespace CommandLine.Tests.IntegrationTests.CommandLine
{
    [TestFixture]
    public class ProgramMainTests
    {
        #region Test Setup
        
        private readonly Mutex sequentialTestExecutionMonitor;

        public ProgramMainTests()
        {
            this.sequentialTestExecutionMonitor = new Mutex(false);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            this.sequentialTestExecutionMonitor.WaitOne();

            CommandLineIntegrationTestUtilities.RemoveAllFilesAndFoldersWhichAreCreatedOnStartup();
        }

        [TearDown]
        public void TearDown()
        {
            this.sequentialTestExecutionMonitor.ReleaseMutex();
        }

        #endregion

        #region Help

        [Test]
        public void NoCommandLineArgumentsSupplied_ResultIsZero()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            int result = Program.Main(commandLineArguments);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void NoCommandLineArguments_HelpOverviewIsDisplayed_HelpTextContainsAllCommandNames()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            Program.Main(commandLineArguments);

            var commandProvider = ObjectFactory.GetInstance<ICommandProvider>();
            var userInterface = ObjectFactory.GetInstance<IUserInterface>();

            // Assert
            foreach (var command in commandProvider.GetAvailableCommands())
            {
                Assert.IsTrue(userInterface.UserInterfaceContent.Contains(command.Attributes.CommandName));
            }
        }

        #endregion

        #region Logging
        
        [Test]
        public void LogFolderIsCreated()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            Program.Main(commandLineArguments);

            // Assert
            var applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            Assert.IsTrue(Directory.Exists(applicationInformation.LogFolder));
        }

        [Test]
        public void LogFileIsCreatedInLogFolder()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            Program.Main(commandLineArguments);

            // Assert
            var applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            Assert.IsTrue(Directory.GetFiles(applicationInformation.LogFolder, "*.log", SearchOption.TopDirectoryOnly).Any());
        }

        #endregion

        #region Packaging

        [Test]
        public void BuildFolderIsCreatedOnStartup()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            Program.Main(commandLineArguments);

            // Assert
            var applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            Assert.IsTrue(Directory.Exists(applicationInformation.BuildFolder));
        }

        [Test]
        public void PrePackageFolderIsCreatedOnStartup()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            Program.Main(commandLineArguments);

            // Assert
            var applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            Assert.IsTrue(Directory.Exists(applicationInformation.PrePackagingFolder));
        }

        [Test]
        public void PackageFolderIsCreatedOnStartup()
        {
            // Arrange
            var commandLineArguments = new string[] { };

            // Act
            Program.Main(commandLineArguments);

            // Assert
            var applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            Assert.IsTrue(Directory.Exists(applicationInformation.PackagingFolder));
        }

        #endregion
    }
}