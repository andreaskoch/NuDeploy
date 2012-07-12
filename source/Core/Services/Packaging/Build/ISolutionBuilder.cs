using System.Collections.Generic;

namespace NuDeploy.Core.Services.Packaging.Build
{
    public interface ISolutionBuilder
    {
        bool Build(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> buildProperties);
    }
}