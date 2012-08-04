using System;

using NuDeploy.CommandLine.UserInterface;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.UserInterface
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

        #region WrapText

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
        public void WrapText_MultiLineWithoutWhitespace_ResultWrappedAtMaxLengthText()
        {
            // Arrange
            string text = "LoremIpsumDolorSitAmetConseteturSadipscing";
            int maxWidth = 20;

            // Act
            string result = this.consoleTextManipulation.WrapText(text, maxWidth);

            // Assert
            string expectedResult = "LoremIpsumDolorSitAm" + Environment.NewLine + "etConseteturSadipsci" + Environment.NewLine + "ng";
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void WrapText_MultiLine_TextIsAlreadyWrappedProperly_ResultIsAsInput()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet" + Environment.NewLine + " consetetur sadipscing";
            int maxWidth = 30;

            // Act
            string result = this.consoleTextManipulation.WrapText(text, maxWidth);

            // Assert
            Assert.AreEqual(text, result);
        }

        [Test]
        public void WrapText_MultiLine_TextIsAlreadyWrapped_ButNotProperly_ResultIsWrappedProperly()
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet" + Environment.NewLine + " consetetur sadipscing";
            int maxWidth = 15;

            // Act
            string result = this.consoleTextManipulation.WrapText(text, maxWidth);

            // Assert
            string expectedResult = "Lorem ipsum" + Environment.NewLine + "dolor sit amet" + Environment.NewLine + " consetetur" + Environment.NewLine + "sadipscing";
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void WrapText_InvalidMaxWidth_TextIsReturnedAsIs(int maxWidth)
        {
            // Arrange
            string text = "Lorem ipsum dolor sit amet consetetur sadipscing";

            // Act
            string result = this.consoleTextManipulation.WrapText(text, maxWidth);

            // Assert
            Assert.AreEqual(text, result);
        }

        #endregion

        #region IndentText
        
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

        #endregion

        #region WrapLongTextWithHangingIndentation

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

        #endregion
    }
}
