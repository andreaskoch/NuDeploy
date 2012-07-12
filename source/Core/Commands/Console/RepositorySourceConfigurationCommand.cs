using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Status;

namespace NuDeploy.Core.Commands.Console
{
    public class RepositorySourceConfigurationCommand : ICommand
    {
        private const string CommandName = "sources";

        private const string ArgumentNameAction = "Action";

        private const string ArgumentNameRepositoryName = "Name";

        private const string ArgumentNameRepositoryUrl = "Url";

        private const string ActionAdd = "add";

        private const string ActionDelete = "delete";

        private const string ActionReset = "reset";

        private const string ActionList = "list";

        private readonly string[] alternativeCommandNames = new[] { "repositoryconfiguration", "repoconfig", "repositorysourceconfig", "repositorysourceconfiguration" };

        private readonly string[] allowedActions = new[] { ActionAdd, ActionDelete, ActionReset, ActionList };

        private readonly IUserInterface userInterface;

        private readonly ISourceRepositoryProvider sourceRepositoryProvider;

        public RepositorySourceConfigurationCommand(IUserInterface userInterface, ISourceRepositoryProvider sourceRepositoryProvider)
        {
            this.userInterface = userInterface;
            this.sourceRepositoryProvider = sourceRepositoryProvider;

            this.Attributes = new CommandAttributes
                {
                    CommandName = CommandName,
                    AlternativeCommandNames = this.alternativeCommandNames,
                    RequiredArguments = new[] { ArgumentNameAction, ArgumentNameRepositoryName, ArgumentNameRepositoryUrl },
                    PositionalArguments = new[] { ArgumentNameAction, ArgumentNameRepositoryName, ArgumentNameRepositoryUrl },
                    Description = Resources.RepositorySourceConfigurationCommand.CommandDescriptionText,
                    Usage =
                        string.Format(
                            "{0} <{1}> <{2}> <{3}>", CommandName, string.Join("|", this.allowedActions), ArgumentNameRepositoryName, ArgumentNameRepositoryUrl),
                    Examples =
                        new Dictionary<string, string>
                            {
                                {
                                    string.Format("{0} {1} \"{2}\" \"{3}\"", CommandName, ActionAdd, Resources.RepositorySourceConfigurationCommand.SampleRepositoryName, NuDeployConstants.DefaultFeedUrl),
                                    Resources.RepositorySourceConfigurationCommand.AddCommandExampleDescriptionNamedArguments
                                    },
                                {
                                    string.Format(
                                        "{0} -{1}={2} -{3}=\"{4}\" -{5}=\"{6}\"",
                                        CommandName,
                                        ArgumentNameAction,
                                        ActionAdd,
                                        ArgumentNameRepositoryName,
                                        Resources.RepositorySourceConfigurationCommand.SampleRepositoryName,
                                        ArgumentNameRepositoryUrl,
                                        NuDeployConstants.DefaultFeedUrl),
                                    Resources.RepositorySourceConfigurationCommand.AddCommandExampleDescriptionNamedArguments
                                    },
                                {
                                    string.Format("{0} {1} \"{2}\"", CommandName, ActionDelete, Resources.RepositorySourceConfigurationCommand.SampleRepositoryName),
                                    Resources.RepositorySourceConfigurationCommand.DeleteCommandExampleDescriptionPositionalArguments
                                    },
                                {
                                    string.Format("{0} {1}", CommandName, ActionReset),
                                    Resources.RepositorySourceConfigurationCommand.ResetCommandExampleDescription
                                    },
                                {
                                    string.Format("{0} {1}", CommandName, ActionList), Resources.RepositorySourceConfigurationCommand.ListCommandExampleDescription
                                    }
                            },
                    ArgumentDescriptions =
                        new Dictionary<string, string>
                            {
                                {
                                    ArgumentNameAction,
                                    string.Format(
                                        Resources.RepositorySourceConfigurationCommand.ArgumentDescriptionRepositoryActionTemplate,
                                        string.Join(", ", this.allowedActions))
                                    },
                                { ArgumentNameRepositoryName, Resources.RepositorySourceConfigurationCommand.ArgumentDescriptionRepositoryName },
                                { ArgumentNameRepositoryUrl, Resources.RepositorySourceConfigurationCommand.ArgumentDescriptionRepositoryUrl }
                            }
                };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            // identify the action (required parameter)
            string suppliedActionName = this.Arguments.Values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(suppliedActionName) || this.allowedActions.Any(a => a.Equals(suppliedActionName, StringComparison.OrdinalIgnoreCase)) == false)
            {
                this.userInterface.WriteLine(
                    string.Format(Resources.RepositorySourceConfigurationCommand.InvalidActionNameMessageTemplate, string.Join(", ", this.allowedActions)));

                return;
            }

