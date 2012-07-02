namespace NuDeploy.Core.Commands
{
    public interface ICommandArgumentNameMatcher
    {
        bool IsMatch(string fullArgumentName, string suppliedArgumentName);
    }
}