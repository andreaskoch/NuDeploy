using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public class PackageSolutionCommand : ICommand
    {
        private const string CommandName = "package";

        private const string ArgumentNameSolutionPath = "SolutionPath";

        private const string ArgumentNameBuildConfiguration = "BuildConfiguration";

        private const string ArgumentNameMSBuildProperties = "MSBuildProperties";

        private readonly string[] alternativeCommandNames = new[] { "pack" };

        public PackageSolutionCommand()
        {
            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                Description = Resources.PackageSolutionCommand.CommandDescriptionText,
                Usage = string.Format("{0} -{1}=<Path> -{2}=<Debug|Release> -{3}=<Property1=Value1;Property2=Value2>", CommandName, ArgumentNameSolutionPath, ArgumentNameBuildConfiguration, ArgumentNameMSBuildProperties),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0} -{1}=C:\\dev\\projects\\sample\\sample.sln -{2}=Release -{3}=IsAutoBuild=True", CommandName, ArgumentNameSolutionPath, ArgumentNameBuildConfiguration, ArgumentNameMSBuildProperties),
                            "Creates a nuget deployment package from the solution"
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameSolutionPath, Resources.PackageSolutionCommand.ArgumentDescriptionSolutionPath },
                        { ArgumentNameBuildConfiguration, Resources.PackageSolutionCommand.ArgumentDescriptionBuildConfiguration },
                        { ArgumentNameMSBuildProperties, Resources.PackageSolutionCommand.ArgumentDescriptionMSBuildProperties }
                    }
            };

            this.Arguments = new Dictionary<string, string>
                {
                    { ArgumentNameSolutionPath, null },
                    { ArgumentNameBuildConfiguration, null },
                    { ArgumentNameMSBuildProperties, null }
                };
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}