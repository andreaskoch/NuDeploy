using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

namespace NuDeploy.Core.Commands
{
    public class CleanupCommand : ICommand
    {
        private const string CommandName = "cleanup";

        private const string ArgumentNameNugetPackageId = "NugetPackageId";

        private readonly string[] alternativeCommandNames = new[] { "purge" };

        private readonly IUserInterface userInterface;

        private readonly ICleanupService cleanupService;

        public CleanupCommand(IUserInterface userInterface, ICleanupService cleanupService)
        {
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