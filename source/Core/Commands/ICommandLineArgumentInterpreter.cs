using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public interface ICommandLineArgumentInterpreter
    {
        ICommand GetCommand(IList<string> commandLineArguments);
    }
}