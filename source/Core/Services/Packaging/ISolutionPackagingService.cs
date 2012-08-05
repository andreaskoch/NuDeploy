using System.Collections.Generic;

namespace NuDeploy.Core.Services.Packaging
{
    public interface ISolutionPackagingService
    {
        IServiceResult PackageSolution(string solutionPath, string buildConfiguration, KeyValuePair<string, string>[] buildProperties);
    }
}