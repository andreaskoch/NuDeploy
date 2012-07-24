namespace NuDeploy.CommandLine.Commands
{
    public interface IRepositoryConfigurationCommandActionParser
    {
        RepositoryConfigurationCommandAction ParseAction(string actionName);
    }
}