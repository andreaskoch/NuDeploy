using System.Collections.Generic;
using System.Linq;

using Moq;

using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests
{
    [TestFixture]
    public class CommandLineArgumentInterpreterTests
    {
        private ICommand packackageSolutionCommand;

        private IEnumerable<ICommand> commands;

        private ICommandLineArgumentInterpreter commandLineArgumentInterpreter;

        private ICommandProvider commandProvider;

        private ICommandNameMatcher commandNameMatcher;

        private ICommandArgumentParser commandArgumentParser;

        private ICommandArgumentNameMatcher commandArgumentNameMatcher;

        [SetUp]
        public void Setup()
        {
            this.packackageSolutionCommand = new PackageSolutionCommand();
            this.commands = new List<ICommand> { this.packackageSolutionCommand };

            this.commandProvider = new CommandProvider(this.commands);
            this.commandNameMatcher = new CommandNameMatcher();
            this.commandArgumentParser = new CommandArgumentParser();
            this.commandArgumentNameMatcher = new CommandArgumentNameMatcher();

            this.commandLineArgumentInterpreter = new CommandLineArgumentInterpreter(
                this.commandProvider, this.commandNameMatcher, this.commandArgumentParser, this.commandArgumentNameMatcher);            
        }

        [Test]
        public void GetCommand_UnknownCommandNameIsSupplied_ResultIsNull()
        {
            // Arrange
            var arguments = new[] { "Jdfklsajdlkjsadlkjasdlö" };

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(arguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_ArternativeCommandNameIsSupplied_ResultIsThePackageSolutionCommand()
        {
            // Arrange
            var arguments = new[] { "pack" };

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(arguments);

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_PartialCommandNameIsSupplied_ResultIsThePackageSolutionCommand()
        {
            // Arrange
            var arguments = new[] { "pa" };

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(arguments);

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
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

        [Test]
        public void GetCommand_MultipleCommandsWithSimilarName_ArgumentIsOnlyPartialCommandNameWhichMatchesMoreThanOneCommand_ResultIsNull()
        {
            // Arrange
            var command1 = new Mock<ICommand>();
            command1.Setup(c => c.Attributes).Returns(new CommandAttributes { CommandName = "abc" });

            var command2 = new Mock<ICommand>();
            command2.Setup(c => c.Attributes).Returns(new CommandAttributes { CommandName = "abcd" });

            var command3 = new Mock<ICommand>();
            command3.Setup(c => c.Attributes).Returns(new CommandAttributes { CommandName = "abcde" });

            var commandList = new List<ICommand> { command1.Object, command2.Object, command3.Object };

            var customCommandProvider = new CommandProvider(commandList);

            var interpreter = new CommandLineArgumentInterpreter(
                customCommandProvider, this.commandNameMatcher, this.commandArgumentParser, this.commandArgumentNameMatcher);

            var arguments = new[] { "abc" };

            // Act
            ICommand result = interpreter.GetCommand(arguments);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_FullCommandNameAndAllArgumentsAreSupplied_SingleDashArguments_NoQuotesForValues_ResultIsThePackageSolutionCommandWithAllArgumentValuesSet()
        {
            // Arrange
            string commandLine = @"package -SolutionPath=C:\dev\project\project.sln -BuildConfiguration=Release -MSBuildProperties=IsAutoBuild=True";

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(commandLine.Split(' '));

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
            foreach (var key in result.Arguments.Keys)
            {
                Assert.IsNotNull(result.Arguments[key]);
            }
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_FullCommandNameAndAllArgumentsAreSupplied_SingleDashArguments_SingleQuotesForValues_ResultIsThePackageSolutionCommandWithAllArgumentValuesSet()
        {
            // Arrange
            string commandLine = @"package -SolutionPath='C:\dev\project\project.sln' -BuildConfiguration='Release' -MSBuildProperties='IsAutoBuild=True'";

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(commandLine.Split(' '));

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
            foreach (var key in result.Arguments.Keys)
            {
                Assert.IsNotNull(result.Arguments[key]);
            }
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_FullCommandNameAndAllArgumentsAreSupplied_SingleDashArguments_DoubleQuotesForValues_ResultIsThePackageSolutionCommandWithAllArgumentValuesSet()
        {
            // Arrange
            string commandLine = "package -SolutionPath=\"C:\\dev\\project\\project.sln' -BuildConfiguration=\"Release\" -MSBuildProperties=\"IsAutoBuild=True\"";

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(commandLine.Split(' '));

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
            foreach (var key in result.Arguments.Keys)
            {
                Assert.IsNotNull(result.Arguments[key]);
            }
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_UnmappableNamedArgumentIsSupplied_ResultIsThePackageSolutionCommandWithAdditionalArgument()
        {
            // Arrange
            string commandLine = @"package -UnmappableArgumentName=UnmappableArgumentValue";

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(commandLine.Split(' '));

            // Assert: Package Command is returned
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);

            // Assert: Unmappable argument is added to argument list
            Assert.IsTrue(result.Arguments.Keys.Contains("UnmappableArgumentName"));
            Assert.AreEqual("UnmappableArgumentValue", result.Arguments["UnmappableArgumentName"]);
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_UnmappableUnnamedArgumentIsSupplied_ResultIsThePackageSolutionCommandWithAdditionalUnnamedArgument()
        {
            // Arrange
            string commandLine = @"package SomeValueWithoutAnArgumentName";

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(commandLine.Split(' '));

            // Assert: Package Command is returned
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);

            // Assert: Unnamed argument is added to the argument list
            Assert.IsTrue(result.Arguments.Values.Contains("SomeValueWithoutAnArgumentName"));
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_FullCommandNameIsSupplied_AllArguments_ResultIsThePackageSolutionCommandWithAllArgumentValues()
        {
            // Arrange
            string commandName = "package";

            string argumentPrefix = "-";
            string argumentValueAssignmentOperator = "=";

            string argument1Name = "SolutionPath";
            string argument1Value = @"C:\dev\project\project.sln";
            string argument1 = argumentPrefix + argument1Name + argumentValueAssignmentOperator + argument1Value;

            string argument2Name = "BuildConfiguration";
            string argument2Value = @"Release";
            string argument2 = argumentPrefix + argument2Name + argumentValueAssignmentOperator + argument2Value;

            string argument3Name = "MSBuildProperties";
            string argument3Value = @"IsAutoBuild=True";
            string argument3 = argumentPrefix + argument3Name + argumentValueAssignmentOperator + argument3Value;

            var arguments = new[] { commandName, argument1, argument2, argument3 };

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(arguments);

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
            Assert.AreEqual(argument1Value, result.Arguments[argument1Name]);
            Assert.AreEqual(argument2Value, result.Arguments[argument2Name]);
            Assert.AreEqual(argument3Value, result.Arguments[argument3Name]);
        }

        [Test]
        public void GetCommand_PackageSolutionCommand_FullCommandNameIsSupplied_PositionalArguments_ResultIsThePackageSolutionCommandWithAllArgumentValues()
        {
            // Arrange
            string commandName = "package";

            string argument1 = @"C:\dev\project\project.sln";
            string argument2 = "Release";
            string argument3 = "IsAutoBuild=True";

            var arguments = new[] { commandName, argument1, argument2, argument3 };

            // Act
            ICommand result = this.commandLineArgumentInterpreter.GetCommand(arguments);

            // Assert
            Assert.AreEqual(this.packackageSolutionCommand.Attributes.CommandName, result.Attributes.CommandName);
            Assert.AreEqual(argument1, result.Arguments.Values.ToArray()[0]);
            Assert.AreEqual(argument2, result.Arguments.Values.ToArray()[1]);
            Assert.AreEqual(argument3, result.Arguments.Values.ToArray()[2]);
        }
    }
}
