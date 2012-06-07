namespace NuDeploy.Core.Commands
{
    public interface ICommandNameMatcher
    {
        bool IsMatch(ICommand command, string commandName);
    }
}