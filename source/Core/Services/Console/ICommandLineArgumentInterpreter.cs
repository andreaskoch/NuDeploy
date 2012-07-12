using System.Collections.Generic;

using NuDeploy.Core.Commands;

namespace NuDeploy.Core.Services.Console
{
    public interface ICommandLineArgumentInterpreter
    {
        ICommand GetCommand(IList<string> commandLineArguments);
    }
}