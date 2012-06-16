using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public class CommandAttributes
    {
        public CommandAttributes()
        {
            this.CommandName = string.Empty;
            this.AlternativeCommandNames = new string[] { };
            this.Description = string.Empty;
            this.ArgumentDescriptions = new Dictionary<string, string>();
        }

        public string CommandName { get; set; }

        public IEnumerable<string> AlternativeCommandNames { get; set; }

        public string Description { get; set; }

        public string Usage { get; set; }

        public IDictionary<string, string> ArgumentDescriptions { get; set; }
    }
}