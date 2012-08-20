using System;

using Moq;

using NuDeploy.CommandLine.UserInterface;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;

using NUnit.Framework;

using NuDeploy.Core.Services;

namespace NuDeploy.CommandLine.Tests.UnitTests.UserInterface
{
    [TestFixture]
    public class ConsoleUserInterfaceTests
    {
        #region Constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            // Act
            var result = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ConsoleTextManipulationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            // Act
            new ConsoleUserInterface(null, logger.Object, serviceResultVisualizer.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ActionLoggerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var textManipulation = new Mock<IConsoleTextManipulation>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            // Act
            new ConsoleUserInterface(textManipulation.Object, null, serviceResultVisualizer.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ServiceResultVisualizerParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();

            // Act
            new ConsoleUserInterface(textManipulation.Object, logger.Object, null);
        }

        #endregion

        #region WindowWidth Property

        [Test]
        public void WindowWidth_PropertyIsNotZero()
        {
            // Arrange
            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            var result = consoleUserInterface.WindowWidth;

            // Assert
            Assert.AreNotEqual(0, result);
            Assert.IsTrue(result > 0);
        }

        [Test]
        public void WindowWidth_PropertyCanBeChanged()
        {
            // Arrange
            int newWindowWidth = 313;

            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);
            int oldWindowWidth = consoleUserInterface.WindowWidth;

            // Act
            consoleUserInterface.WindowWidth = newWindowWidth;
            var result = consoleUserInterface.WindowWidth;

            // Assert
            Assert.AreNotEqual(oldWindowWidth, result);
            Assert.AreEqual(newWindowWidth, result);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WindowWidth_SetToInvalidValue_ArgumentOutOfRangeExceptionIsThrown(int newWindowWidth)
        {
            // Arrange
            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            consoleUserInterface.WindowWidth = newWindowWidth;
        }

        #endregion

        #region UserInterfaceContent Property

        [Test]
        public void UserInterfaceContent_IsEmptyWhenNothingIsWrittenToTheUserInterface()
        {
            // Arrange
            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            var result = consoleUserInterface.UserInterfaceContent;
            
            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void UserInterfaceContent_ContainsTextThatIsWrittenUsing_WriteLine()
        {
            // Arrange
            string text = "Sample Text";

            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            consoleUserInterface.WriteLine(text);
            var result = consoleUserInterface.UserInterfaceContent;

            // Assert
            Assert.IsTrue(result.Contains(text));
        }

        [Test]
        public void UserInterfaceContent_ContainsEverythingThatIsWrittenUsing_WriteLine()
        {
            // Arrange
            string text1 = "Sample Text 1";
            string text2 = "Sample Text 2";

            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            consoleUserInterface.WriteLine(text1);
            consoleUserInterface.WriteLine(text2);
            var result = consoleUserInterface.UserInterfaceContent;

            // Assert
            Assert.IsTrue(result.Contains(text1));
            Assert.IsTrue(result.Contains(text2));
        }

        [Test]
        public void UserInterfaceContent_ContainsEverythingThatIsWrittenUsing_Write()
        {
            // Arrange
            var textFragments = new[] { "A", "B", "C", "D" };

            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            foreach (var textFragment in textFragments)
            {
                consoleUserInterface.Write(textFragment);
            }

            // Assert
            Assert.AreEqual(string.Join(string.Empty, textFragments), consoleUserInterface.UserInterfaceContent);
        }

        [Test]
        public void UserInterfaceContent_ContainsTextThatIsWrittenUsing_ShowIndented()
        {
            // Arrange
            string text = "Sample Text";

            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new Mock<IServiceResultVisualizer>();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer.Object);

            // Act
            consoleUserInterface.ShowIndented(text, 2);
            var result = consoleUserInterface.UserInterfaceContent;

            // Assert
            Assert.IsTrue(result.Contains(text));
        }

        [Test]
        public void UserInterfaceContent_ContainsTextThatIsWrittenUsing_Display_ServiceResult()
        {
            // Arrange
            string text = "Some Message";
            var serviceResult = new ServiceResult(ServiceResultType.Success, text);

            var textManipulation = new Mock<IConsoleTextManipulation>();
            var logger = new Mock<IActionLogger>();
            var serviceResultVisualizer = new ConsoleServiceResultVisualizer();

            var consoleUserInterface = new ConsoleUserInterface(textManipulation.Object, logger.Object, serviceResultVisualizer);

            // Act
            consoleUserInterface.Display(serviceResult);
            var result = consoleUserInterface.UserInterfaceContent;

            // Assert
            Assert.IsTrue(result.Contains(text));
        }

        #endregion
    }
}