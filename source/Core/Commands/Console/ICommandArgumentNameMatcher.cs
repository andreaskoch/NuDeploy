namespace NuDeploy.Core.Commands.Console
{
    public interface ICommandArgumentNameMatcher
    {
        bool IsMatch(string fullArgumentName, string suppliedArgumentName);
    }
}