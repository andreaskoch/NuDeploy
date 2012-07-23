using System;
using System.Linq;

using NuDeploy.CommandLine.Commands;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class BuildPropertyParserTests
    {
        private IBuildPropertyParser buildPropertyParser;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.buildPropertyParser = new BuildPropertyParser();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string buildProperties = null;

            // Act
            this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties);
        }

        [TestCase("")]
        [TestCase(" ")]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterIsEmpty_ResultIsEmptyList(string buildProperties)
        {
            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestCase("Parameter1:Value1")]
        [TestCase("Parameter1>Value1")]
        [TestCase("Parameter1=>Value1=True")]
        [TestCase("Parameter1:Value1,Parameter2 Value2")]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterHasInvalidSyntax_ResultIsEmptyList(string buildProperties)
        {
            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestCase("Parameter1=Value1;Parameter2=Value2")]
        [TestCase("Parameter1=Value1 Parameter2=Value2")]
        [TestCase("Parameter1=Value1/Parameter2=Value2")]
        [TestCase("Parameter1=Value1\\Parameter2=Value2")]
        [TestCase("Parameter1=Value1?Parameter2=Value2")]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterHasInvalidMultiValueSeperator_ResultIsEmptyList(string buildProperties)
        {
            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestCase("Parameter1=Value1", "Parameter1", "Value1")]
        [TestCase("Parameter1=", "Parameter1", "")]
        [TestCase("P1=V1", "P1", "V1")]
        [TestCase("1=1", "1", "1")]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterHasOneKeyValuePair_ResultContainsOneKeyValuePair(string buildProperties, string expectedKey, string expectedValue)
        {
            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties).ToList();

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(expectedKey, result.First().Key);
            Assert.AreEqual(expectedValue, result.First().Value);
        }

        [Test]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterHasMultipleKeyValuePairs_ResultNonEmptyList()
        {
            // Arrange
            string buildProperties = "Parameter1=Value1,Parameter2=Value2";

            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties).ToList();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void ParseBuildPropertiesArgument_BuildPropertiesParameterHasMultipleKeyValuePairsWithWhitespace_ResultNonEmptyList()
        {
            // Arrange
            string buildProperties = "Parameter1=Value1,Parameter2=Value2";

            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties).ToList();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void ParseBuildPropertiesArgument_BuildPropertyKeyContainsWhitespace_WhitespaceIsTrimmed()
        {
            // Arrange
            string buildProperties = " Parameter1 =Value1";

            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties).ToList();

            // Assert
            Assert.AreEqual("Parameter1", result.First().Key);
        }

        [Test]
        public void ParseBuildPropertiesArgument_BuildPropertyValueContainsWhitespace_WhitespaceIsTrimmed()
        {
            // Arrange
            string buildProperties = "Parameter1= Value1 ";

            // Act
            var result = this.buildPropertyParser.ParseBuildPropertiesArgument(buildProperties).ToList();

            // Assert
            Assert.AreEqual("Value1", result.First().Value);
        }
    }
}