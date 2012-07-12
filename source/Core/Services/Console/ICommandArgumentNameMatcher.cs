namespace NuDeploy.Core.Services.Console
{
    public interface ICommandArgumentNameMatcher
    {
        bool IsMatch(string fullArgumentName, string suppliedArgumentName);
    }
}