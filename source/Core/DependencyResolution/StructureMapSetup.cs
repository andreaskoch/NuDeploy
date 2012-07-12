using System.Management.Automation.Host;
using System.Reflection;

using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Common.UserInterface.Console;
using NuDeploy.Core.PowerShell;
using NuDeploy.Core.Services.AssemblyResourceAccess;
using NuDeploy.Core.Services.Cleanup;
using NuDeploy.Core.Services.Commands;
using NuDeploy.Core.Services.Configuration;
using NuDeploy.Core.Services.Console;
using NuDeploy.Core.Services.Filesystem;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Installation.Repositories;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Nuget;
using NuDeploy.Core.Services.Packaging.PrePackaging;
using NuDeploy.Core.Services.Status;
using NuDeploy.Core.Services.Transformation;
using NuDeploy.Core.Services.Update;

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
                        /* file encoding */
                        config.For<IEncodingProvider>().Singleton().Use<DefaultFileEncodingProvider>();

                        /* logging */
                        config.For<IActionLogger>().Singleton().Use<ActionLogger>();

                        /* infrastructure */
                        config.For<ApplicationInformation>().Use(applicationInformation);

                        /* filesystem access */
                        config.For<IFilesystemAccessor>().Singleton().Use<PhysicalFilesystemAccessor>();

                        /* console */
                        config.For<IUserInterface>().Use<ConsoleUserInterface>();
                        config.For<IConsoleTextManipulation>().Use<ConsoleTextManipulation>();

                        /* powershell */
                        config.For<PSHost>().Use<PowerShellHost>();
                        config.For<PSHostUserInterface>().Use<NuDeployPowerShellUserInterface>();
                        config.For<IPackageRepositoryFactory>().Use<CommandLineRepositoryFactory>();

                        /* assembly resource access */
                        config.For<Assembly>().Use(typeof(ApplicationInformationProvider).Assembly);
                        config.For<IAssemblyResourceFilePathProvider>().Use<AssemblyResourceFilePathProvider>();
                        config.For<IAssemblyFileResourceProvider>().Use<AssemblyFileResourceProvider>();
                        config.For<IDeploymentScriptResourceDownloader>().Use<DeploymentScriptResourceDownloader>();

                        /* cleanup */
                        config.For<ICleanupService>().Use<CleanupService>();

                        /* configuration */
                        config.For<IBuildFolderPathProvider>().Use<BuildFolderPathProvider>();
                        config.For<IPackagingFolderPathProvider>().Use<PackagingFolderPathProvider>();
                        config.For<IPrePackagingFolderPathProvider>().Use<PrePackagingFolderPathProvider>();
                        config.For<ISourceRepositoryProvider>().Use<ConfigFileSourceRepositoryProvider>();

                        /* file system */
                        config.For<IRelativeFilePathInfoFactory>().Use<RelativeFilePathInfoFactory>();

                        /* installation */
                        config.For<IPackageRepositoryBrowser>().Use<PackageRepositoryBrowser>();
                        config.For<IPackageConfigurationAccessor>().Use<PackageConfigurationAccessor>();
                        config.For<IPackageInstaller>().Use<PackageInstaller>();

                        /* packaging */
                        config.For<ISolutionPackagingService>().Use<SolutionPackagingService>();

                        /* build */
                        config.For<ISolutionBuilder>().Use<SolutionBuilder>();
                        config.For<IBuildResultFilePathProvider>().Use<ConventionBasedBuildResultFilePathProvider>();

                        /* console services */
                        config.For<ICommandArgumentNameMatcher>().Use<CommandArgumentNameMatcher>();
                        config.For<ICommandArgumentParser>().Use<CommandArgumentParser>();
                        config.For<ICommandLineArgumentInterpreter>().Use<CommandLineArgumentInterpreter>();
                        config.For<ICommandNameMatcher>().Use<CommandNameMatcher>();

                        /* nuget */
                        config.For<IPackagingService>().Use<PackagingService>();

                        /* pre-packaging */
                        config.For<IPrepackagingService>().Use<PrepackagingService>();

                        /* status */
                        config.For<IInstallationStatusProvider>().Use<InstallationStatusProvider>();

                        /* transformation */
                        config.For<IConfigurationFileTransformer>().Use<ConfigurationFileTransformer>();

                        /* update */
                        config.For<ISelfUpdateService>().Use<SelfUpdateService>();

                        config.For<ICommandProvider>().Use<NuDeployConsoleCommandProvider>();
                    });
        }
    }
}
