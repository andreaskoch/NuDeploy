using System.Collections.Generic;

namespace NuDeploy.Core.Services
{
    public interface ISolutionBuilder
    {
        string BuildFolder { get; }

        bool Build(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> buildProperties);
    }
}