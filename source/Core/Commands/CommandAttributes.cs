using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public class CommandAttributes
    {
        public string CommandName { get; set; }

        public IEnumerable<string> AlternativeCommandNames { get; set; }
    }
}