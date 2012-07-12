using System.Collections.Generic;

namespace NuDeploy.Core.Services.Console
{
    public interface ICommandArgumentParser
    {
        IEnumerable<KeyValuePair<string, string>> ParseParameters(IEnumerable<string> commandArguments);
    }
}