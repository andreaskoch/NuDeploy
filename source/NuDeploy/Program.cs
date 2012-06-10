using System;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Common;
using NuDeploy.Core.DependencyResolution;

using StructureMap;

namespace NuDeploy
{
    public class Program
    {
        public Program()
        {
            StructureMapSetup.Setup();
        }

        public static int Main(string[] args)
        {
            var program = new Program();
            return program.Run(args);
        }

        public int Run(string[] args)
        {
            var console = ObjectFactory.GetInstance<IUserInterface>();

            try
            {
                var commandLineArgumentInterpreter = ObjectFactory.GetInstance<ICommandLineArgumentInterpreter>();
                var command = commandLineArgumentInterpreter.GetCommand(args) ?? ObjectFactory.GetInstance<HelpCommand>();

                command.Execute();
            }
            catch (Exception exception)
            {
                console.Show(exception.Message);
                console.Show(exception.StackTrace);

                return 1;
            }

            return 0;
        }
    }
}