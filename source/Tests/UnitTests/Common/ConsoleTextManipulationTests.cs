using System;

using NuDeploy.Core.Common;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Common
{
    [TestFixture]
    public class ConsoleTextManipulationTests
    {
        private IConsoleTextManipulation consoleTextManipulation;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.consoleTextManipulation = new ConsoleTextManipulation();
        }

        [Test]
        public void WrapText_SingleLine_ResultEqualsInput()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet consetetur sadipscing";
            int maxWidth = 70;

            // Act
            string result = this.consoleTextManipulation.WrapText(text, maxWidth);

            // Assert
            Assert.AreEqual(text, result);
        }

        [Test]
        public void WrapText_MultiLine_ResultWrappedText()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet consetetur sadipscing";
            int maxWidth = 30;

            // Act
            string result = this.consoleTextManipulation.WrapText(text, maxWidth);

            // Assert
            string expectedResult = "Lorem ipsum dolor sit amet" + Environment.NewLine + "consetetur sadipscing";
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void IndentText_SingleLine_ResultIsTextWithWhitespaceAtTheBeginning()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet, consete";
            int maxWidth = 60;
            int indentation = 4;

            // Act
            string result = this.consoleTextManipulation.IndentText(text, maxWidth, indentation);

            // Assert
            Assert.AreEqual("    Lorem ipsum dolor sit amet, consete", result);
        }

        [Test]
        public void IndentText_MultiLine_ResultIsTextWithWhitespaceAtTheBeginning()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet consetetur sadipscing";
            int maxWidth = 30;
            int indentation = 4;

            // Act
            string result = this.consoleTextManipulation.IndentText(text, maxWidth, indentation);

            // Assert
            string indentationString = new string(' ', indentation);
            string expectedResult = indentationString + "Lorem ipsum dolor sit" + Environment.NewLine + indentationString + "amet consetetur sadipscing";
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void WrapLongTextWithHangingIndentation_SingleLine_ResultIsLikeInput()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet, consete";
            int maxWidth = 60;
            int indentation = 4;

            // Act
            string result = this.consoleTextManipulation.WrapLongTextWithHangingIndentation(text, maxWidth, indentation);

            // Assert
            Assert.AreEqual(text, result);
        }

        [Test]
        public void WrapLongTextWithHangingIndentation_MultiLine_ResultIsLikeInput()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet consetetur sadipscing";
            int maxWidth = 30;
            int indentation = 4;

            // Act
            string result = this.consoleTextManipulation.WrapLongTextWithHangingIndentation(text, maxWidth, indentation);

            // Assert
            string indentationString = new string(' ', indentation);
            string expectedResult = "Lorem ipsum dolor sit" + Environment.NewLine + indentationString + "amet consetetur sadipscing";
            Assert.AreEqual(expectedResult, result);
        }
    }
}
