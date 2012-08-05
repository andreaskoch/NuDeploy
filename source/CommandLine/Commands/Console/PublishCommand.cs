using System;
using System.Collections.Generic;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Publishing;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class PublishCommand : ICommand
    {
        public const string CommandName = "publish";

        public const string ArgumentNameNugetPackagePath = "NugetPackagePath";

        public const string ArgumentNamePublishConfigurationName = "PublishConfigurationName";

        private readonly string[] alternativeCommandNames = new[] { "push" };

        private readonly IUserInterface userInterface;

        private readonly IPublishingService publishingService;

        public PublishCommand(IUserInterface userInterface, IPublishingService publishingService)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (publishingService == null)
            {
                throw new ArgumentNullException("publishingService");
            }

            this.userInterface = userInterface;
            this.publishingService = publishingService;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new[] { ArgumentNameNugetPackagePath, ArgumentNamePublishConfigurationName },
                Description = Resources.PublishCommand.CommandDescriptionText,
                Usage = string.Format("{0} <{1}> <{2}>", CommandName, ArgumentNameNugetPackagePath, ArgumentNamePublishConfigurationName),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} \"{1}\" \"{2}\"", CommandName, @"C:\local-repository\SomePackage.1.0.0.nupkg", "Nuget Gallery"),
                            string.Format(Resources.PublishCommand.CommandExampleDescription1, @"C:\local-repository\SomePackage.1.0.0.nupkg", "Nuget Gallery")
                        },
                        {
                            string.Format("{0} -{1}=\"{2}\" {3}=\"{4}\"", CommandName, ArgumentNameNugetPackagePath, @"C:\local-repository\SomePackage.1.0.0.nupkg", ArgumentNamePublishConfigurationName, "Nuget Gallery"),
                            string.Format(Resources.PublishCommand.CommandExampleDescription2, @"C:\local-repository\SomePackage.1.0.0.nupkg", "Nuget Gallery")
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameNugetPackagePath, Resources.PublishCommand.ArgumentDescriptionNugetPackagePath },
                        { ArgumentNamePublishConfigurationName, Resources.PublishCommand.ArgumentDescriptionPublishConfigurationName }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // package path (required parameter)
            string packagePath = this.Arguments.ContainsKey(ArgumentNameNugetPackagePath) ? this.Arguments[ArgumentNameNugetPackagePath] : string.Empty;
            if (string.IsNullOrWhiteSpace(packagePath))
            {
                this.userInterface.WriteLine(Resources.PublishCommand.NoPackagePathSpecifiedMessage);
                return false;
            }

            // publish configuration name (required parameter)
            string publishConfigurationName = this.Arguments.ContainsKey(ArgumentNamePublishConfigurationName) ? this.Arguments[ArgumentNamePublishConfigurationName] : string.Empty;
            if (string.IsNullOrWhiteSpace(publishConfigurationName))
            {
                this.userInterface.WriteLine(Resources.PublishCommand.NoPublishConfigurationNameSpecifiedMessage);
                return false;
            }

            if (!this.publishingService.PublishPackage(packagePath, publishConfigurationName))
            {
                this.userInterface.WriteLine(string.Format(Resources.PublishCommand.PublishFailedMessageTemplate, packagePath, publishConfigurationName));
                return false;
            }

            this.userInterface.WriteLine(string.Format(Resources.PublishCommand.PublishSucceededMessageTemplate, packagePath, publishConfigurationName));
            return true;
        }
    }
}