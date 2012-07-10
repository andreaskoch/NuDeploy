using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Execution;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Commands.Console
{
    public class PackageSolutionCommand : ICommand
    {
        private const string CommandName = "package";

        private const string ArgumentNameSolutionPath = "SolutionPath";

        private const string ArgumentNameBuildConfiguration = "BuildConfiguration";

        private const string ArgumentNameMSBuildProperties = "MSBuildProperties";

        private const string BuildPropertyNameTargetPlatform = "Platform";

        private const string BuildPropertyNameOutputPath = "OutputPath";

        private const string BuildPropertyNameBuildConfiguration = "Configuration";

        private const string BuildPropertyBuildTarget = "Rebuild";

        private const string BuildPropertyTargetPlatform = "Any CPU";

        private readonly string[] alternativeCommandNames = new[] { "pack" };

        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PackageSolutionCommand(IUserInterface userInterface, ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;

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
            var additionalBuildProperties = this.ParseBuildPropertiesArgument(buildPropertiesArgument).ToList();

            // build folder
            string buildFolderPath = this.GetBuildFolderPath();
            if (!this.PrepareBuildFolder(buildFolderPath))
            {
                this.userInterface.WriteLine(string.Format("Could not prepare the build folder \"{0}\". Please check the folder and try again.", buildFolderPath));
                return;
            }

            // build properties
            var buildProperties = new Dictionary<string, string>
                {
                    { BuildPropertyNameBuildConfiguration, buildConfiguration },
                    { BuildPropertyNameTargetPlatform, BuildPropertyTargetPlatform },
                    { BuildPropertyNameOutputPath, buildFolderPath }
                };

            // add additional build properties
            foreach (var additionalBuildProperty in additionalBuildProperties)
            {
                buildProperties[additionalBuildProperty.Key] = additionalBuildProperty.Value;
            }

            // build
            var request = new BuildRequestData(solutionPath, buildProperties, null, new[] { BuildPropertyBuildTarget }, null);
            var parms = new BuildParameters();

            BuildResult result = BuildManager.DefaultBuildManager.Build(parms, request);
            bool buildWasSuccessful = result.OverallResult == BuildResultCode.Success;

            // evaluate build result
            this.userInterface.WriteLine(
                buildWasSuccessful
                    ? string.Format("The solution \"{0}\" has been build successfully (Build Configuration: {1}, Platform: {2}).", solutionPath, buildConfiguration, BuildPropertyTargetPlatform)
                    : string.Format("Building the solution \"{0}\" failed (Build Configuration: {1}, Platform: {2}).", solutionPath, buildConfiguration, BuildPropertyTargetPlatform));
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

        private bool PrepareBuildFolder(string buildFolderPath)
        {
            // create folder if it does not exist
            if (this.filesystemAccessor.DirectoryExists(buildFolderPath) == false)
            {
                return this.filesystemAccessor.CreateDirectory(buildFolderPath);
            }

            // cleanup existing folder
            if (this.filesystemAccessor.DeleteFolder(buildFolderPath))
            {
                return this.filesystemAccessor.CreateDirectory(buildFolderPath);
            }

            // build folder could not be cleaned
            return false;
        }

        private string GetBuildFolderPath()
        {
            return this.applicationInformation.BuildFolder;
        }
    }
}