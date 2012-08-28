using System;
using System.Collections.Generic;
using System.Linq;

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

        private readonly string buildFolder;

        public SolutionPackagingService(ISolutionBuilder solutionBuilder, IPrepackagingService prepackagingService, IPackagingService packagingService, IBuildFolderPathProvider buildFolderPathProvider)
        {
            if (solutionBuilder == null)
            {
                throw new ArgumentNullException("solutionBuilder");
            }

            if (prepackagingService == null)
            {
                throw new ArgumentNullException("prepackagingService");
            }

            if (packagingService == null)
            {
                throw new ArgumentNullException("packagingService");
            }

            if (buildFolderPathProvider == null)
            {
                throw new ArgumentNullException("buildFolderPathProvider");
            }

            this.solutionBuilder = solutionBuilder;
            this.prepackagingService = prepackagingService;
            this.packagingService = packagingService;
            this.buildFolder = buildFolderPathProvider.GetBuildFolderPath();
        }

        public IServiceResult PackageSolution(string solutionPath, string buildConfiguration, KeyValuePair<string, string>[] buildProperties)
        {
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                return new FailureResult(Resources.SolutionPackagingService.ErrorSolutionPathParameterCannotBeEmpty);
            }

            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                return new FailureResult(Resources.SolutionPackagingService.ErrorBuildConfigurationParameterCannotBeEmpty);
            }

            if (buildProperties == null)
            {
                return new FailureResult(Resources.SolutionPackagingService.ErrorBuildPropertiesParameterCannotBeNull);
            }

            // Build the solution
            IServiceResult buildResult = this.solutionBuilder.Build(solutionPath, buildConfiguration, buildProperties);
            if (buildResult.Status == ServiceResultType.Failure)
            {
                return
                    new FailureResult(
                        Resources.SolutionPackagingService.BuildFailedMessageTemplate,
                        solutionPath,
                        buildConfiguration,
                        string.Join(",", buildProperties.Select(p => p.Key + "=" + p.Value)))
                        {
                            InnerResult = buildResult
                        };
            }

            // pre-packaging
            IServiceResult prepackagingResult = this.prepackagingService.Prepackage(this.buildFolder);
            if (prepackagingResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.SolutionPackagingService.PrepackagingFailedMessageTemplate, solutionPath) { InnerResult = prepackagingResult };
            }

            // packaging
            IServiceResult packageResult = this.packagingService.Package();
            if (packageResult.Status == ServiceResultType.Failure)
            {
                return new FailureResult(Resources.SolutionPackagingService.PackagingFailedMessageTemplate, solutionPath) { InnerResult = packageResult };
            }

            string packagePath = packageResult.ResultArtefact;

            return
                new SuccessResult(
                    Resources.SolutionPackagingService.PackagingSucceededMessageTemplate,
                    solutionPath,
                    buildConfiguration,
                    string.Join(",", buildProperties.Select(p => p.Key + "=" + p.Value)),
                    packagePath)
                    {
                        ResultArtefact = packagePath
                    };
        }
    }
}