using System;
using System.Diagnostics;
using System.Threading;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.CommandLine.DependencyResolution;
using NuDeploy.CommandLine.Infrastructure.Console;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;

using StructureMap;

namespace NuDeploy.CommandLine
{
    public class Program
    {
        private const string AppGuid = "af28a510-01c5-4e7c-b9f6-06a016c56d1c";

        public Program()
        {
            StructureMapSetup.Setup();
        }

        [STAThread]
        public static int Main(string[] args)
        {
            using (var mutex = new Mutex(false, "Global\\" + AppGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    Console.WriteLine(Resources.Application.AnotherInstanceIsAlreadyRunning);
                    return 0;
                }

#if DEBUG
                int processId = Process.GetCurrentProcess().Id;
                Console.WriteLine(string.Format("For debug attach to process {0} and hit <Enter>.", processId));
                Console.ReadLine();
#endif

                var program = new Program();
                return program.Run(args);                
            }
        }

        public int Run(string[] args)
        {
            var console = ObjectFactory.GetInstance<IUserInterface>();
            var logger = ObjectFactory.GetInstance<IActionLogger>();
            var applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();

            try
            {
                logger.Log("Command: {0} {1}", applicationInformation.NameOfExecutable, string.Join(" ", args));

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