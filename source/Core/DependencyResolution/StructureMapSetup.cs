using System;
using System.Collections.Generic;
using System.Management.Automation.Host;
using System.Reflection;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Commands.Console;
using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;
using NuDeploy.Core.Repositories;
using NuDeploy.Core.Services;

using NuGet;

using StructureMap;

namespace NuDeploy.Core.DependencyResolution
{
    public static class StructureMapSetup
    {
        public static void Setup()
        {
            var applicationInformation = new ApplicationInformation
                {
                    ApplicationName = NuDeployConstants.ApplicationName, 
                    NameOfExecutable = NuDeployConstants.ExecutableName,
                    ApplicationVersion = Assembly.GetAssembly(typeof(StructureMapSetup)).GetName().Version,
                    StartupFolder = Environment.CurrentDirectory
                };

            ObjectFactory.Configure(
                config =>
                    {
                        config.For<IConsoleTextManipulation>().Use<ConsoleTextManipulation>();
                        config.For<IUserInterface>().Use<ConsoleUserInterface>();
                        config.For<ApplicationInformation>().Use(applicationInformation);

                        config.For<ICommandArgumentNameMatcher>().Use<CommandArgumentNameMatcher>();
                        config.For<ICommandArgumentParser>().Use<CommandArgumentParser>();
                        config.For<ICommandLineArgumentInterpreter>().Use<CommandLineArgumentInterpreter>();
                        config.For<ICommandNameMatcher>().Use<CommandNameMatcher>();

                        config.For<PSHost>().Use<PowerShellHost>();
                        config.For<PSHostUserInterface>().Use<NuDeployPowerShellUserInterface>();
                        config.For<IPackageRepositoryFactory>().Use<CommandLineRepositoryFactory>();
                    });

            ObjectFactory.Configure(
                config =>
                {
                    var packageRepository = ObjectFactory.GetInstance<IPackageRepositoryFactory>().CreateRepository(NuDeployConstants.DefaultFeedUrl);
                    config.For<IPackageRepository>().Use(packageRepository);
                });

            ObjectFactory.Configure(
                config =>
                {
                    config.For<IPackageConfigurationFileReader>().Use<PackageConfigurationFileReader>();
                    config.For<IPackageInstaller>().Use<PackageInstaller>();
                    config.For<ICleanupService>().Use<CleanupService>();
                    config.For<IInstallationStatusProvider>().Use<ConfigFileInstallationStatusProvider>();
                });

            ObjectFactory.Configure(
                config =>
                {
                    var packageRepository = ObjectFactory.GetInstance<IPackageRepository>();
                    var packageInstaller = ObjectFactory.GetInstance<IPackageInstaller>();
                    var cleanupService = ObjectFactory.GetInstance<ICleanupService>();
                    var installationStatusProvider = ObjectFactory.GetInstance<IInstallationStatusProvider>();

                    var packageCommand = new PackageSolutionCommand();
                    var helpCommand = new HelpCommand(ObjectFactory.GetInstance<IUserInterface>(), applicationInformation);
                    var installationStatusCommand = new InstallationStatusCommand(ObjectFactory.GetInstance<IUserInterface>(), installationStatusProvider);
                    var installCommand = new InstallCommand(ObjectFactory.GetInstance<IUserInterface>(), packageInstaller);
                    var uninstallCommand = new UninstallCommand(ObjectFactory.GetInstance<IUserInterface>(), packageInstaller);
                    var cleanupCommand = new CleanupCommand(ObjectFactory.GetInstance<IUserInterface>(), cleanupService);
                    var selfUpdateCommand = new SelfUpdateCommand(ObjectFactory.GetInstance<IUserInterface>(), applicationInformation, packageRepository);

                    var commands = new List<ICommand> { packageCommand, installCommand, uninstallCommand, cleanupCommand, installationStatusCommand, selfUpdateCommand, helpCommand };
                    ICommandProvider commandProvider = new CommandProvider(commands);

                    config.For<ICommandProvider>().Use(commandProvider);
                    config.For<HelpCommand>().Use(helpCommand);
                });
        }
    }
}
