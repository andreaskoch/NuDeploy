using System.Collections.Generic;

namespace NuDeploy.Core.Commands.Console
{
    public interface ICommandLineArgumentInterpreter
    {
        ICommand GetCommand(IList<string> commandLineArguments);
    }
}