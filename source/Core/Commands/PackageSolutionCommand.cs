using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public class PackageSolutionCommand : ICommand
    {
        public PackageSolutionCommand()
        {
            this.Attributes = new CommandAttributes
            {
                CommandName = "package",
                AlternativeCommandNames = new[] { "pack" }
            };

            this.Arguments = new Dictionary<string, string>
                {
                    { "SolutionPath", null },
                    { "BuildConfiguration", null },
                    { "MSBuildProperties", null }
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