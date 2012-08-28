using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Moq;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Cleanup;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Installation.Status;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Publishing;
using NuDeploy.Core.Services.Update;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandProviderTests
    {
        private IUserInterface userInterface;

        private InstallationStatusCommand installationStatus;

        private InstallCommand install;

        private UninstallCommand uninstall;

        private CleanupCommand cleanup;

        private PackageSolutionCommand packageSolution;

        private RepositorySourceConfigurationCommand configureSources;

        private PublishingTargetConfigurationCommand configurePublishingTargets;

        private SelfUpdateCommand selfUpdate;

        private PublishCommand publishCommand;

        private HelpCommand help;

        private PackageBuildOutputCommand packageBuildOutput;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.userInterface = new Mock<IUserInterface>().Object;
            this.installationStatus = new InstallationStatusCommand(this.userInterface, new Mock<IInstallationStatusProvider>().Object);
            this.install = new InstallCommand(this.userInterface, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            this.uninstall = new UninstallCommand(this.userInterface, new Mock<IPackageUninstaller>().Object);
            this.cleanup = new CleanupCommand(this.userInterface, new Mock<ICleanupService>().Object);
            this.packageSolution = new PackageSolutionCommand(this.userInterface, new Mock<ISolutionPackagingService>().Object, new Mock<IBuildPropertyParser>().Object, new Mock<IPublishingService>().Object);
            this.packageBuildOutput = new PackageBuildOutputCommand(this.userInterface, new Mock<IBuildOutputPackagingService>().Object, new Mock<IPublishingService>().Object);
            this.configureSources = new RepositorySourceConfigurationCommand(this.userInterface, new Mock<IRepositoryConfigurationCommandActionParser>().Object, new Mock<ISourceRepositoryProvider>().Object);
            this.configurePublishingTargets = new PublishingTargetConfigurationCommand(this.userInterface, new Mock<IPublishingTargetConfigurationCommandActionParser>().Object, new Mock<IPublishConfigurationAccessor>().Object);
            this.selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object, new Mock<_Assembly>().Object);
            this.publishCommand = new PublishCommand(this.userInterface, new Mock<IPublishingService>().Object);
            this.help = new HelpCommand(new Mock<IHelpProvider>().Object);            
        }

        [Test]
        public void Constructor_AllParametersAreSet_ClassIsInstantiated()
        {
            // Act
            var result = new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            new ConsoleCommandProvider(
                null,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                null,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UninstallCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                null,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CleanupCommandCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                null,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageSolutionCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                null,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageBuildOutputCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                null,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_RepositorySourceConfigurationCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                null,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishingTargetConfigurationCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                null,
                this.selfUpdate,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SelfUpdateCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                null,
                this.publishCommand,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                null,
                this.help);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HelpCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            new ConsoleCommandProvider(
                this.installationStatus,
                this.install,
                this.uninstall,
                this.cleanup,
                this.packageSolution,
                this.packageBuildOutput,
                this.configureSources,
                this.configurePublishingTargets,
                this.selfUpdate,
                this.publishCommand,
                null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CommandListConstructor_CommandListParameterIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            new ConsoleCommandProvider(null);
        }

        [Test]
        public void GetCommands_EmptyListOfCommandsIsProvided_ResultIsEmptyListOfCommands()
        {
            // Arrange
            ICommandProvider commandProvider = new ConsoleCommandProvider(new List<ICommand>());

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

            ICommandProvider commandProvider = new ConsoleCommandProvider(commandList);

            // Act
            var result = commandProvider.GetAvailableCommands();

            // Assert
            Assert.AreEqual(commandList, result);
        }
    }
}
