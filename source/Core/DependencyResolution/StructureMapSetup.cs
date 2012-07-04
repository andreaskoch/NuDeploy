using System.Collections.Generic;
using System.Management.Automation.Host;

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
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            ObjectFactory.Configure(
                config =>
                    {
                        config.For<IActionLogger>().Singleton().Use<ActionLogger>();

                        config.For<IConsoleTextManipulation>().Use<ConsoleTextManipulation>();
                        config.For<IUserInterface>().Use<ConsoleUserInterface>();
                        config.For<ApplicationInformation>().Use(applicationInformation);

                        config.For<ICommandArgumentNameMatcher>().Use<CommandArgumentNameMatcher>();
                        config.For<ICommandArgumentParser>().Use<CommandArgumentParser>();
                        config.For<ICommandLineArgumentInterpreter>().Use<CommandLineArgumentInterpreter>();
                        config.For<ICommandNameMatcher>().Use<CommandNameMatcher>();

                        config.For<ISourceRepositoryProvider>().Use<ConfigFileSourceRepositoryProvider>();

                        config.For<PSHost>().Use<PowerShellHost>();
                        config.For<PSHostUserInterface>().Use<NuDeployPowerShellUserInterface>();
                        config.For<IPackageRepositoryFactory>().Use<CommandLineRepositoryFactory>();
                    });

            ObjectFactory.Configure(
                config =>
                {
                    config.For<IPackageRepositoryBrowser>().Use<PackageRepositoryBrowser>();
                    config.For<IPackageConfigurationAccessor>().Use<PackageConfigurationAccessor>();
                    config.For<IPackageInstaller>().Use<PackageInstaller>();
                    config.For<ICleanupService>().Use<CleanupService>();
                    config.For<IInstallationStatusProvider>().Use<InstallationStatusProvider>();
                });

            ObjectFactory.Configure(
                config =>
                {
                    var sourceRepositoryConfigurationProvider = ObjectFactory.GetInstance<ISourceRepositoryProvider>();
                    var packageRepositoryBrowser = ObjectFactory.GetInstance<IPackageRepositoryBrowser>();
                    var packageInstaller = ObjectFactory.GetInstance<IPackageInstaller>();
                    var cleanupService = ObjectFactory.GetInstance<ICleanupService>();
                    var installationStatusProvider = ObjectFactory.GetInstance<IInstallationStatusProvider>();

                    var packageCommand = new PackageSolutionCommand();
                    var helpCommand = new HelpCommand(ObjectFactory.GetInstance<IUserInterface>(), applicationInformation);
                    var installationStatusCommand = new InstallationStatusCommand(ObjectFactory.GetInstance<IUserInterface>(), installationStatusProvider);
                    var installCommand = new InstallCommand(ObjectFactory.GetInstance<IUserInterface>(), packageInstaller);
                    var uninstallCommand = new UninstallCommand(ObjectFactory.GetInstance<IUserInterface>(), packageInstaller);
                    var cleanupCommand = new CleanupCommand(ObjectFactory.GetInstance<IUserInterface>(), cleanupService);
                    var selfUpdateCommand = new SelfUpdateCommand(ObjectFactory.GetInstance<IUserInterface>(), applicationInformation, packageRepositoryBrowser);
                    var configureSources = new RepositorySourceConfigurationCommand(
                        ObjectFactory.GetInstance<IUserInterface>(), sourceRepositoryConfigurationProvider);

                    var commands = new List<ICommand>
                        {
                            packageCommand,
                            installCommand,
                            uninstallCommand,
                            cleanupCommand,
                            installationStatusCommand,
                            configureSources,
                            selfUpdateCommand,
                            helpCommand
                        };

                    ICommandProvider commandProvider = new CommandProvider(commands);

                    config.For<ICommandProvider>().Use(commandProvider);
                    config.For<HelpCommand>().Use(helpCommand);
                });
        }
    }
}
