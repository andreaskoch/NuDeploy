using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Common;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests
{
    [TestFixture]
    public class CommandLineArgumentInterpreterTests
    {
        private ICommand packackageSolutionCommand;

        private IEnumerable<ICommand> commands;

        private ICommand helpCommand;

        private ICommandLineArgumentInterpreter commandLineArgumentInterpreter;

        [TestFixtureSetUp]
        public void Setup()
        {
            var userInterfaceMock = new Mock<IUserInterface>();
            this.helpCommand = new HelpCommand(userInterfaceMock.Object);

            this.packackageSolutionCommand = new PackageSolutionCommand();
            this.commands = new List<ICommand> { this.packackageSolutionCommand, this.helpCommand };

            ICommandProvider commandProvider = new CommandProvider(this.commands);
            ICommandNameMatcher commandNameMatcher = new CommandNameMatcher();
            ICommandArgumentParser commandArgumentParser = new CommandArgumentParser();
            ICommandArgumentNameMatcher commandArgumentNameMatcher = new CommandArgumentNameMatcher();

            this.commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                commandProvider, commandNameMatcher, commandArgumentParser, commandArgumentNameMatcher);            
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_FullCommandNameIsSupplied_NoArguments_ResultIsThePackageSolutionCommandWithoutArgumentValues()
        {
            // Arrange
            var arguments = new[] { "package" };

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(arguments);

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
            foreach (var key in result.Arguments.Keys)
            {
                Assert.IsNull(result.Arguments[key]);
            }
        }
    }
}
