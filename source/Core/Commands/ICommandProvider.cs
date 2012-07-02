using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public interface ICommandProvider
    {
        IList<ICommand> GetAvailableCommands();
    }
}