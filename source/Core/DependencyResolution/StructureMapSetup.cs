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
                        config.For<IFilesystemAccessor>().Singleton().Use<PhysicalFilesystemAccessor>();
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

                    config.For<IConfigurationFileTransformer>().Use<ConfigurationFileTransformer>();
                });

            ObjectFactory.Configure(
                config =>
                    {
                        var commands = new List<ICommand>
                            {
                                ObjectFactory.GetInstance<PackageSolutionCommand>(),
                                ObjectFactory.GetInstance<HelpCommand>(),
                                ObjectFactory.GetInstance<InstallationStatusCommand>(),
                                ObjectFactory.GetInstance<InstallCommand>(),
                                ObjectFactory.GetInstance<UninstallCommand>(),
                                ObjectFactory.GetInstance<CleanupCommand>(),
                                ObjectFactory.GetInstance<SelfUpdateCommand>(),
                                ObjectFactory.GetInstance<RepositorySourceConfigurationCommand>(),
                            };

                    ICommandProvider commandProvider = new NuDeployConsoleCommandProvider(commands);

                    config.For<ICommandProvider>().Use(commandProvider);
                });
        }
    }
}
