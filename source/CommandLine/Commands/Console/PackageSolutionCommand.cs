using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Packaging;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class PackageSolutionCommand : ICommand
    {
        private const string CommandName = "package";

        private const string ArgumentNameSolutionPath = "SolutionPath";

        private const string ArgumentNameBuildConfiguration = "BuildConfiguration";

        private const string ArgumentNameMSBuildProperties = "MSBuildProperties";

        private readonly string[] alternativeCommandNames = new[] { "pack" };

        private readonly IUserInterface userInterface;

        private readonly ISolutionPackagingService solutionPackagingService;

        public PackageSolutionCommand(IUserInterface userInterface, ISolutionPackagingService solutionPackagingService)
        {
            this.userInterface = userInterface;
            this.solutionPackagingService = solutionPackagingService;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new[]
                    {
                        ArgumentNameSolutionPath,
                        ArgumentNameBuildConfiguration,
                        ArgumentNameMSBuildProperties
                    },
                PositionalArguments = new[]
                    {
                        ArgumentNameSolutionPath,
                        ArgumentNameBuildConfiguration,
                        ArgumentNameMSBuildProperties                        
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
                        { ArgumentNameMSBuildProperties, Resources.PackageSolutionCommand.ArgumentDescriptionMSBuildProperties }
                    }
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            // Solution Path Parameter
            string solutionPath = this.Arguments.ContainsKey(ArgumentNameSolutionPath) ? this.Arguments[ArgumentNameSolutionPath] : string.Empty;
            if (string.IsNullOrWhiteSpace(solutionPath))
            {
                this.userInterface.WriteLine(string.Format("You must specifiy a solution path."));
                return;
            }

            // Build Configuration
            string buildConfiguration = this.Arguments.ContainsKey(ArgumentNameBuildConfiguration) ? this.Arguments[ArgumentNameBuildConfiguration] : string.Empty;
            if (string.IsNullOrWhiteSpace(buildConfiguration))
            {
                this.userInterface.WriteLine(string.Format("You must specify a build configuration (e.g. Debug, Release)."));
                return;
            }

            // MSBuild Properties
            var buildPropertiesArgument = this.Arguments.ContainsKey(ArgumentNameMSBuildProperties) ? this.Arguments[ArgumentNameMSBuildProperties] : string.Empty;
            var buildProperties = this.ParseBuildPropertiesArgument(buildPropertiesArgument).ToList();


            if (!this.solutionPackagingService.PackageSolution(solutionPath, buildConfiguration, buildProperties))
            {
                this.userInterface.WriteLine("Packaging failed.");
                return;
            }

            this.userInterface.WriteLine("Packaging succeeded.");
        }

        private IEnumerable<KeyValuePair<string, string>> ParseBuildPropertiesArgument(string builProperties)
        {
            var keyValuePairStrings = builProperties.Split(NuDeployConstants.MultiValueSeperator).Where(p => string.IsNullOrWhiteSpace(p) == false).Select(p => p.Trim());
            foreach (var keyValuePairString in keyValuePairStrings)
            {
                var segments = keyValuePairString.Split('=');
                if (segments.Count() == 2)
                {
                    string key = segments.First();
                    string value = segments.Last();

                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }
    }
}