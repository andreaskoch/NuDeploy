using System;

using Moq;

using NuDeploy.CommandLine.UserInterface;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.UserInterface
{
    [TestFixture]
    public class ConsoleServiceResultVisualizerTests
    {
        private IServiceResultVisualizer serviceResultVisualizer;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.serviceResultVisualizer = new ConsoleServiceResultVisualizer();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Display_UserInterfaceParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            IServiceResult serviceResult = new SuccessResult();

            // Act
            this.serviceResultVisualizer.Display(null, serviceResult);
        }

        [Test]
        public void Display_ServiceResultParameterIsNull_NothingIsWrittenToUserInterface()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            IServiceResult serviceResult = null;

            // Act
            this.serviceResultVisualizer.Display(userInterface.Object, serviceResult);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Display_ServiceResultTypeIsInvalid_NothingIsWrittenToUserInterface()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            IServiceResult serviceResult = new ServiceResult(ServiceResultType.NoResult, string.Empty);

            // Act
            this.serviceResultVisualizer.Display(userInterface.Object, serviceResult);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Display_ServiceResultMessageIsInvalid_NothingIsWrittenToUserInterface(string resultMessage)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            IServiceResult serviceResult = new ServiceResult(ServiceResultType.Success, resultMessage);

            // Act
            this.serviceResultVisualizer.Display(userInterface.Object, serviceResult);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.IsAny<string>()), Times.Never());
        }

        [TestCase(ServiceResultType.Failure)]
        [TestCase(ServiceResultType.Success)]
        public void Display_ServiceResultIsValid_InnerResultIsEmpty_MessageIsWrittenToUserInterface(ServiceResultType resultType)
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            string resultMessage = "Yada Yada";
            IServiceResult serviceResult = new ServiceResult(resultType, resultMessage);

            // Act
            this.serviceResultVisualizer.Display(userInterface.Object, serviceResult);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.Is<string>(message => message.EndsWith(resultMessage))), Times.Once());
        }

        [Test]
        public void Display_ServiceResultIsValid_ContainsResultArtefact_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            string resultArtefact = "Bla Bla";
            IServiceResult serviceResult = new ServiceResult(ServiceResultType.Success, "Yada Yada") { ResultArtefact = resultArtefact };

            // Act
            this.serviceResultVisualizer.Display(userInterface.Object, serviceResult);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.Is<string>(message => message.EndsWith(resultArtefact))), Times.Once());
        }

        [Test]
        public void Display_ServiceResultIsValid_InnerResultIsEmpty_ContainsResultArtefact_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();

            IServiceResult innerResult2 = new ServiceResult(ServiceResultType.Success, Guid.NewGuid().ToString());
            IServiceResult innerResult1 = new ServiceResult(ServiceResultType.Failure, Guid.NewGuid().ToString()) { InnerResult = innerResult2 };
            IServiceResult mainResult = new ServiceResult(ServiceResultType.Success, Guid.NewGuid().ToString()) { InnerResult = innerResult1 };

            // Act
            this.serviceResultVisualizer.Display(userInterface.Object, mainResult);

            // Assert
            userInterface.Verify(u => u.WriteLine(It.Is<string>(message => message.EndsWith(innerResult1.Message))), Times.Once());
            userInterface.Verify(u => u.WriteLine(It.Is<string>(message => message.EndsWith(innerResult2.Message))), Times.Once());
            userInterface.Verify(u => u.WriteLine(It.Is<string>(message => message.EndsWith(mainResult.Message))), Times.Once());
        }
    }
}