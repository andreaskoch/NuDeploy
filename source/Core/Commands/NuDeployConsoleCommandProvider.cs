using System;
using System.Collections.Generic;
using System.Linq;

namespace NuDeploy.Core.Commands
{
    public class NuDeployConsoleCommandProvider : ICommandProvider
    {
        private readonly List<ICommand> commands;

        public NuDeployConsoleCommandProvider(IEnumerable<ICommand> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }

            this.commands = commands.ToList();
        }

        public IList<ICommand> GetAvailableCommands()
        {
            return this.commands;
        }
    }
}