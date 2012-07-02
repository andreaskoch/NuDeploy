using System.Collections.Generic;

using NuDeploy.Core.Commands.Console;

namespace NuDeploy.Core.Commands
{
    public interface ICommand
    {
        CommandAttributes Attributes { get; }

        IDictionary<string, string> Arguments { get; set; }

        void Execute();
    }
}