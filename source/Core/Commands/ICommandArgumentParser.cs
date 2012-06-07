using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public interface ICommandArgumentParser
    {
        IDictionary<string, string> ParseParameters(IEnumerable<string> commandArguments);
    }
}