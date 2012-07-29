using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Update;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class SelfUpdateCommand : ICommand
    {
        public const string CommandName = "selfupdate";

        private readonly string[] alternativeCommandNames = new[] { "update" };

        private readonly ApplicationInformation applicationInformation;

        private readonly ISelfUpdateService selfUpdateService;

        private readonly _Assembly targetAssembly;

        public SelfUpdateCommand(ApplicationInformation applicationInformation, ISelfUpdateService selfUpdateService, _Assembly targetAssembly)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (selfUpdateService == null)
            {
                throw new ArgumentNullException("selfUpdateService");
            }

            if (targetAssembly == null)
            {
                throw new ArgumentNullException("targetAssembly");
            }

            this.applicationInformation = applicationInformation;
            this.selfUpdateService = selfUpdateService;
            this.targetAssembly = targetAssembly;

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

        public bool Execute()
        {
            return this.selfUpdateService.SelfUpdate(this.targetAssembly.Location, this.targetAssembly.GetName().Version);
        }
    }
}