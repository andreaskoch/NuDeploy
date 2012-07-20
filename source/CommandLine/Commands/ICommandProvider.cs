using System.Collections.Generic;

namespace NuDeploy.CommandLine.Commands
{
    public interface ICommandProvider
    {
        IList<ICommand> GetAvailableCommands();
    }
}