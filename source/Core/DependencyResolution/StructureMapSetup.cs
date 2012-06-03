using System.Collections.Generic;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Common;

using StructureMap;

namespace NuDeploy.Core.DependencyResolution
{
    public static class StructureMapSetup
    {
        public static void Setup()
        {
            ObjectFactory.Configure(
                config =>
                    {
                        config.Scan(
                            scan =>
                                {
                                    scan.TheCallingAssembly();
                                    scan.WithDefaultConventions();
                                });

                        config.For<IUserInterface>().Use<ConsoleUserInterface>();
                    });

            ObjectFactory.Configure(
                config =>
                    {
                        var commands = new List<ICommand> { new HelpCommand(ObjectFactory.GetInstance<IUserInterface>()), new PackageSolutionCommand() };
                        ICommandProvider commandProvider = new CommandProvider(commands);
                        config.For<ICommandProvider>().Use(commandProvider);
                    });
        }
    }
}
