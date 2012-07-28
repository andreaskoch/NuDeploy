using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation;
using NuDeploy.Core.Services.Transformation;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class InstallCommand : ICommand
    {
        public const string CommandName = "install";

        public const string ArgumentNameNugetPackageId = "NugetPackageId";

        public const string ArgumentNameNugetDeploymentType = "DeploymentType";

        public const string ArgumentNameSystemSettingTransformationProfiles = "SystemSettingProfiles";

        public const string ArgumentNameBuildConfigurationProfiles = "BuildConfigurationProfiles";

        private readonly string[] alternativeCommandNames = new[] { "deploy" };

        private readonly string[] allowedDeploymentTypes = Enum.GetValues(typeof(DeploymentType)).Cast<DeploymentType>().Where(d => d != DeploymentType.NotRecognized).Select(d => d.ToString()).ToArray();

        private readonly IUserInterface userInterface;

        private readonly IPackageInstaller packageInstaller;

        private readonly IDeploymentTypeParser deploymentTypeParser;

        public InstallCommand(IUserInterface userInterface, IPackageInstaller packageInstaller, IDeploymentTypeParser deploymentTypeParser)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (packageInstaller == null)
            {
                throw new ArgumentNullException("packageInstaller");
            }

            if (deploymentTypeParser == null)
            {
                throw new ArgumentNullException("deploymentTypeParser");
            }

            this.userInterface = userInterface;
            this.packageInstaller = packageInstaller;
            this.deploymentTypeParser = deploymentTypeParser;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[] { ArgumentNameNugetPackageId, ArgumentNameNugetDeploymentType, ArgumentNameSystemSettingTransformationProfiles, ArgumentNameBuildConfigurationProfiles },
                PositionalArguments = new[] { ArgumentNameNugetPackageId, ArgumentNameNugetDeploymentType, ArgumentNameSystemSettingTransformationProfiles, ArgumentNameBuildConfigurationProfiles },
                Description = Resources.InstallCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}> <{2}>", CommandName, ArgumentNameNugetPackageId, string.Join("|", this.allowedDeploymentTypes)),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} {1} <{2}> \"{3}\" \"{4}\"", CommandName, "Some.Package.Name", string.Join("|", this.allowedDeploymentTypes), "PROD,Server-03", "PROD"),
                            Resources.InstallCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.InstallCommand.ArgumentDescriptionNugetPackageId },
                        { ArgumentNameNugetDeploymentType, string.Format(Resources.InstallCommand.ArgumentDescriptionDeploymentTypeTemplate, string.Join(", ", this.allowedDeploymentTypes), DeploymentType.Full) },
                        { ArgumentNameSystemSettingTransformationProfiles, string.Format(Resources.InstallCommand.ArgumentDescriptionSystemSettingTransformationProfilesTemplate, PackageConfigurationTransformationService.TransformedSystemSettingsFileName) },
                        { ArgumentNameBuildConfigurationProfiles, string.Format(Resources.InstallCommand.ArgumentDescriptionBuildConfigurationProfilesTemplate, PackageConfigurationTransformationService.TransformedSystemSettingsFileName) }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // package id (required parameter)
            string packageId = this.Arguments.ContainsKey(ArgumentNameNugetPackageId) ? this.Arguments[ArgumentNameNugetPackageId] : string.Empty;
            if (string.IsNullOrWhiteSpace(packageId))
            {
                this.userInterface.WriteLine(Resources.InstallCommand.NoPackageIdSpecifiedMessage);
                return false;
            }

            // deployment type
            string deploymentTypeString = this.Arguments.ContainsKey(ArgumentNameNugetDeploymentType) ? this.Arguments[ArgumentNameNugetDeploymentType] : string.Empty;
            DeploymentType deploymentType = this.deploymentTypeParser.GetDeploymentType(deploymentTypeString);
            if (deploymentType == DeploymentType.NotRecognized)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.InstallCommand.DeploymentTypeWasNotRecognizedMessageTemplate,
                        deploymentTypeString,
                        string.Join(", ", this.allowedDeploymentTypes)));

                return false;
            }

            // system settings transformation names
            string transformationProfileNamesArgument = this.Arguments.ContainsKey(ArgumentNameSystemSettingTransformationProfiles) ? this.Arguments[ArgumentNameSystemSettingTransformationProfiles] : string.Empty;
            var systemSettingTransformationProfileNames = new string[] { };
            if (!string.IsNullOrWhiteSpace(transformationProfileNamesArgument))
            {
                systemSettingTransformationProfileNames =
                    transformationProfileNamesArgument.Split(NuDeployConstants.MultiValueSeperator).Where(arg => string.IsNullOrWhiteSpace(arg) == false).Select(arg => arg.Trim()).ToArray();
            }

            // build configuration profiles name
            string buildConfigurationProfileNamesArgument = this.Arguments.ContainsKey(ArgumentNameBuildConfigurationProfiles) ? this.Arguments[ArgumentNameBuildConfigurationProfiles] : string.Empty;
            var buildConfigurationProfileNames = new string[] { };
            if (!string.IsNullOrWhiteSpace(buildConfigurationProfileNamesArgument))
            {
                buildConfigurationProfileNames =
                    buildConfigurationProfileNamesArgument.Split(NuDeployConstants.MultiValueSeperator).Where(arg => string.IsNullOrWhiteSpace(arg) == false).
                        Select(arg => arg.Trim()).ToArray();
            }

            // options
            bool forceInstallation =
                this.Arguments.Any(
                    pair =>
                    pair.Key.Equals(NuDeployConstants.CommonCommandOptionNameForce, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase));

            // install the package
            return this.packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileNames, buildConfigurationProfileNames);
        }
    }
}