using System.Collections.Generic;

namespace NuDeploy.CommandLine.Commands
{
    public class CommandAttributes
    {
        public CommandAttributes()
        {
            this.CommandName = string.Empty;
            this.AlternativeCommandNames = new string[] { };
            this.RequiredArguments = new List<string>();
            this.PositionalArguments = new List<string>();
            this.Description = string.Empty;
            this.ArgumentDescriptions = new Dictionary<string, string>();
            this.Examples = new Dictionary<string, string>();
        }

        public string CommandName { get; set; }

        public IEnumerable<string> AlternativeCommandNames { get; set; }

        public IEnumerable<string> RequiredArguments { get; set; }

        public IEnumerable<string> PositionalArguments { get; set; }

        public string Description { get; set; }

        public string Usage { get; set; }

        public IDictionary<string, string> ArgumentDescriptions { get; set; }

        public IDictionary<string, string> Examples { get; set; }
    }
}