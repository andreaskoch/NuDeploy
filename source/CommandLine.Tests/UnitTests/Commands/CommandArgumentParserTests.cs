using System;
using System.Linq;

using NUnit.Framework;

using NuDeploy.CommandLine.Infrastructure.Console;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandArgumentParserTests
    {
        private ICommandArgumentParser commandArgumentParser;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.commandArgumentParser = new CommandArgumentParser();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseParameters_ParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            this.commandArgumentParser.ParseParameters(null);
        }

        [Test]
        public void ParseParameters_ParameterIsEmptyArray_ResultIsEmpty()
        {
            // Arrange
            var arguments = new string[] { };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseParameters_ParameterIsArrayWithEmptyStrings_ResultContainsOneEntry()
        {
            // Arrange
            var arguments = new[] { string.Empty };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ParseParameters_SingleUnnamedParameterIsSupplied_NameOfArgumentFollowsFormatOfUnnamedParameters()
        {
            // Arrange
            var arguments = new[] { "randomparameter" };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(string.Empty, result.First().Key);
        }

        [Test]
        public void ParseParameters_SingleDashNamedParameterIsSupplied_ArgumentNameIsReturned()
        {
            // Arrange
            string argumentPrefix = "-";

            string argumentName = "argumentName";
            string argumentValue = "argumentValue";
            string parameter = argumentPrefix + argumentName + "=" + argumentValue;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(argumentName, result.First().Key);
        }

        [Test]
        public void ParseParameters_DoubleDashNamedParameterIsSupplied_ArgumentNameIsReturned()
        {
            // Arrange
            string argumentPrefix = "--";

            string argumentName = "argumentName";
            string argumentValue = "argumentValue";
            string parameter = argumentPrefix + argumentName + "=" + argumentValue;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(argumentName, result.First().Key);
        }

        [Test]
        public void ParseParameters_ForwardSlashNamedParameterIsSupplied_ArgumentNameIsReturned()
        {
            // Arrange
            string argumentPrefix = "/";

            string argumentName = "argumentName";
            string argumentValue = "argumentValue";
            string parameter = argumentPrefix + argumentName + "=" + argumentValue;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(argumentName, result.First().Key);
        }

        [Test]
        public void ParseParameters_SingleDashNamedParameterIsSupplied_ValueIsWrappedInDoubleQuotes_ArgumentNameAndValueIsReturnedCorrectly()
        {
            // Arrange
            string argumentPrefix = "-";
            string quotes = "\"";

            string argumentName = "argumentName";
            string argumentValue = "argumentValue";
            string parameter = argumentPrefix + argumentName + "=" + quotes + argumentValue + quotes;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(argumentName, result.First().Key);
            Assert.AreEqual(argumentValue, result.First().Value);
        }

        [Test]
        public void ParseParameters_SingleDashNamedParameterIsSupplied_ValueIsWrappedInSingleQuotes_ArgumentNameAndValueIsReturnedCorrectly()
        {
            // Arrange
            string argumentPrefix = "-";
            string quotes = "'";

            string argumentName = "argumentName";
            string argumentValue = "argumentValue";
            string parameter = argumentPrefix + argumentName + "=" + quotes + argumentValue + quotes;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(argumentName, result.First().Key);
            Assert.AreEqual(argumentValue, result.First().Value);
        }

        [Test]
        public void ParseParameters_NamedParameterWithoutPrefixIsSupplied_ResultIsUnnamed()
        {
            // Arrange
            string argumentName = "argumentName";
            string argumentValue = "argumentValue";
            string parameter = argumentName + "=" + argumentValue;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(string.Empty, result.First().Key);
            Assert.AreEqual(parameter, result.First().Value);
        }

        [Test]
        public void ParseParameters_MultipleUnnamedParametersAreSupplied_AllUnnamedParametersAreReturned()
        {
            // Arrange
            string arg1 = "arg1";
            string arg2 = "arg2";
            string arg3 = "arg3";

            var arguments = new[] { arg1, arg2, arg3 };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(arguments, result.Select(pair => pair.Value).ToArray());
        }

        [Test]
        public void ParseParameters_MultipleNamedParametersAreSupplied_AllUnnamedParametersAreReturned()
        {
            // Arrange
            string parameter1 = "-argumentName1=argumentValue1";
            string paramter2 = "-argumentName2=argumentValue2";
            string parameter3 = "-argumentName3=argumentValue3";

            var arguments = new[] { parameter1, paramter2, parameter3 };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual("argumentName1", result.Skip(0).Take(1).First().Key);
            Assert.AreEqual("argumentValue1", result.Skip(0).Take(1).First().Value);

            Assert.AreEqual("argumentName2", result.Skip(1).Take(1).First().Key);
            Assert.AreEqual("argumentValue2", result.Skip(1).Take(1).First().Value);

            Assert.AreEqual("argumentName3", result.Skip(2).Take(1).First().Key);
            Assert.AreEqual("argumentValue3", result.Skip(2).Take(1).First().Value);
        }

        [Test]
        public void ParseParameters_SingleSwitchParameterIsSupplied_SwitchNameIsRecognizedAndValueIsTrue()
        {
            // Arrange
            string switchArgumentName = "someSwitch";
            string switchPrefix = "-";

            string parameter = switchPrefix + switchArgumentName;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(switchArgumentName, result.First().Key);
            Assert.AreEqual(bool.TrueString, result.First().Value);
        }

        [Test]
        public void ParseParameters_SingleSwitchParameterWithInvalidPrefixIsSupplied_ResultIsUnnamed()
        {
            // Arrange
            string switchArgumentName = "someSwitch";
            string switchPrefix = "--";

            string parameter = switchPrefix + switchArgumentName;

            var arguments = new[] { parameter };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments).ToList();

            // Assert
            Assert.AreEqual(string.Empty, result.First().Key);
            Assert.AreEqual(parameter, result.First().Value);
        }
    }
}
