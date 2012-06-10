using System.Collections.Generic;
using System.Reflection;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Common;

using StructureMap;

namespace NuDeploy.Core.DependencyResolution
{
    public static class StructureMapSetup
    {
        public static void Setup()
        {
            var applicationInformation = new ApplicationInformation
                {
                    ApplicationName = "NuDeploy", 
                    NameOfExecutable = "NuDeploy.exe",
                    ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version
                };

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
                        config.For<ApplicationInformation>().Use(applicationInformation);
                    });

            ObjectFactory.Configure(
                config =>
                    {
                        var helpCommand = new HelpCommand(ObjectFactory.GetInstance<IUserInterface>(), applicationInformation);
                        config.For<HelpCommand>().Use(helpCommand);
                    });

            ObjectFactory.Configure(
                config =>
                    {
                        var commands = new List<ICommand> { ObjectFactory.GetInstance<HelpCommand>(), new PackageSolutionCommand() };
                        ICommandProvider commandProvider = new CommandProvider(commands);
                        config.For<ICommandProvider>().Use(commandProvider);
                    });
        }
    }
}
