using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Cleanup;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Installation.Status;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Update;

namespace CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class CommandProviderTests
    {
        [Test]
        public void Constructor_AllParametersAreSet_ClassIsInstantiated()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            var result = new ConsoleCommandProvider(installationStatus, install, uninstall, cleanup, package, help, configureSources, selfUpdate);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallationStatusCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(null, install, uninstall, cleanup, package, help, configureSources, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_InstallCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, null, uninstall, cleanup, package, help, configureSources, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_UninstallCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, install, null, cleanup, package, help, configureSources, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_CleanupCommandCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, install, uninstall, null, package, help, configureSources, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PackageSolutionCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, install, uninstall, cleanup, null, help, configureSources, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_HelpCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            HelpCommand help = null;
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, install, uninstall, cleanup, package, help, configureSources, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_RepositorySourceConfigurationCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var selfUpdate = new SelfUpdateCommand(new ApplicationInformation(), new Mock<ISelfUpdateService>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, install, uninstall, cleanup, package, help, null, selfUpdate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SelfUpdateCommandIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var installationStatus = new InstallationStatusCommand(new Mock<IUserInterface>().Object, new Mock<IInstallationStatusProvider>().Object);
            var install = new InstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageInstaller>().Object, new Mock<IDeploymentTypeParser>().Object);
            var uninstall = new UninstallCommand(new Mock<IUserInterface>().Object, new Mock<IPackageUninstaller>().Object);
            var cleanup = new CleanupCommand(new Mock<IUserInterface>().Object, new Mock<ICleanupService>().Object);
            var package = new PackageSolutionCommand(new Mock<IUserInterface>().Object, new Mock<ISolutionPackagingService>().Object);
            var help = new HelpCommand(new Mock<IUserInterface>().Object, new ApplicationInformation());
            var configureSources = new RepositorySourceConfigurationCommand(new Mock<IUserInterface>().Object, new Mock<ISourceRepositoryProvider>().Object);

            // Act
            new ConsoleCommandProvider(installationStatus, install, uninstall, cleanup, package, help, configureSources, null);
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
