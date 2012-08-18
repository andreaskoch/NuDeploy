using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
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

        public bool Execute()
        {
            // package id (required parameter)
            string packageId = this.Arguments.Values.FirstOrDefault();

            // package specific cleanup
            if (string.IsNullOrWhiteSpace(packageId))
            {
                IServiceResult generalCleanupResult = this.cleanupService.Cleanup();
                if (generalCleanupResult.Status == ServiceResultType.Failure)
                {
                    this.userInterface.Display(generalCleanupResult);
                    this.userInterface.WriteLine(string.Format(Resources.CleanupCommand.PackageCleanupFailedMessageTemplate, packageId));

                    return false;
                }

                this.userInterface.WriteLine(string.Format(Resources.CleanupCommand.PackageCleanupSucceededMessageTemplate, packageId));
                return true;
            }
            
            // general cleanup
            IServiceResult packageSpecificCleanupResult = this.cleanupService.Cleanup(packageId);
            if (packageSpecificCleanupResult.Status == ServiceResultType.Failure)
            {
                this.userInterface.Display(packageSpecificCleanupResult);
                this.userInterface.WriteLine(Resources.CleanupCommand.GeneralCleanupFailed);

                return false;
            }

            this.userInterface.WriteLine(Resources.CleanupCommand.GeneralCleanupSucceeded);
            return true;
        }
    }
}