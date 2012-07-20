using System.Collections.Generic;
using System.Reflection;

using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Update;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class SelfUpdateCommand : ICommand
    {
        private const string CommandName = "selfupdate";

        private readonly string[] alternativeCommandNames = new[] { "update" };

        private readonly ApplicationInformation applicationInformation;

        private readonly ISelfUpdateService selfUpdateService;

        public SelfUpdateCommand(ApplicationInformation applicationInformation, ISelfUpdateService selfUpdateService)
        {
            this.applicationInformation = applicationInformation;
            this.selfUpdateService = selfUpdateService;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new string[] { },
                Description = string.Format(Resources.SelfUpdateCommand.CommandDescriptionTextTemplate, this.applicationInformation.NameOfExecutable),
                Usage = string.Format("{0}", CommandName),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.SelfUpdateCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>()
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            Assembly assembly = this.GetType().Assembly;
            this.selfUpdateService.SelfUpdate(assembly.Location, assembly.GetName().Version);
        }
    }
}