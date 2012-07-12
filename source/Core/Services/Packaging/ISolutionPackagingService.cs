using System.Collections.Generic;

namespace NuDeploy.Core.Services.Packaging
{
    public interface ISolutionPackagingService
    {
        bool PackageSolution(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> buildProperties);
    }
}