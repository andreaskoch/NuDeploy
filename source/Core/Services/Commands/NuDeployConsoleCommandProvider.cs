using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Commands.Console;

namespace NuDeploy.Core.Services.Commands
{
    public class NuDeployConsoleCommandProvider : ICommandProvider
    {
        private readonly IList<ICommand> commands;

        public NuDeployConsoleCommandProvider(InstallationStatusCommand installationStatus, InstallCommand install, UninstallCommand uninstall, CleanupCommand cleanup, PackageSolutionCommand package, HelpCommand help, RepositorySourceConfigurationCommand configureSources, SelfUpdateCommand selfUpdate)
        {
            if (installationStatus == null)
            {
                throw new ArgumentNullException("installationStatus");
            }

            if (install == null)
            {
                throw new ArgumentNullException("install");
            }

            if (uninstall == null)
            {
                throw new ArgumentNullException("uninstall");
            }

            if (cleanup == null)
            {
                throw new ArgumentNullException("cleanup");
            }

            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            if (help == null)
            {
                throw new ArgumentNullException("help");
            }

            if (configureSources == null)
            {
                throw new ArgumentNullException("configureSources");
            }

            if (selfUpdate == null)
            {
                throw new ArgumentNullException("selfUpdate");
            }

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