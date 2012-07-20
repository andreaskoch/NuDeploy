namespace NuDeploy.CommandLine.Infrastructure.Console
{
    public interface ICommandArgumentNameMatcher
    {
        bool IsMatch(string fullArgumentName, string suppliedArgumentName);
    }
}