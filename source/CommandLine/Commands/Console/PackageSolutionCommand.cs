using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Publishing;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class PackageSolutionCommand : ICommand
    {
        public const string CommandName = "package";

        public const string ArgumentNameSolutionPath = "SolutionPath";

        public const string ArgumentNameBuildConfiguration = "BuildConfiguration";

        public const string ArgumentNameMSBuildProperties = "MSBuildProperties";

        public const string ArgumentNamePublishingConfiguration = "PublishingConfiguration";

        private readonly string[] alternativeCommandNames = new[] { "pack" };

        private readonly IUserInterface userInterface;

        private readonly ISolutionPackagingService solutionPackagingService;

        private readonly IBuildPropertyParser buildPropertyParser;

        private readonly IPublishingService publishingService;

        public PackageSolutionCommand(IUserInterface userInterface, ISolutionPackagingService solutionPackagingService, IBuildPropertyParser buildPropertyParser, IPublishingService publishingService)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (solutionPackagingService == null)
            {
                throw new ArgumentNullException("solutionPackagingService");
            }

            if (buildPropertyParser == null)
            {
                throw new ArgumentNullException("buildPropertyParser");
            }

            if (publishingService == null)
            {
                throw new ArgumentNullException("publishingService");
            }

            this.userInterface = userInterface;
            this.solutionPackagingService = solutionPackagingService;
            this.buildPropertyParser = buildPropertyParser;
            this.publishingService = publishingService;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[]
                    {
                        ArgumentNameSolutionPath,
                        ArgumentNameBuildConfiguration,
                        ArgumentNameMSBuildProperties,
                        ArgumentNamePublishingConfiguration
                    },
                PositionalArguments = new[]
                    {
                        ArgumentNameSolutionPath,
                        ArgumentNameBuildConfiguration,
                        ArgumentNameMSBuildProperties,
                        ArgumentNamePublishingConfiguration
                    },
                Description = Resources.PackageSolutionCommand.CommandDescriptionText,
                Usage = string.Format("{0} -{1}=<Path> -{2}=<Debug|Release> -{3}=<Property1=Value1;Property2=Value2>", CommandName, ArgumentNameSolutionPath, ArgumentNameBuildConfiguration, ArgumentNameMSBuildProperties),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} -{1}=C:\\dev\\projects\\sample\\sample.sln -{2}=Release -{3}=IsAutoBuild=True", CommandName, ArgumentNameSolutionPath, ArgumentNameBuildConfiguration, ArgumentNameMSBuildProperties),
                            Resources.PackageSolutionCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameSolutionPath, Resources.PackageSolutionCommand.ArgumentDescriptionSolutionPath },
                        { ArgumentNameBuildConfiguration, Resources.PackageSolutionCommand.ArgumentDescriptionBuildConfiguration },
                        { ArgumentNameMSBuildProperties, Resources.PackageSolutionCommand.ArgumentDescriptionMSBuildProperties },
                        { ArgumentNamePublishingConfiguration, Resources.PackageSolutionCommand.ArgumentDescriptionPublishingConfiguration }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // Solution Path Parameter
            string solutionPath = this.Arguments.ContainsKey(ArgumentNameSolutionPath) ? this.Arguments[ArgumentNameSolutionPath] : string.Empty;
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageSolutionCommand.SolutionPathArgumentNotSetMessage));
                return false;
            }

            // Build Configuration
            string buildConfiguration = this.Arguments.ContainsKey(ArgumentNameBuildConfiguration) ? this.Arguments[ArgumentNameBuildConfiguration] : string.Empty;
            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageSolutionCommand.BuildConfigurationArgumentNotSetMessage));
                return false;
            }

            // MSBuild Properties
            var buildPropertiesArgument = this.Arguments.ContainsKey(ArgumentNameMSBuildProperties) ? this.Arguments[ArgumentNameMSBuildProperties] : string.Empty;
            var buildProperties = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrWhiteSpace(buildPropertiesArgument))
            {
                buildProperties = this.buildPropertyParser.ParseBuildPropertiesArgument(buildPropertiesArgument).ToList();
            }

            // Package
            IServiceResult packagingResult = this.solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties.ToArray());
            if (packagingResult.Status == ServiceResultType.Failure)
            {
                this.userInterface.Display(packagingResult);
                this.userInterface.WriteLine(Resources.PackageSolutionCommand.PackagingFailureMessage);

                return false;
            }

            // Publish
            string packagPath = packagingResult.ResultArtefact;
            string publishConfigurationName = this.Arguments.ContainsKey(ArgumentNamePublishingConfiguration) ? this.Arguments[ArgumentNamePublishingConfiguration] : string.Empty;
            if (!string.IsNullOrWhiteSpace(publishConfigurationName))
            {
                var publishResult = this.publishingService.PublishPackage(packagPath, publishConfigurationName);
                if (publishResult.Status == ServiceResultType.Failure)
                {
                    this.userInterface.Display(publishResult);
                }

                this.userInterface.WriteLine(
                    publishResult.Status == ServiceResultType.Success
                        ? string.Format(Resources.PackageSolutionCommand.PublishingSucceededMessageTemplate, packagPath, publishConfigurationName)
                        : string.Format(Resources.PackageSolutionCommand.PublishingFailedMessageTemplate, packagPath, publishConfigurationName));
            }

            this.userInterface.WriteLine(Resources.PackageSolutionCommand.PackagingSuccessMessage);

            return true;
        }
    }
}