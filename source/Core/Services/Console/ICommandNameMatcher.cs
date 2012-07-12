using NuDeploy.Core.Commands;

namespace NuDeploy.Core.Services.Console
{
    public interface ICommandNameMatcher
    {
        bool IsMatch(ICommand command, string commandName);
    }
}