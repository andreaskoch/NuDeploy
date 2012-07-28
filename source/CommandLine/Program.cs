using System;
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
        public const string AppGuid = "af28a510-01c5-4e7c-b9f6-06a016c56d1c";

        public const string NuDeployMutexName = "Global\\" + AppGuid;

        private readonly ApplicationInformation applicationInformation;

        private readonly IUserInterface userInterface;

        private readonly IActionLogger logger;

        private readonly ICommandLineArgumentInterpreter commandLineArgumentInterpreter;

        private readonly HelpCommand helpCommand;

        public Program(ApplicationInformation applicationInformation, IUserInterface userInterface, IActionLogger logger, ICommandLineArgumentInterpreter commandLineArgumentInterpreter, HelpCommand helpCommand)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (commandLineArgumentInterpreter == null)
            {
                throw new ArgumentNullException("commandLineArgumentInterpreter");
            }

            if (helpCommand == null)
            {
                throw new ArgumentNullException("helpCommand");
            }

            this.applicationInformation = applicationInformation;
            this.userInterface = userInterface;
            this.logger = logger;
            this.commandLineArgumentInterpreter = commandLineArgumentInterpreter;
            this.helpCommand = helpCommand;
        }

        [STAThread]
        public static int Main(string[] args)
        {
            using (var mutex = new Mutex(false, NuDeployMutexName))
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

                StructureMapSetup.Setup();

                var program = new Program(
                    ObjectFactory.GetInstance<ApplicationInformation>(),
                    ObjectFactory.GetInstance<IUserInterface>(),
                    ObjectFactory.GetInstance<IActionLogger>(),
                    ObjectFactory.GetInstance<ICommandLineArgumentInterpreter>(),
                    ObjectFactory.GetInstance<HelpCommand>());

                return program.Run(args);                
            }
        }

        public int Run(string[] args)
        {
            var console = this.userInterface;

            try
            {
                this.logger.Log("Command: {0} {1}", this.applicationInformation.NameOfExecutable, string.Join(" ", args));

                var command = this.commandLineArgumentInterpreter.GetCommand(args) ?? this.helpCommand;
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