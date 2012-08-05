namespace NuDeploy.CommandLine.Commands
{
    public interface IPublishingTargetConfigurationCommandActionParser
    {
        PublishingTargetConfigurationCommandAction ParseAction(string actionName);
    }
}