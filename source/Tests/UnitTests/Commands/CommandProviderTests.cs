using System;
using System.Collections.Generic;

using Moq;

using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandProviderTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Initialize_CommandProvider_CommandListParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            new CommandProvider(null);
        }

        [Test]
        public void GetCommands_EmptyListOfCommandsIsProvided_ResultIsEmptyListOfCommands()
        {
            // Arrange
            ICommandProvider commandProvider = new CommandProvider(new List<ICommand>());

            // Act
            var result = commandProvider.GetAvailableCommands();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetCommands_ListOfCommandsIsProvided_ResultIsSameListOfCommands()
        {
            // Arrange
            var command1 = new Mock<ICommand>();
            var command2 = new Mock<ICommand>();
            var commandList = new List<ICommand> { command1.Object, command2.Object };

            ICommandProvider commandProvider = new CommandProvider(commandList);

            // Act
            var result = commandProvider.GetAvailableCommands();

            // Assert
            Assert.AreEqual(commandList, result);
        }
    }
}
