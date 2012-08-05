using System.Collections.Generic;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public interface ISolutionBuilder
    {
        IServiceResult Build(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> additionalBuildProperties);
    }
}