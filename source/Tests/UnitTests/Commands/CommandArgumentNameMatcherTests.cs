using System;

using NuDeploy.Core.Commands;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandArgumentNameMatcherTests
    {
        private ICommandArgumentNameMatcher commandArgumentNameMatcher = new CommandArgumentNameMatcher();

        [TestFixtureSetUp]
        public void Setup()
        {
            this.commandArgumentNameMatcher = new CommandArgumentNameMatcher();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsMatch_FullArgumentNameParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string realArgumentName = null;
            string suppliedArgumentName = "SolutionPath";

            // Act
            this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsMatch_SuppliedArgumentNameParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = null;

            // Act
            this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameIsIdentificalToTarget_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameStartWithSingleDash_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "-SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameStartWithDoubleDash_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "--SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameStartWithSlash_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "/SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameContainsWhitespaceAtTheEnd_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "SolutionPath ";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameContainsWhitespaceAtTheStart_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = " SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameHasDifferentCase_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "solutionpath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameStartWithDoubleSlash_ResultIsFalse()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "//SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameStartWithBackSlash_ResultIsFalse()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "\\SolutionPath";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameIsSubstringFromTheBeginning_ResultIsTrue()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "Solution";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameIsSubstringFromTheEnd_ResultIsFalse()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "Path";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMatch_SuppliedArgumentNameIsCompletelyDifferent_ResultIsFalse()
        {
            // Arrange
            string realArgumentName = "SolutionPath";
            string suppliedArgumentName = "yadayada";

            // Act
            bool result = this.commandArgumentNameMatcher.IsMatch(realArgumentName, suppliedArgumentName);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
