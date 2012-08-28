using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public class SolutionBuilder : ISolutionBuilder
    {
        private const string DefaultBuildTarget = "Build";

        private const string DefaultTargetPlatform = "Any CPU";

        private readonly string buildFolder;

        private readonly IBuildPropertyProvider buildPropertyProvider;

        public SolutionBuilder(IBuildFolderPathProvider buildFolderPathProvider, IBuildPropertyProvider buildPropertyProvider)
        {
            if (buildFolderPathProvider == null)
            {
                throw new ArgumentNullException("buildFolderPathProvider");
            }

            if (buildPropertyProvider == null)
            {
                throw new ArgumentNullException("buildPropertyProvider");
            }

            this.buildFolder = buildFolderPathProvider.GetBuildFolderPath();
            this.buildPropertyProvider = buildPropertyProvider;
        }

        public IServiceResult Build(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> additionalBuildProperties)
        {
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                throw new ArgumentException("solutionPath");
            }

            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                throw new ArgumentException("buildConfiguration");
            }

            if (additionalBuildProperties == null)
            {
                throw new ArgumentNullException("additionalBuildProperties");
            }

            // prepare build parameters
            var buildProperties = this.buildPropertyProvider.GetBuildProperties(
                buildConfiguration, DefaultTargetPlatform, this.buildFolder, additionalBuildProperties.ToList());

            // prepare build request
            var buildLogger = new ConsoleLogger();
            var buildRequestData = new BuildRequestData(solutionPath, buildProperties, null, new[] { DefaultBuildTarget }, null);
            var buildParameters = new BuildParameters { Loggers = new ILogger[] { buildLogger } };

            // build
            BuildResult result = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            if (result.OverallResult == BuildResultCode.Success)
            {
                return new SuccessResult(
                    Resources.SolutionBuilder.SuccessMessageTemplate,
                    solutionPath,
                    buildConfiguration,
                    string.Join(",", buildProperties.Select(p => p.Key + "=" + p.Value)));
            }

            return new FailureResult(
                Resources.SolutionBuilder.FailureMessageTemplate,
                solutionPath,
                buildConfiguration,
                string.Join(",", buildProperties.Select(p => p.Key + "=" + p.Value)));
        }
    }
}