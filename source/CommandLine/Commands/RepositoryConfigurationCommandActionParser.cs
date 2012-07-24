using System;

namespace NuDeploy.CommandLine.Commands
{
    public class RepositoryConfigurationCommandActionParser : IRepositoryConfigurationCommandActionParser
    {
        public const RepositoryConfigurationCommandAction DefaultAction = RepositoryConfigurationCommandAction.Unrecognized;

        public RepositoryConfigurationCommandAction ParseAction(string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                return DefaultAction;
            }

            RepositoryConfigurationCommandAction commandActionEnum;
            if (!Enum.TryParse(actionName.Trim(), true, out commandActionEnum))
            {
                return DefaultAction;
            }

            return commandActionEnum;
        }
    }
}