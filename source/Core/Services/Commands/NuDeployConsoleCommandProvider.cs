using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Commands.Console;

namespace NuDeploy.Core.Services.Commands
{
    public class NuDeployConsoleCommandProvider : ICommandProvider
    {
        private readonly List<ICommand> commands;

        public NuDeployConsoleCommandProvider(InstallationStatusCommand installationStatus, InstallCommand install, UninstallCommand uninstall, CleanupCommand cleanup, PackageSolutionCommand package, HelpCommand help, RepositorySourceConfigurationCommand configureSources, SelfUpdateCommand selfUpdate)
        {
            this.commands = new List<ICommand> { installationStatus, install, uninstall, cleanup, package, help, configureSources, selfUpdate };
        }

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