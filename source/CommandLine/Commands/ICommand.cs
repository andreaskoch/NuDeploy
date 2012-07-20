using System.Collections.Generic;

namespace NuDeploy.CommandLine.Commands
{
    public interface ICommand
    {
        CommandAttributes Attributes { get; }

        IDictionary<string, string> Arguments { get; set; }

        void Execute();
    }
}