namespace NuDeploy.Core.Commands.Console
{
    public interface ICommandNameMatcher
    {
        bool IsMatch(ICommand command, string commandName);
    }
}