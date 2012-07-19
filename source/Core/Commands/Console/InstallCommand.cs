using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Transformation;

namespace NuDeploy.Core.Commands.Console
{
    public class InstallCommand : ICommand
    {
        private const string CommandName = "install";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private const string ArgumentNameNugetDeploymentType = "DeploymentType";

        private const string ArgumentNameSystemSettingTransformationProfiles = "TransformationProfiles";

        private readonly string[] alternativeCommandNames = new[] { "deploy" };

        private readonly string[] allowedDeploymentTypes = Enum.GetValues(typeof(DeploymentType)).Cast<DeploymentType>().Select(d => d.ToString()).ToArray();

        private readonly IUserInterface userInterface;

        private readonly IPackageInstaller packageInstaller;

        private readonly IDeploymentTypeParser deploymentTypeParser;

        public InstallCommand(IUserInterface userInterface, IPackageInstaller packageInstaller, IDeploymentTypeParser deploymentTypeParser)
        {
            this.userInterface = userInterface;
            this.packageInstaller = packageInstaller;
            this.deploymentTypeParser = deploymentTypeParser;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[] { ArgumentNameNugetPackageId, ArgumentNameNugetDeploymentType, ArgumentNameSystemSettingTransformationProfiles },
                PositionalArguments = new[] { ArgumentNameNugetPackageId, ArgumentNameNugetDeploymentType, ArgumentNameSystemSettingTransformationProfiles },
                Description = Resources.InstallCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}> <{2}>", CommandName, ArgumentNameNugetPackageId, string.Join("|", this.allowedDeploymentTypes)),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} {1}", CommandName, NuDeployConstants.NuDeployCommandLinePackageId),
                            Resources.InstallCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} {1} {2}", CommandName, NuDeployConstants.NuDeployCommandLinePackageId, DeploymentType.Full),
                            Resources.InstallCommand.CommandExampleDescription2
                        },
                        {
                            string.Format("{0} -{1}=\"{2}\" -{3}=\"{4}\"", CommandName, ArgumentNameNugetPackageId, NuDeployConstants.NuDeployCommandLinePackageId, ArgumentNameNugetDeploymentType, DeploymentType.Full),
                            Resources.InstallCommand.CommandExampleDescription3
                        },
                        {
                            string.Format("{0} -{1}=\"{2}\" -{3}=\"{4}\"", CommandName, ArgumentNameNugetPackageId, NuDeployConstants.NuDeployCommandLinePackageId, ArgumentNameSystemSettingTransformationProfiles, "PROD-A"),
                            Resources.InstallCommand.CommandExampleDescription4
                        },
                        {
                            string.Format("{0} -{1}=\"{2}\" -{3}=\"{4}\"", CommandName, ArgumentNameNugetPackageId, NuDeployConstants.NuDeployCommandLinePackageId, ArgumentNameSystemSettingTransformationProfiles, "DB-Instance-B,Server-1"),
                            Resources.InstallCommand.CommandExampleDescription5
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.InstallCommand.ArgumentDescriptionNugetPackageId },
                        { ArgumentNameNugetDeploymentType, string.Format(Resources.InstallCommand.ArgumentDescriptionDeploymentTypeTemplate, string.Join(", ", this.allowedDeploymentTypes), DeploymentType.Full) },
                        { ArgumentNameSystemSettingTransformationProfiles, string.Format(Resources.InstallCommand.ArgumentDescriptionSystemSettingTransformationProfilesTemplate, PackageConfigurationTransformationService.TransformedSystemSettingsFileName) }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            // package id (required parameter)
            string packageId = this.Arguments.Values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(packageId))
            {
                this.userInterface.WriteLine(Resources.InstallCommand.NoPackageIdSpecifiedMessage);
                return;
            }

            // deployment type
            string deploymentTypeString = this.Arguments.ContainsKey(ArgumentNameNugetDeploymentType) ? this.Arguments[ArgumentNameNugetDeploymentType] : string.Empty;
            DeploymentType deploymentType = this.deploymentTypeParser.GetDeploymentType(deploymentTypeString);

            // system settings transformation names
            string transformationProfileNamesArgument = this.Arguments.ContainsKey(ArgumentNameSystemSettingTransformationProfiles) ? this.Arguments[ArgumentNameSystemSettingTransformationProfiles] : string.Empty;
            string[] systemSettingTransformationProfileNames = null;
            if (!string.IsNullOrWhiteSpace(transformationProfileNamesArgument))
            {
                systemSettingTransformationProfileNames =
                    transformationProfileNamesArgument.Split(NuDeployConstants.MultiValueSeperator).Where(arg => string.IsNullOrWhiteSpace(arg) == false).Select(arg => arg.Trim()).ToArray();
            }

            // options
            bool forceInstallation =
                this.Arguments.Any(
                    pair =>
                    pair.Key.Equals(NuDeployConstants.CommonCommandOptionNameForce, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase));

            // install the package
            this.packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames);
        }
    }
}