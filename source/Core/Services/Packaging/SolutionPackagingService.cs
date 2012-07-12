using System.Collections.Generic;

using NuDeploy.Core.Services.Packaging.Build;
using NuDeploy.Core.Services.Packaging.Nuget;
using NuDeploy.Core.Services.Packaging.PrePackaging;

namespace NuDeploy.Core.Services.Packaging
{
    public class SolutionPackagingService : ISolutionPackagingService
    {
        private readonly ISolutionBuilder solutionBuilder;

        private readonly IPrepackagingService prepackagingService;

        private readonly IPackagingService packagingService;

        public SolutionPackagingService(ISolutionBuilder solutionBuilder, IPrepackagingService prepackagingService, IPackagingService packagingService)
        {
            this.solutionBuilder = solutionBuilder;
            this.prepackagingService = prepackagingService;
            this.packagingService = packagingService;
        }

        public bool PackageSolution(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> buildProperties)
        {
            // Build the solution
            if (!this.solutionBuilder.Build(solutionPath, buildConfiguration, buildProperties))
            {
                return false;
            }

            // pre-packaging
            if (!this.prepackagingService.Prepackage(buildConfiguration))
            {
                return false;
            }

            // packaging
            if (!this.packagingService.Package())
            {
                return false;
            }

            return true;
        }
    }
}