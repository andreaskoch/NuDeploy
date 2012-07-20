using System.Collections.Generic;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public interface IBuildPropertyProvider
    {
        IDictionary<string, string> GetBuildProperties(
            string buildConfiguration, string targetPlatform, string outputPath, IEnumerable<KeyValuePair<string, string>> additionalBuildProperties);
    }
}