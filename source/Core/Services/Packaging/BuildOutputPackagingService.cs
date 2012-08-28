using System;

using NuDeploy.Core.Services.Packaging.Nuget;
using NuDeploy.Core.Services.Packaging.PrePackaging;

namespace NuDeploy.Core.Services.Packaging
{
    public class BuildOutputPackagingService : IBuildOutputPackagingService
    {
        private readonly IPrepackagingService prepackagingService;

        private readonly IPackagingService packagingService;

        public BuildOutputPackagingService(IPrepackagingService prepackagingService, IPackagingService packagingService)
        {
            if (prepackagingService == null)
            {
                throw new ArgumentNullException("prepackagingService");
            }

            if (packagingService == null)
            {
                throw new ArgumentNullException("packagingService");
            }

            this.prepackagingService = prepackagingService;
            this.packagingService = packagingService;
        }

        public IServiceResult Package(string buildOutputFolderPath)
        {
            if (string.IsNullOrWhiteSpace(buildOutputFolderPath))
            {
                return new FailureResult(Resources.BuildOutputPackagingService.ErrorBuildOutputFolderPathParameterCannotBeEmpty);
            }

            // pre-packaging
            IServiceResult prepackagingResult = this.prepackagingService.Prepackage(buildOutputFolderPath);
            if (prepackagingResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.BuildOutputPackagingService.PrepackagingFailedMessageTemplate, buildOutputFolderPath) { InnerResult = prepackagingResult };
            }

            // packaging
            IServiceResult packageResult = this.packagingService.Package();
            if (packageResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.BuildOutputPackagingService.PackagingFailedMessageTemplate, buildOutputFolderPath) { InnerResult = packageResult };
            }

            string packagePath = packageResult.ResultArtefact;

            return new SuccessResult(Resources.BuildOutputPackagingService.PackagingSucceededMessageTemplate, buildOutputFolderPath, packagePath)
                {
                    ResultArtefact = packagePath
                };
        }
    }
}