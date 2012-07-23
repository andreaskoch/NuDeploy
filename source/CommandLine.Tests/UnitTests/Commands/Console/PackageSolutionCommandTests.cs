using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Packaging;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class PackageSolutionCommandTests
    {
        private LoggingUserInterface loggingUserInterface;

        [SetUp]
        public void BeforeEachTest()
        {
            this.loggingUserInterface = new LoggingUserInterface();
        }

        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            // Act
            var packageSolutionCommand = new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, buildPropertyParser.Object);

            // Assert
            Assert.IsNotNull(packageSolutionCommand);
        }

        [Test]
        public void Constructor_CommandAttributesAreInitializedProperly()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            // Act
            var packageSolutionCommand = new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, buildPropertyParser.Object);

            // Assert
            CommandTestUtilities.ValidateCommandAttributes(packageSolutionCommand.Attributes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UserInterfaceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            // Act
            new PackageSolutionCommand(null, solutionPackagingService.Object, buildPropertyParser.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SolutionPackagingServiceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            // Act
            new PackageSolutionCommand(userInterface.Object, null, buildPropertyParser.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildPropertyParserParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var solutionPackagingService = new Mock<ISolutionPackagingService>();

            // Act
            new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, null);
        }

        #endregion

        #region Execute - SolutionPath Tests

        [Test]
        public void Execute_SolutionPathArgumentIsNotSet_PackageSolutionIsNotExecuted()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never());
        }

        [Test]
        public void Execute_SolutionPathArgumentIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_SolutionPathArgumentIsInvalid_PackageSolutionIsNotExecuted(string solutionPath)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_SolutionPathArgumentIsInvalid_MessageIsWrittenToUserInterface(string solutionPath)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion

        #region Execute - BuildConfiguration Tests

        [Test]
        public void Execute_BuildConfigurationArgumentIsNotSet_PackageSolutionIsNotExecuted()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never());
        }

        [Test]
        public void Execute_BuildConfigurationArgumentIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_BuildConfigurationArgumentIsInvalid_PackageSolutionIsNotExecuted(string buildConfiguration)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_BuildConfigurationArgumentIsInvalid_MessageIsWrittenToUserInterface(string buildConfiguration)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsNotNullOrEmpty(this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion

        #region Execute - MSBuildProperties

        [Test]
        public void Execute_MSBuildPropertiesArgumentIsNotSet_ParseBuildPropertiesArgumentIsNotCalled()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            buildPropertyParser.Verify(b => b.ParseBuildPropertiesArgument(It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_MSBuildPropertiesArgumentIsInvalid_ParseBuildPropertiesArgumentIsNotCalled(string buildPropertiesArgument)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameMSBuildProperties, buildPropertiesArgument);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            buildPropertyParser.Verify(b => b.ParseBuildPropertiesArgument(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_MSBuildPropertiesArgumentIsNotSet_PackageSolutionIsCalledWithEmptyListOfBuildProperties()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(
                p => p.PackageSolution(solutionPath, buildConfiguration, It.Is<IEnumerable<KeyValuePair<string, string>>>(d => d.Any() == false)), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_MSBuildPropertiesArgumentIsInvalid_PackageSolutionIsCalledWithEmptyListOfBuildProperties(string buildPropertiesArgument)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameMSBuildProperties, buildPropertiesArgument);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(
                p => p.PackageSolution(solutionPath, buildConfiguration, It.Is<IEnumerable<KeyValuePair<string, string>>>(d => d.Any() == false)), Times.Once());
        }

        [Test]
        public void Execute_MSBuildPropertiesArgumentContainsTwoValues_PackageSolutionIsCalledWithEmptyListOfTwoBuildProperties()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var buildProperties = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("IsAutoBuild", "True"),
                    new KeyValuePair<string, string>("SomeOtherParam", "Value")
                };

            string buildPropertiesArgument = string.Join(
                NuDeployConstants.MultiValueSeperator.ToString(CultureInfo.InvariantCulture),
                buildProperties.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)));

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameMSBuildProperties, buildPropertiesArgument);

            buildPropertyParser.Setup(b => b.ParseBuildPropertiesArgument(buildPropertiesArgument)).Returns(buildProperties);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(
                p =>
                p.PackageSolution(solutionPath, buildConfiguration, It.Is<IEnumerable<KeyValuePair<string, string>>>(d => d.Count() == buildProperties.Count)),
                Times.Once());
        }

        [Test]
        public void Execute_PackagingFails_FailIsWrittenToUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<IEnumerable<KeyValuePair<string, string>>>())).Returns(false);

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(this.loggingUserInterface.UserInterfaceOutput.ToLower().Contains("fail"));
        }

        [Test]
        public void Execute_PackagingSucceeds_SucceessIsWrittenToUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<IEnumerable<KeyValuePair<string, string>>>())).Returns(true);

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.ToLower().Contains("succeeded")
                || this.loggingUserInterface.UserInterfaceOutput.ToLower().Contains("success"));
        }

        #endregion
    }
}