using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Installation;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class UninstallCommand : ICommand
    {
        public const string CommandName = "uninstall";

        public const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "remove" };

        private readonly IUserInterface userInterface;

        private readonly IPackageUninstaller packageUninstaller;

        public UninstallCommand(IUserInterface userInterface, IPackageUninstaller packageUninstaller)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (packageUninstaller == null)
            {
                throw new ArgumentNullException("packageUninstaller");
            }

            this.userInterface = userInterface;
            this.packageUninstaller = packageUninstaller;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[] { ArgumentNameNugetPackageId },
                PositionalArguments = new[] { ArgumentNameNugetPackageId },
                Description = Resources.UninstallCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                            Resources.UninstallCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.UninstallCommand.ArgumentDescriptionNugetPackageId }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // package id (required parameter)
            string packageId = this.Arguments.Values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(packageId))
            {
                this.userInterface.WriteLine(Resources.UninstallCommand.NoPackageIdSpecifiedMessage);
                return false;
            }

            IServiceResult uninstallResult = this.packageUninstaller.Uninstall(packageId, null);
            if (uninstallResult.Status == ServiceResultType.Failure)
            {
                this.userInterface.Display(uninstallResult);
                this.userInterface.WriteLine(string.Format(Resources.UninstallCommand.UninstallFailedMessageTemplate, packageId));
                return false;
            }

            this.userInterface.WriteLine(string.Format(Resources.UninstallCommand.UninstallSucceededMessageTemplate, packageId));
            return true;
        }
    }
}