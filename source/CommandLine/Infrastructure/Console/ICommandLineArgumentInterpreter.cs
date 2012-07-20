using System.Collections.Generic;

using NuDeploy.CommandLine.Commands;

namespace NuDeploy.CommandLine.Infrastructure.Console
{
    public interface ICommandLineArgumentInterpreter
    {
        ICommand GetCommand(IList<string> commandLineArguments);
    }
}