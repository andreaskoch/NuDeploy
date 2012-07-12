using System.Collections.Generic;

using NuDeploy.Core.Commands;

namespace NuDeploy.Core.Services.Commands
{
    public interface ICommandProvider
    {
        IList<ICommand> GetAvailableCommands();
    }
}