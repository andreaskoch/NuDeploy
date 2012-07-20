using System;
using System.Collections.Generic;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public class BuildPropertyProvider : IBuildPropertyProvider
    {
        public const string BuildPropertyNameTargetPlatform = "Platform";

        public const string BuildPropertyNameOutputPath = "OutputPath";

        public const string BuildPropertyNameBuildConfiguration = "Configuration";

        public IDictionary<string, string> GetBuildProperties(string buildConfiguration, string targetPlatform, string outputPath, IEnumerable<KeyValuePair<string, string>> additionalBuildProperties)
        {
            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                throw new ArgumentException("buildConfiguration");
            }

            if (string.IsNullOrWhiteSpace(targetPlatform))
            {
                throw new ArgumentException("targetPlatform");
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("outputPath");
            }

            if (additionalBuildProperties == null)
            {
                throw new ArgumentNullException("additionalBuildProperties");
            }

            var buildParameters = new Dictionary<string, string>
                {
                    { BuildPropertyNameBuildConfiguration, buildConfiguration },
                    { BuildPropertyNameTargetPlatform, targetPlatform },
                    { BuildPropertyNameOutputPath, outputPath }
                };

            foreach (var buildProperty in additionalBuildProperties)
            {
                buildParameters[buildProperty.Key] = buildProperty.Value;
            }

            return buildParameters;
        }
    }
}