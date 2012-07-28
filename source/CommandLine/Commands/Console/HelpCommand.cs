using System;
using System.Collections.Generic;
using System.Linq;

using StructureMap;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class HelpCommand : ICommand
    {
        public const string CommandName = "help";

        public const string ArgumentNameCommandName = "CommandName";

        private readonly string[] alternativeCommandNames = new[] { "?" };

        private readonly IHelpProvider helpProvider;

        private IList<ICommand> availableCommands;

        public HelpCommand(IHelpProvider helpProvider)
        {
            if (helpProvider == null)
            {
                throw new ArgumentNullException("helpProvider");
            }

            this.helpProvider = helpProvider;

            this.Attributes = new CommandAttributes
                {
                    CommandName = CommandName,
                    AlternativeCommandNames = this.alternativeCommandNames,
                    RequiredArguments = new[] { ArgumentNameCommandName },
                    PositionalArguments = new[] { ArgumentNameCommandName },
                    Description = Resources.HelpCommand.CommandDescriptionText,
                    Usage = string.Format("{0} <{1}>", CommandName, ArgumentNameCommandName),
                    Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.HelpCommand.CommandExampleDescription1
                        },
                        {
                            string.Format("{0} {1}", CommandName, "package"),
                            Resources.HelpCommand.CommandExampleDescription2
                        }
                    },
                    ArgumentDescriptions = new Dictionary<string, string>
                    {
                        { ArgumentNameCommandName, Resources.HelpCommand.ArgumentDescriptionCommandName }
                    }
                };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        protected IList<ICommand> AvailableCommands
        {
            get
            {
                if (this.availableCommands == null)
                {
                    this.availableCommands = ObjectFactory.GetInstance<ICommandProvider>().GetAvailableCommands();
                }

                return this.availableCommands;
            }
        }

        public bool Execute()
        {
            string commandName = this.Arguments.Values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(commandName) == false)
            {
                ICommand matchedCommand =
                    this.AvailableCommands.FirstOrDefault(
                        c =>
                        c.Attributes.CommandName.Equals(commandName, StringComparison.OrdinalIgnoreCase)
                        || c.Attributes.AlternativeCommandNames.Any(a => a.Equals(commandName, StringComparison.OrdinalIgnoreCase)));

                if (matchedCommand != null)
                {
                    this.helpProvider.ShowHelp(matchedCommand);
                    return true;
                }                
            }

            this.helpProvider.ShowHelpOverview(this.AvailableCommands);
            return true;
        }
    }
}