using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public interface ICommandLineArgumentParser
    {
        ICommand ParseCommandLineArguments(IEnumerable<string> commandLineArguments);
    }
}