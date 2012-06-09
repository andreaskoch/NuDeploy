using System;
using System.Linq;

using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Commands
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ParseParameters_ParameterIsArrayWithEmptyStrings_ResultContainsOneEntry()
        {
            // Arrange
            var arguments = new[] { string.Empty };

            // Act
            var result = this.commandArgumentParser.ParseParameters(arguments);

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
            Assert.AreEqual(string.Format(CommandArgumentParser.NameFormatUnnamedParamters, 1), result.Keys.First());
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
            Assert.AreEqual(argumentName, result.Keys.First());
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
            Assert.AreEqual(argumentName, result.Keys.First());
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
            Assert.AreEqual(argumentName, result.Keys.First());
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(argumentName, result.Keys.First());
            Assert.AreEqual(argumentValue, result.Values.First());
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(argumentName, result.Keys.First());
            Assert.AreEqual(argumentValue, result.Values.First());
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(string.Format(CommandArgumentParser.NameFormatUnnamedParamters, 1), result.Keys.First());
            Assert.AreEqual(parameter, result.Values.First());
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
            Assert.AreEqual(arguments, result.Values.ToArray());
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual("argumentName1", result.Keys.Skip(0).Take(1).First());
            Assert.AreEqual("argumentValue1", result.Values.Skip(0).Take(1).First());

            Assert.AreEqual("argumentName2", result.Keys.Skip(1).Take(1).First());
            Assert.AreEqual("argumentValue2", result.Values.Skip(1).Take(1).First());

            Assert.AreEqual("argumentName3", result.Keys.Skip(2).Take(1).First());
            Assert.AreEqual("argumentValue3", result.Values.Skip(2).Take(1).First());
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(switchArgumentName, result.Keys.First());
            Assert.AreEqual(bool.TrueString, result.Values.First());
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
            var result = this.commandArgumentParser.ParseParameters(arguments);

            // Assert
            Assert.AreEqual(string.Format(CommandArgumentParser.NameFormatUnnamedParamters, 1), result.Keys.First());
            Assert.AreEqual(parameter, result.Values.First());
        }
    }
}
