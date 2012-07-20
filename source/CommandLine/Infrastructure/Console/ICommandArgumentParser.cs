using System.Collections.Generic;

namespace NuDeploy.CommandLine.Infrastructure.Console
{
    public interface ICommandArgumentParser
    {
        IEnumerable<KeyValuePair<string, string>> ParseParameters(IEnumerable<string> commandArguments);
    }
}