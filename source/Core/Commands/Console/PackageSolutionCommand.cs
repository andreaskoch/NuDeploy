using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

using NuGet;

namespace NuDeploy.Core.Commands.Console
{
    public class PackageSolutionCommand : ICommand
    {
        private const string CommandName = "package";

        private const string ArgumentNameSolutionPath = "SolutionPath";

        private const string ArgumentNameBuildConfiguration = "BuildConfiguration";

        private const string ArgumentNameMSBuildProperties = "MSBuildProperties";

        private readonly string[] alternativeCommandNames = new[] { "pack" };

        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly ISolutionBuilder solutionBuilder;

        public PackageSolutionCommand(IUserInterface userInterface, ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, ISolutionBuilder solutionBuilder)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.solutionBuilder = solutionBuilder;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[]
                    {
                        ArgumentNameSolutionPath,
                        ArgumentNameBuildConfiguration,
                        ArgumentNameMSBuildProperties
                    },
                PositionalArguments = new[]
                    {
                        ArgumentNameSolutionPath,
                        ArgumentNameBuildConfiguration,
                        ArgumentNameMSBuildProperties                        
                    },
                Description = Resources.PackageSolutionCommand.CommandDescriptionText,
                Usage = string.Format("{0} -{1}=<Path> -{2}=<Debug|Release> -{3}=<Property1=Value1;Property2=Value2>", CommandName, ArgumentNameSolutionPath, ArgumentNameBuildConfiguration, ArgumentNameMSBuildProperties),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} -{1}=C:\\dev\\projects\\sample\\sample.sln -{2}=Release -{3}=IsAutoBuild=True", CommandName, ArgumentNameSolutionPath, ArgumentNameBuildConfiguration, ArgumentNameMSBuildProperties),
                            Resources.PackageSolutionCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameSolutionPath, Resources.PackageSolutionCommand.ArgumentDescriptionSolutionPath },
                        { ArgumentNameBuildConfiguration, Resources.PackageSolutionCommand.ArgumentDescriptionBuildConfiguration },
                        { ArgumentNameMSBuildProperties, Resources.PackageSolutionCommand.ArgumentDescriptionMSBuildProperties }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            // Solution Path Parameter
            string solutionPath = this.Arguments.ContainsKey(ArgumentNameSolutionPath) ? this.Arguments[ArgumentNameSolutionPath] : string.Empty;
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                this.userInterface.WriteLine(string.Format("You must specifiy a solution path."));
                return;
            }

            if (this.filesystemAccessor.FileExists(solutionPath) == false)
            {
                this.userInterface.WriteLine(string.Format("You must specifiy an existing solution path."));
                return;
            }

            // Build Configuration
            string buildConfiguration = this.Arguments.ContainsKey(ArgumentNameBuildConfiguration) ? this.Arguments[ArgumentNameBuildConfiguration] : string.Empty;
            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                this.userInterface.WriteLine(string.Format("You must specify a build configuration (e.g. Debug, Release)."));
                return;
            }

            // MSBuild Properties
            var buildPropertiesArgument = this.Arguments.ContainsKey(ArgumentNameMSBuildProperties) ? this.Arguments[ArgumentNameMSBuildProperties] : string.Empty;
            var buildProperties = this.ParseBuildPropertiesArgument(buildPropertiesArgument).ToList();

            // Build the solution
            if (!this.solutionBuilder.Build(solutionPath, buildConfiguration, buildProperties))
            {
                this.userInterface.WriteLine(
                    string.Format("Building the solution \"{0}\" for build configuration \"{1}\" failed.", solutionPath, buildConfiguration));

                return;
            }

            this.userInterface.WriteLine(string.Format("The solution \"{0}\" has been build successfully for the build configuration \"{0}\".", solutionPath));

            // pre-packaging
            var buildFolderPath = this.solutionBuilder.BuildFolder;
            var prepackagingFolder = Path.Combine(this.applicationInformation.StartupFolder, "NuDeployPrepackaging");
            if (this.filesystemAccessor.DirectoryExists(prepackagingFolder))
            {
                this.filesystemAccessor.DeleteFolder(prepackagingFolder);
            }

            this.filesystemAccessor.CreateDirectory(prepackagingFolder);

            // NuSpec file
            var nuspecFileSearchPattern = string.Format("*.{0}.nuspec", buildConfiguration);
            var nuspecFile = Directory.GetFiles(buildFolderPath, nuspecFileSearchPattern, SearchOption.TopDirectoryOnly).First();
            if (nuspecFile == null)
            {
                this.userInterface.WriteLine(string.Format("There was no NuSpec file found in your build folder. Without a NuSpec file we will not be able to create a NuGet package (Search Pattern: {0}, Folder: {1}).", nuspecFileSearchPattern, buildFolderPath));
                return;
            }

            var nuspecFileInfo = new FileInfo(nuspecFile);
            var nuspecFileTargetPath = Path.GetFullPath(Path.Combine(prepackagingFolder, nuspecFileInfo.Name));
            File.Copy(nuspecFile, nuspecFileTargetPath);

            // deployment scripts
            string deploymentScriptNamespace = "NuDeploy.Core.Resources.DeploymentScripts.";
            Assembly coreAssembly = typeof(PackageSolutionCommand).Assembly;
            string[] deploymentScriptResourceNames =
                coreAssembly.GetManifestResourceNames().Where(r => r.StartsWith(deploymentScriptNamespace, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (var resourceName in deploymentScriptResourceNames)
            {
                var fileNameFragments = resourceName.Replace(deploymentScriptNamespace, string.Empty).Split('.');
                string fileName = Path.Combine(
                    string.Join(@"\", fileNameFragments.Take(fileNameFragments.Length - 2)),
                    string.Join(".", fileNameFragments.Skip(fileNameFragments.Length - 2).Take(2)));

                string filePath = Path.GetFullPath(Path.Combine(prepackagingFolder, fileName));
                string fileDirectory = new FileInfo(filePath).Directory.FullName;
                this.filesystemAccessor.CreateDirectory(fileDirectory);

                using (Stream resourceStream = coreAssembly.GetManifestResourceStream(resourceName))
                {
                    using (Stream fileStream = File.OpenWrite(filePath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }  
                }
            }

            // websites
            var publishedWebsitesTargetFolderPath = Path.GetFullPath(Path.Combine(prepackagingFolder, "content", "websites"));
            var publishedWebsitesSourceFolderPath = Path.GetFullPath(Path.Combine(buildFolderPath, "_PublishedWebsites"));
            if (this.filesystemAccessor.DirectoryExists(publishedWebsitesSourceFolderPath))
            {
                var publishedWebsiteDirectories = Directory.GetDirectories(publishedWebsitesSourceFolderPath, "*.Website.*", SearchOption.TopDirectoryOnly);
                var publishedWebsiteFiles = publishedWebsiteDirectories.SelectMany(d => Directory.GetFiles(d, "*", SearchOption.AllDirectories));

                foreach (var file in publishedWebsiteFiles)
                {
                    string targetFileName = file.Replace(publishedWebsitesSourceFolderPath + "\\", string.Empty);
                    string targetFilePath = Path.Combine(publishedWebsitesTargetFolderPath, targetFileName);

                    string targetFileDirectory = new FileInfo(targetFilePath).Directory.FullName;
                    this.filesystemAccessor.CreateDirectory(targetFileDirectory);

                    File.Move(file, targetFilePath);
                }                
            }

            // web applications
            var publishedWebapplicationsTargetFolderPath = Path.GetFullPath(Path.Combine(prepackagingFolder, "content", "webapplications"));
            var publishedWebapplicationsSourceFolderPath = Path.GetFullPath(Path.Combine(buildFolderPath, "_PublishedWebsites"));
            if (this.filesystemAccessor.DirectoryExists(publishedWebapplicationsSourceFolderPath))
            {
                var publishedWebapplicationFiles = Directory.GetFiles(publishedWebapplicationsSourceFolderPath, "*", SearchOption.AllDirectories);

                foreach (var file in publishedWebapplicationFiles)
                {
                    string targetFileName = file.Replace(publishedWebapplicationsSourceFolderPath + "\\", string.Empty);
                    string targetFilePath = Path.Combine(publishedWebapplicationsTargetFolderPath, targetFileName);

                    string targetFileDirectory = new FileInfo(targetFilePath).Directory.FullName;
                    this.filesystemAccessor.CreateDirectory(targetFileDirectory);

                    File.Move(file, targetFilePath);
                }                
            }

            // applications
            var publishedApplicationsTargetFolderPath = Path.GetFullPath(Path.Combine(prepackagingFolder, "content", "applications"));
            var publishedApplicationsSourceFolderPath = Path.GetFullPath(Path.Combine(buildFolderPath, "_PublishedApplications"));
            if (this.filesystemAccessor.DirectoryExists(publishedApplicationsSourceFolderPath))
            {
                var publishedApplicationFiles = Directory.GetFiles(publishedApplicationsSourceFolderPath, "*", SearchOption.AllDirectories);

                foreach (var file in publishedApplicationFiles)
                {
                    string targetFileName = file.Replace(publishedApplicationsSourceFolderPath + "\\", string.Empty);
                    string targetFilePath = Path.Combine(publishedApplicationsTargetFolderPath, targetFileName);

                    string targetFileDirectory = new FileInfo(targetFilePath).Directory.FullName;
                    this.filesystemAccessor.CreateDirectory(targetFileDirectory);

                    File.Move(file, targetFilePath);
                }                
            }

            // deployment package additions
            var deploymentPackageAdditionsTargetFolderPath = Path.GetFullPath(prepackagingFolder);
            var publishedDeploymentPackageAdditionsSourceFolderPath = Path.GetFullPath(Path.Combine(buildFolderPath, "deploymentpackageadditions"));
            if (this.filesystemAccessor.DirectoryExists(publishedDeploymentPackageAdditionsSourceFolderPath))
            {
                var publishedDeploymentPackageAdditionFiles = Directory.GetFiles(publishedDeploymentPackageAdditionsSourceFolderPath, "*", SearchOption.AllDirectories);

                foreach (var file in publishedDeploymentPackageAdditionFiles)
                {
                    string targetFileName = file.Replace(publishedDeploymentPackageAdditionsSourceFolderPath + "\\", string.Empty);
                    string targetFilePath = Path.Combine(deploymentPackageAdditionsTargetFolderPath, targetFileName);

                    string targetFileDirectory = new FileInfo(targetFilePath).Directory.FullName;
                    this.filesystemAccessor.CreateDirectory(targetFileDirectory);

                    File.Move(file, targetFilePath);
                }                
            }

            // build package
            string packageBasePath = Path.GetFullPath(prepackagingFolder);
            string packageFolder = Path.GetFullPath(Path.Combine(this.applicationInformation.StartupFolder, "NuDeployPackages"));
            string nugetPackageFilePath = Path.Combine(packageFolder, nuspecFileInfo.Name.Replace(".nuspec", ".1.0.0.nupkg"));
            if (!this.filesystemAccessor.DirectoryExists(packageFolder))
            {
                this.filesystemAccessor.CreateDirectory(packageFolder);
            }

            var packageBuilder = new PackageBuilder(nuspecFileTargetPath, packageBasePath);
            using (Stream stream = File.Create(nugetPackageFilePath))
            {
                packageBuilder.Save(stream);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> ParseBuildPropertiesArgument(string builProperties)
        {
            var keyValuePairStrings = builProperties.Split(NuDeployConstants.MultiValueSeperator).Where(p => string.IsNullOrWhiteSpace(p) == false).Select(p => p.Trim());
            foreach (var keyValuePairString in keyValuePairStrings)
            {
                var segments = keyValuePairString.Split('=');
                if (segments.Count() == 2)
                {
                    string key = segments.First();
                    string value = segments.Last();

                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }
    }
}