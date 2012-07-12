using System.Collections.Generic;

using Microsoft.Build.Execution;

using NuDeploy.Core.Services.Configuration;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public class SolutionBuilder : ISolutionBuilder
    {
        private const string BuildPropertyNameTargetPlatform = "Platform";

        private const string BuildPropertyNameOutputPath = "OutputPath";

        private const string BuildPropertyNameBuildConfiguration = "Configuration";

        private const string DefaultBuildTarget = "Rebuild";

        private const string DefaultTargetPlatform = "Any CPU";

        private readonly string buildFolder;

        public SolutionBuilder(IBuildFolderPathProvider buildFolderPathProvider)
        {
            this.buildFolder = buildFolderPathProvider.GetBuildFolderPath();
        }

        public bool Build(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> buildProperties)
        {
            // prepare build parameters
            var buildParameters = new Dictionary<string, string>
                {
                    { BuildPropertyNameBuildConfiguration, buildConfiguration },
                    { BuildPropertyNameTargetPlatform, DefaultTargetPlatform },
                    { BuildPropertyNameOutputPath, this.buildFolder }
                };

            foreach (var buildProperty in buildProperties)
            {
                buildParameters[buildProperty.Key] = buildProperty.Value;
            }

            var request = new BuildRequestData(solutionPath, buildParameters, null, new[] { DefaultBuildTarget }, null);
            var parms = new BuildParameters();
            BuildResult result = BuildManager.DefaultBuildManager.Build(parms, request);
            return result.OverallResult == BuildResultCode.Success;
        }
    }
}