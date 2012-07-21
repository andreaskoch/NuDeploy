using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Cleanup;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class CleanupCommand : ICommand
    {
        public const string CommandName = "cleanup";

        public const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "purge" };

        private readonly IUserInterface userInterface;

        private readonly ICleanupService cleanupService;

        public CleanupCommand(IUserInterface userInterface, ICleanupService cleanupService)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (cleanupService == null)
            {
                throw new ArgumentNullException("cleanupService");
            }

            this.userInterface = userInterface;
            this.cleanupService = cleanupService;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new[] { ArgumentNameNugetPackageId },
                Description = Resources.CleanupCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.CleanupCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} <{1}>", CommandName, ArgumentNameNugetPackageId),
                            Resources.CleanupCommand.CommandExampleDescription2
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackageId, Resources.CleanupCommand.ArgumentDescriptionNugetPackageId }
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
                this.userInterface.WriteLine(Resources.CleanupCommand.CleanupMessageAllInstalledPackages);
                this.cleanupService.Cleanup();
                return;
            }

            this.userInterface.WriteLine(string.Format(Resources.CleanupCommand.CleanupMessageTemplateSpecificPackage, packageId));
            this.cleanupService.Cleanup(packageId);
        }
    }
}