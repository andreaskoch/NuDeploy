using System.Threading;

using NuDeploy.CommandLine;
using NuDeploy.CommandLine.Commands;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

using StructureMap;

namespace CommandLine.Tests.IntegrationTests.CommandLine
{
    [TestFixture]
    public class ProgramMainTests
    {
        private readonly Mutex sequentialTestExecutionMonitor;

        public ProgramMainTests()
        {
            this.sequentialTestExecutionMonitor = new Mutex(false);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            this.sequentialTestExecutionMonitor.WaitOne();

            CommandLineIntegrationTestUtilities.Cleanup();
        }

        [TearDown]
        public void TearDown()
        {
            this.sequentialTestExecutionMonitor.ReleaseMutex();
        }

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
    }
}