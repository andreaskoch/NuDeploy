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
        }

        public string CommandName { get; set; }

        public IEnumerable<string> AlternativeCommandNames { get; set; }

        public string Description { get; set; }
    }
}