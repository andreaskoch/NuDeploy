using System;
using System.Collections.Generic;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Packaging;
using NuDeploy.Core.Services.Publishing;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class PackageBuildOutputCommand : ICommand
    {
        public const string CommandName = "packagebuildoutput";

        public const string ArgumentNameBuildOutputFolderPath = "SolutionPath";

        public const string ArgumentNamePublishingConfiguration = "PublishingConfiguration";

        private readonly string[] alternativeCommandNames = new[] { "packbuildoutput", "packbuild" };

        private readonly IUserInterface userInterface;

        private readonly IBuildOutputPackagingService buildOutputPackagingService;

        private readonly IPublishingService publishingService;

        public PackageBuildOutputCommand(IUserInterface userInterface, IBuildOutputPackagingService buildOutputPackagingService, IPublishingService publishingService)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (buildOutputPackagingService == null)
            {
                throw new ArgumentNullException("buildOutputPackagingService");
            }

            if (publishingService == null)
            {
                throw new ArgumentNullException("publishingService");
            }

            this.userInterface = userInterface;
            this.buildOutputPackagingService = buildOutputPackagingService;
            this.publishingService = publishingService;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[]
                    {
                        ArgumentNameBuildOutputFolderPath,
                        ArgumentNamePublishingConfiguration
                    },
                PositionalArguments = new[]
                    {
                        ArgumentNameBuildOutputFolderPath,
                        ArgumentNamePublishingConfiguration
                    },
                Description = Resources.PackageBuildOutputCommand.CommandDescriptionText,
                Usage = string.Format("{0} -{1}=<FolderPath>", CommandName, ArgumentNameBuildOutputFolderPath),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} -{1}=C:\\build-output", CommandName, ArgumentNameBuildOutputFolderPath),
                            Resources.PackageBuildOutputCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameBuildOutputFolderPath, Resources.PackageBuildOutputCommand.ArgumentDescriptionBuildOutputPath },
                        { ArgumentNamePublishingConfiguration, Resources.PackageBuildOutputCommand.ArgumentDescriptionPublishingConfiguration }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // Build Output Folder Path Parameter
            string buildOutputFolderPath = this.Arguments.ContainsKey(ArgumentNameBuildOutputFolderPath) ? this.Arguments[ArgumentNameBuildOutputFolderPath] : string.Empty;
            if (string.IsNullOrWhiteSpace(buildOutputFolderPath))
            {
                this.userInterface.WriteLine(string.Format(Resources.PackageBuildOutputCommand.BuildOutputPathArgumentNotSetMessage));
                return false;
            }

            // Package
            IServiceResult packagingResult = this.buildOutputPackagingService.Package(buildOutputFolderPath);
            if (packagingResult.Status == ServiceResultType.Failure)
            {
                this.userInterface.Display(packagingResult);
                this.userInterface.WriteLine(Resources.PackageBuildOutputCommand.PackagingFailureMessage);

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
                        ? string.Format(Resources.PackageBuildOutputCommand.PublishingSucceededMessageTemplate, packagPath, publishConfigurationName)
                        : string.Format(Resources.PackageBuildOutputCommand.PublishingFailedMessageTemplate, packagPath, publishConfigurationName));
            }

            this.userInterface.WriteLine(Resources.PackageBuildOutputCommand.PackagingSuccessMessage);

            return true;
        }
    }
}