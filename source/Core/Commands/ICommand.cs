using System.Collections.Generic;

namespace NuDeploy.Core.Commands
{
    public interface ICommand
    {
        CommandAttributes Attributes { get; }

        IList<string> Arguments { get; }

        void Execute();
    }
}