using System.Collections.Generic;

namespace NuDeploy.CommandLine.Commands
{
    public interface IHelpProvider
    {
        void ShowHelp(ICommand command);

        void ShowHelpOverview(IEnumerable<ICommand> commands);
    }
}