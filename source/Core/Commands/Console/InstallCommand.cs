using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

namespace NuDeploy.Core.Commands.Console
{
    public class InstallCommand : ICommand
    {
        private const string CommandName = "install";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private const string ArgumentNameNugetDeploymentType = "DeploymentType";

        private const string DeploymentTypeFull = "full";

        private const string DeploymentTypeUpdate = "update";

        private const string ArgumentNameSystemSettingTransformationProfile = "TransformationProfile";

        private const string DeploymentTypeDefault = DeploymentTypeFull;

        private readonly string[] alternativeCommandNames = new[] { "deploy" };

        private readonly string[] allowedDeploymentTypes = new[] { DeploymentTypeFull, DeploymentTypeUpdate };

        private readonly IUserInterface userInterface;

        private readonly IPackageInstaller packageInstaller;

        public InstallCommand(IUserInterface userInterface, IPackageInstaller packageInstaller)
        {
            this.userInterface = userInterface;
            this.packageInstaller = packageInstaller;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[] { ArgumentNameNugetPackageId, ArgumentNameNugetDeploymentType, ArgumentNameSystemSettingTransformationProfile },
                PositionalArguments = new[] { ArgumentNameNugetPackageId, ArgumentNameNugetDeploymentType, ArgumentNameSystemSettingTransformationProfile },
                Description = Resources.InstallCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}> <{2}>", CommandName, ArgumentNameNugetPackageId, string.Join("|", this.allowedDeploymentTypes)),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} {1}", CommandName, NuDeployConstants.NuDeployCommandLinePackageId),
                            Resources.InstallCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} {1} {2}", CommandName, NuDeployConstants.NuDeployCommandLinePackageId, DeploymentTypeFull),
                            Resources.InstallCommand.CommandExampleDescription2
                        },
                        {
                            string.Format("{0} -{1}=\"{2}\" -{3}=\"{4}\"", CommandName, ArgumentNameNugetPackageId, NuDeployConstants.NuDeployCommandLinePackageId, ArgumentNameNugetDeploymentType, DeploymentTypeFull),
                            Resources.InstallCommand.CommandExampleDescription3
                        },
                        {
                            string.Format("{0} -{1}=\"{2}\" -{3}=\"{4}\"", CommandName, ArgumentNameNugetPackageId, NuDeployConstants.NuDeployCommandLinePackageId, ArgumentNameSystemSettingTransformationProfile, "PROD-A"),
                            Resources.InstallCommand.CommandExampleDescription4
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.InstallCommand.ArgumentDescriptionNugetPackageId },
                        { ArgumentNameNugetDeploymentType, string.Format(Resources.InstallCommand.ArgumentDescriptionDeploymentTypeTemplate, string.Join(", ", this.allowedDeploymentTypes), DeploymentTypeDefault) },
                        { ArgumentNameSystemSettingTransformationProfile, string.Format(Resources.InstallCommand.ArgumentDescriptionSystemSettingTransformationProfileTemplate, PackageInstaller.TransformedSystemSettingsFileName) }
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
            string deploymentType = this.Arguments.ContainsKey(ArgumentNameNugetDeploymentType) ? this.Arguments[ArgumentNameNugetDeploymentType] : string.Empty;
            if (string.IsNullOrWhiteSpace(deploymentType))
            {
                deploymentType = DeploymentTypeDefault;
            }

            if (!this.allowedDeploymentTypes.Any(t => t.Equals(deploymentType, StringComparison.OrdinalIgnoreCase)))
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.InstallCommand.DeploymentTypeWasNotRecognizedMessageTemplate, deploymentType, string.Join(", ", this.allowedDeploymentTypes)));

                return;
            }

            // system settings transformation names
            string systemSettingTransformationProfileName = this.Arguments.ContainsKey(ArgumentNameSystemSettingTransformationProfile) ? this.Arguments[ArgumentNameSystemSettingTransformationProfile] : string.Empty;

            // options
            bool forceInstallation =
                this.Arguments.Any(
                    pair =>
                    pair.Key.Equals(NuDeployConstants.CommonCommandOptionNameForce, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase));

            // install the package
            this.packageInstaller.Install(packageId, deploymentType, forceInstallation, systemSettingTransformationProfileName);
        }
    }
}