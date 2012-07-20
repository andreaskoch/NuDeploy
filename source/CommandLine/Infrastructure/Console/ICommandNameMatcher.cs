using NuDeploy.CommandLine.Commands;

namespace NuDeploy.CommandLine.Infrastructure.Console
{
    public interface ICommandNameMatcher
    {
        bool IsMatch(ICommand command, string commandName);
    }
}