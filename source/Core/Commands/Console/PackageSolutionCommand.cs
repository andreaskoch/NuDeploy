using System.Collections.Generic;

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

        private const string BuildTarget = "Rebuild";

        private const string TargetPlatform = "Any CPU";

        private const string BuildResultFolder = "C:\\temp";

        private readonly string[] alternativeCommandNames = new[] { "pack" };

        private readonly IUserInterface userInterface;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PackageSolutionCommand(IUserInterface userInterface, IFilesystemAccessor filesystemAccessor)
        {
            this.userInterface = userInterface;
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
                this.userInterface.WriteLine(string.Format("You must specify a build configuration."));
                return;
            }

            var buildProperties = new Dictionary<string, string>();
            buildProperties["Configuration"] = buildConfiguration;
            buildProperties["Platform"] = TargetPlatform;
            buildProperties["OutputPath"] = BuildResultFolder;
            buildProperties["IsAutoBuild"] = bool.TrueString;

            var request = new BuildRequestData(solutionPath, buildProperties, null, new[] { BuildTarget }, null);
            var parms = new BuildParameters();

            BuildResult result = BuildManager.DefaultBuildManager.Build(parms, request);
            bool buildWasSuccessful = result.OverallResult == BuildResultCode.Success;

            if (buildWasSuccessful)
            {
                this.userInterface.WriteLine(string.Format("The solution \"{0}\" has been build successfully ({1}|{2}).", solutionPath, buildConfiguration, TargetPlatform));
            }
            else
            {
                this.userInterface.WriteLine(string.Format("Building the solution \"{0}\" failed ({1}|{2}).", solutionPath, buildConfiguration, TargetPlatform));
            }
        }
    }
}