            if (suppliedActionName.Equals(ActionAdd, StringComparison.OrdinalIgnoreCase))
            {
                // add
                if (this.Arguments.Count != 3)
                {
                    this.userInterface.WriteLine(
                        string.Format(Resources.RepositorySourceConfigurationCommand.AddCommandInvalidArgumentCountMessageTemplate, ActionAdd, 2));

                    return;
                }

                string sourceRepositoryName = this.Arguments[ArgumentNameRepositoryName] ?? this.Arguments.Values.Skip(1).Take(1).FirstOrDefault();
                string repositoryUrlString = this.Arguments[ArgumentNameRepositoryUrl] ?? this.Arguments.Values.Skip(2).Take(1).FirstOrDefault();

                Uri sourceRepositoryUri;
                if (Uri.TryCreate(repositoryUrlString, UriKind.Absolute, out sourceRepositoryUri) == false)
                {
                    this.userInterface.WriteLine(
                        string.Format(Resources.RepositorySourceConfigurationCommand.InvalidRespositoryUrlMessageTemplate, repositoryUrlString));

                    return;
                }

                var repositoryConfiguration = new SourceRepositoryConfiguration { Name = sourceRepositoryName, Url = sourceRepositoryUri };
                this.sourceRepositoryProvider.SaveRepositoryConfiguration(repositoryConfiguration);
            }
            else if (suppliedActionName.Equals(ActionDelete, StringComparison.OrdinalIgnoreCase))
            {
                // delete
                string sourceRepositoryName = this.Arguments[ArgumentNameRepositoryName] ?? this.Arguments.Values.Skip(1).Take(1).FirstOrDefault();
                this.sourceRepositoryProvider.DeleteRepositoryConfiguration(sourceRepositoryName);
            }
            else if (suppliedActionName.Equals(ActionList, StringComparison.OrdinalIgnoreCase))
            {
                // list
                var repositoryConfigurations = this.sourceRepositoryProvider.GetRepositoryConfigurations();

                var dataToDisplay = new Dictionary<string, string>
                {
                    { Resources.RepositorySourceConfigurationCommand.SourceRepositoryConfigurationTableHeadlineColumn1, Resources.RepositorySourceConfigurationCommand.SourceRepositoryConfigurationTableHeadlineColumn2 },
                    { new string('-', Resources.RepositorySourceConfigurationCommand.SourceRepositoryConfigurationTableHeadlineColumn1.Length + 3), new string('-', Resources.RepositorySourceConfigurationCommand.SourceRepositoryConfigurationTableHeadlineColumn2.Length + 3) },
                    { string.Empty, string.Empty },
                };

                foreach (var repositoryConfiguration in repositoryConfigurations)
                {
                    dataToDisplay.Add(repositoryConfiguration.Name, repositoryConfiguration.Url.ToString());
                }

                this.userInterface.ShowKeyValueStore(dataToDisplay, 4);
            }
            else if (suppliedActionName.Equals(ActionReset, StringComparison.OrdinalIgnoreCase))
            {
                // reset
                this.sourceRepositoryProvider.ResetRepositoryConfiguration();
            }
        }
    }
}