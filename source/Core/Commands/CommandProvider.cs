using System.Collections.Generic;
using System.Linq;

namespace NuDeploy.Core.Commands
{
    public class CommandProvider : ICommandProvider
    {
        private readonly List<ICommand> commands;

        public CommandProvider(IEnumerable<ICommand> commands)
        {
            this.commands = commands.ToList();
        }

        public IList<ICommand> GetAvailableCommands()
        {
            return this.commands;
        }
    }
}