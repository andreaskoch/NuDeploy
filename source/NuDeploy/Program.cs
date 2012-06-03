using System;

using NuDeploy.Core.Commands;
using NuDeploy.Core.Common;

namespace NuDeploy
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var console = new ConsoleUserInterface();

            try
            {
                var commandLineArgumentParser = new CommandLineArgumentParser();
                var command = commandLineArgumentParser.ParseCommandLineArguments(args);

                command.Execute();
            }
            catch (Exception exception)
            {
                console.Show(exception.Message);

                return 1;
            }

            return 0;
        }
    }
}