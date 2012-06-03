using System;
using System.Collections.Generic;
using System.Linq;

namespace NuDeploy.Core.Commands
{
    public class CommandLineArgumentParser : ICommandLineArgumentParser
    {
        public ICommand ParseCommandLineArguments(IEnumerable<string> commandLineArguments)
        {
            if (commandLineArguments == null || !commandLineArguments.Any())
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }
}