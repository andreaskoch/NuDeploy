using System;
using System.Diagnostics;

using NuDeploy.Core.Commands.Console;
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
#if DEBUG
            int processId = Process.GetCurrentProcess().Id;
            Console.WriteLine(string.Format("For debug attach to process {0} and hit <Enter>.", processId));
            Console.ReadLine();
#endif

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
                console.WriteLine(exception.Message);
                console.WriteLine(exception.StackTrace);

                return 1;
            }

            return 0;
        }
    }
}