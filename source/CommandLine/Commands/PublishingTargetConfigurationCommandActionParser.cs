using System;

namespace NuDeploy.CommandLine.Commands
{
    public class PublishingTargetConfigurationCommandActionParser : IPublishingTargetConfigurationCommandActionParser
    {
        public const PublishingTargetConfigurationCommandAction DefaultAction = PublishingTargetConfigurationCommandAction.Unrecognized;

        public PublishingTargetConfigurationCommandAction ParseAction(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                return DefaultAction;
            }

            PublishingTargetConfigurationCommandAction commandActionEnum;
            if (!Enum.TryParse(actionName.Trim(), true, out commandActionEnum))
            {
                return DefaultAction;
            }

            return commandActionEnum;
        }
    }
}