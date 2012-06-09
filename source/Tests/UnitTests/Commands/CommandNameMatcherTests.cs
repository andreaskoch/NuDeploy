using System;

using Moq;

using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandNameMatcherTests
    {
        private ICommandNameMatcher commandNameMatcher;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.commandNameMatcher = new CommandNameMatcher();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsMatch_SuppliedCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            ICommand command = null;
            string commandName = "update";

            // Act
            this.commandNameMatcher.IsMatch(command, commandName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsMatch_SuppliedCommandNameIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var command = new Mock<ICommand>();
            string commandName = null;

            // Act
            this.commandNameMatcher.IsMatch(command.Object, commandName);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameHasNoModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "update";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_ButHasDifferentCase_CommandNameHasNoModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "UPDATE";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameHasNoModifier_ButWhiteSpaceAtTheEnd_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "update  ";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameHasNoModifier_ButWhiteSpaceAtTheBeginning_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "  update";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameIsSubsetOfTheCommandName_CommandNameHasNoModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "up";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameStartsWithDashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "-update";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_ButHasDifferentCase_CommandNameStartsWithDashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "-UPDATE";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameIsSubsetOfTheCommandName_CommandNameStartsWithDashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "-up";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameStartsWithDoubleDashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "--update";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_ButHasDifferentCase_CommandNameStartsWithDoubleDashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "--UPDATE";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameIsSubsetOfTheCommandName_CommandNameStartsWithDoubleDashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "--up";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameStartsWithForwardSlashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "/update";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_ButHasDifferentCase_CommandNameStartsWithForwardSlashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "/UPDATE";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameIsSubsetOfTheCommandName_CommandNameStartsWithForwardSlashModifier_ResultIsTrue()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "/up";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameMatchesTheNameOfTheCommand_CommandNameStartsWithInvalidModifier_ResultIsFalse()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "?update";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatch_SuppliedCommandNameDoesNotMatchTheNameOfTheCommand_ResultIsFalse()
        {
            // Arrange
            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(
                new CommandAttributes
                {
                    CommandName = "update",
                    AlternativeCommandNames = new string[] { }
                });

            string commandName = "delete";

            // Act
            bool result = this.commandNameMatcher.IsMatch(command.Object, commandName);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
