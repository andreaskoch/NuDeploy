using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands.Console
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
            var publishingService = new Mock<IPublishingService>();

            // Act
            var packageSolutionCommand = new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();

            // Act
            var packageSolutionCommand = new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();

            // Act
            new PackageSolutionCommand(null, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SolutionPackagingServiceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            new PackageSolutionCommand(userInterface.Object, null, buildPropertyParser.Object, publishingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_BuildPropertyParserParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var publishingService = new Mock<IPublishingService>();

            // Act
            new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, null, publishingService.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishingServiceParametersIsNotSet_ArgumentNullException()
        {
            // Arrange
            var userInterface = new Mock<IUserInterface>();
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            // Act
            new PackageSolutionCommand(userInterface.Object, solutionPackagingService.Object, buildPropertyParser.Object, null);
        }

        #endregion

        #region Execute - SolutionPath Tests

        [Test]
        public void Execute_SolutionPathArgumentIsNotSet_PackageSolutionIsNotExecuted()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>()), Times.Never());
        }

        [Test]
        public void Execute_SolutionPathArgumentIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_SolutionPathArgumentIsInvalid_MessageIsWrittenToUserInterface(string solutionPath)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>()), Times.Never());
        }

        [Test]
        public void Execute_BuildConfigurationArgumentIsNotSet_MessageIsWrittenToUserInterface()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_BuildConfigurationArgumentIsInvalid_MessageIsWrittenToUserInterface(string buildConfiguration)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();

            solutionPackagingService.Setup(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();

            solutionPackagingService.Setup(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
            var publishingService = new Mock<IPublishingService>();

            solutionPackagingService.Setup(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            solutionPackagingService.Verify(
                p => p.PackageSolution(solutionPath, buildConfiguration, It.Is<KeyValuePair<string, string>[]>(d => d.Any() == false)), Times.Once());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_MSBuildPropertiesArgumentIsInvalid_PackageSolutionIsCalledWithEmptyListOfBuildProperties(string buildPropertiesArgument)
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();

            solutionPackagingService.Setup(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
                p => p.PackageSolution(solutionPath, buildConfiguration, It.Is<KeyValuePair<string, string>[]>(d => d.Any() == false)), Times.Once());
        }

        [Test]
        public void Execute_MSBuildPropertiesArgumentContainsTwoValues_PackageSolutionIsCalledWithEmptyListOfTwoBuildProperties()
        {
            // Arrange
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();

            solutionPackagingService.Setup(s => s.PackageSolution(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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
                p.PackageSolution(solutionPath, buildConfiguration, It.Is<KeyValuePair<string, string>[]>(d => d.Count() == buildProperties.Count)),
                Times.Once());
        }

        #endregion

        #region Execute - Package Tests

        [Test]
        public void Execute_PackagingFails_ResultIsFalse()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new FailureResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            var result = packageSolutionCommand.Execute();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Execute_PackagingFails_FailureMessageIsWrittenToUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            string failureMessage = "Packaging failed due to yada yada. " + Guid.NewGuid().ToString();
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new FailureResult(failureMessage));

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(failureMessage),
                "The user interface should contain the failure message \"{0}\" but contains this \"{1}\"",
                failureMessage,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PackagingSucceeds_ResultIsTrue()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new SuccessResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            var result = packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Execute_PackagingSucceeds_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new SuccessResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

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

        #region Execute - Publish Tests

        [Test]
        public void Execute_PublishConfigurationIsNotSet_PackageIsNotPublished()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new SuccessResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Execute_PublishConfigurationIsInvalid_PackageIsNotPublished(string publishConfigurationName)
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new SuccessResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Execute_PublishConfigurationIsSet_PublishIsExecuted()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";
            string publishConfigurationName = "Some Publish Config";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new SuccessResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();
            var publishingService = new Mock<IPublishingService>();
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), publishConfigurationName)).Returns(new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            publishingService.Verify(p => p.PublishPackage(It.IsAny<string>(), publishConfigurationName), Times.Once());
        }

        [Test]
        public void Execute_PublishingFails_FailureResultIsWrittenTouUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";
            string publishConfigurationName = "Some Publish Config";

            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(new SuccessResult());

            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            var publishingService = new Mock<IPublishingService>();
            string publishingFailureMessage = "Publishing failed. yada yada. " + Guid.NewGuid().ToString();
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new FailureResult(publishingFailureMessage));


            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(publishingFailureMessage),
                "The user interface should contain the failure message \"{0}\" but contains this \"{1}\"",
                publishingFailureMessage,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PublishingFails_FailureMessageIsWrittenTouUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";
            string publishConfigurationName = "Some Publish Config";

            string packagePath = @"C:\some-package.nupkg";
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult { ResultArtefact = packagePath });

            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            var publishingService = new Mock<IPublishingService>();
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new FailureResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(packagePath)
                && this.loggingUserInterface.UserInterfaceOutput.Contains(publishConfigurationName),
                "The user interface should contain the package path \"{0}\" and the publish configuration name \"{1}\" but contains this \"{2}\"",
                packagePath,
                publishConfigurationName,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        [Test]
        public void Execute_PublishingSucceeds_SuccessMessageIsWrittenTouUserInterface()
        {
            // Arrange
            string solutionPath = @"C:\dev\project-xy\solution.sln";
            string buildConfiguration = "Debug";
            string publishConfigurationName = "Some Publish Config";

            string packagePath = @"C:\some-package.nupkg";
            var solutionPackagingService = new Mock<ISolutionPackagingService>();
            solutionPackagingService.Setup(s => s.PackageSolution(solutionPath, buildConfiguration, It.IsAny<KeyValuePair<string, string>[]>())).Returns(
                new SuccessResult { ResultArtefact = packagePath });

            var buildPropertyParser = new Mock<IBuildPropertyParser>();

            var publishingService = new Mock<IPublishingService>();
            publishingService.Setup(p => p.PublishPackage(It.IsAny<string>(), It.IsAny<string>())).Returns(new SuccessResult());

            var packageSolutionCommand = new PackageSolutionCommand(this.loggingUserInterface.UserInterface, solutionPackagingService.Object, buildPropertyParser.Object, publishingService.Object);

            // prepare command arguments
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameSolutionPath, solutionPath);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNameBuildConfiguration, buildConfiguration);
            packageSolutionCommand.Arguments.Add(PackageSolutionCommand.ArgumentNamePublishingConfiguration, publishConfigurationName);

            // Act
            packageSolutionCommand.Execute();

            // Assert
            Assert.IsTrue(
                this.loggingUserInterface.UserInterfaceOutput.Contains(packagePath)
                && this.loggingUserInterface.UserInterfaceOutput.Contains(publishConfigurationName),
                "The user interface should contain the package path \"{0}\" and the publish configuration name \"{1}\" but contains this \"{2}\"",
                packagePath,
                publishConfigurationName,
                this.loggingUserInterface.UserInterfaceOutput);
        }

        #endregion
    }
}