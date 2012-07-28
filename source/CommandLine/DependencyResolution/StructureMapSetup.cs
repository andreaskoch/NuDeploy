using System;
using System.Management.Automation.Host;
using System.Runtime.InteropServices;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.CommandLine.Infrastructure.Console;
using NuDeploy.CommandLine.UserInterface;
using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.Core.Services.Cleanup;
using NuDeploy.Core.Services.Filesystem;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.PowerShell;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Installation.Status;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Configuration;
using NuDeploy.Core.Services.Packaging.Nuget;
using NuDeploy.Core.Services.Packaging.PrePackaging;
using NuDeploy.Core.Services.Transformation;
using NuDeploy.Core.Services.Update;

using NuGet;

using StructureMap;

namespace NuDeploy.CommandLine.DependencyResolution
{
    public static class StructureMapSetup
    {
        public static void Setup()
        {
            var applicationInformation = ApplicationInformationProvider.GetApplicationInformation();

            ObjectFactory.Configure(
                config =>
                    {
                        /* file encoding */
                        config.For<IEncodingProvider>().Singleton().Use<DefaultFileEncodingProvider>();

                        /* logging */
                        config.For<IActionLogger>().Singleton().Use<ActionLogger>();

                        /* infrastructure */
                        config.For<ApplicationInformation>().Use(applicationInformation);

                        /* filesystem access */
                        config.For<IFilesystemAccessor>().Singleton().Use<PhysicalFilesystemAccessor>();

                        /* console */
                        config.For<IUserInterface>().Singleton().Use<ConsoleUserInterface>();
                        config.For<IConsoleTextManipulation>().Use<ConsoleTextManipulation>();

                        /* powershell */
                        config.For<IPowerShellHost>().Use<PowerShellHost>();
                        config.For<PSHostUserInterface>().Use<NuDeployPowerShellUserInterface>();
                        config.For<IPowerShellExecutor>().Use<PowerShellExecutor>();
                        config.For<IPowerShellSessionFactory>().Use<PowerShellSessionFactory>();

                        /* assembly resource access */
                        config.For<_Assembly>().Use(typeof(ApplicationInformationProvider).Assembly);
                        config.For<IAssemblyResourceFilePathProvider>().Use<AssemblyResourceFilePathProvider>();
                        config.For<IAssemblyFileResourceProvider>().Use<AssemblyFileResourceProvider>();
                        config.For<IAssemblyResourceDownloader>().Use<DeploymentScriptResourceDownloader>();

                        /* cleanup */
                        config.For<ICleanupService>().Use<CleanupService>();

                        /* configuration */

                        /* file system */
                        config.For<IRelativeFilePathInfoFactory>().Use<RelativeFilePathInfoFactory>();

                        /* installation */
                        config.For<IInstallationLogicProvider>().Use<InstallationLogicProvider>();
                        config.For<IDeploymentTypeParser>().Use<DeploymentTypeParser>();
                        config.For<IRepositoryConfigurationCommandActionParser>().Use<RepositoryConfigurationCommandActionParser>();
                        config.For<ISourceRepositoryProvider>().Use<ConfigFileSourceRepositoryProvider>();
                        config.For<ISourceRepositoryConfigurationFactory>().Use<SourceRepositoryConfigurationFactory>();
                        config.For<Func<Uri, IHttpClient>>().Use(uri => new RedirectedHttpClient(uri));
                        config.For<IPackageRepositoryBrowser>().Use<PackageRepositoryBrowser>();
                        config.For<IPackageConfigurationAccessor>().Use<PackageConfigurationAccessor>();
                        config.For<IPackageInstaller>().Use<PackageInstaller>();
                        config.For<IPackageUninstaller>().Use<PackageUninstaller>();
                        config.For<IPackageRepositoryFactory>().Use<CommandLineRepositoryFactory>();
                        config.For<INugetPackageExtractor>().Use<NugetPackageExtractor>();

                        /* packaging */
                        config.For<ISolutionPackagingService>().Use<SolutionPackagingService>();

                        /* build */
                        config.For<IBuildFolderPathProvider>().Use<BuildFolderPathProvider>();
                        config.For<ISolutionBuilder>().Use<SolutionBuilder>();
                        config.For<IBuildResultFilePathProvider>().Use<ConventionBasedBuildResultFilePathProvider>();
                        config.For<IBuildPropertyProvider>().Use<BuildPropertyProvider>();

                        /* console services */
                        config.For<ICommandArgumentNameMatcher>().Use<CommandArgumentNameMatcher>();
                        config.For<ICommandArgumentParser>().Use<CommandArgumentParser>();
                        config.For<ICommandLineArgumentInterpreter>().Use<CommandLineArgumentInterpreter>();
                        config.For<ICommandNameMatcher>().Use<CommandNameMatcher>();
                        config.For<IBuildPropertyParser>().Use<BuildPropertyParser>();

                        /* nuget */
                        config.For<IPackagingFolderPathProvider>().Use<PackagingFolderPathProvider>();
                        config.For<IPackagingService>().Use<PackagingService>();

                        /* pre-packaging */
                        config.For<IPrePackagingFolderPathProvider>().Use<PrePackagingFolderPathProvider>();
                        config.For<IPrepackagingService>().Use<PrepackagingService>();

                        /* status */
                        config.For<IInstallationStatusProvider>().Use<InstallationStatusProvider>();

                        /* transformation */
                        config.For<IPackageConfigurationTransformationService>().Use<PackageConfigurationTransformationService>();
                        config.For<IConfigurationFileTransformationService>().Use<ConfigurationFileTransformationService>();
                        config.For<IConfigurationFileTransformer>().Use<ConfigurationFileTransformer>();

                        /* update */
                        config.For<ISelfUpdateService>().Use<SelfUpdateService>();

                        config.For<IHelpProvider>().Use<HelpProvider>();
                        config.For<IHelpCommand>().Use<HelpCommand>();
                        config.For<ICommandProvider>().Use<ConsoleCommandProvider>();
                    });
        }
    }
}
