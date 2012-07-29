using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Repositories;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class RepositorySourceConfigurationCommand : ICommand
    {
        public const string CommandName = "sources";

        public const string ArgumentNameAction = "Action";

        public const string ArgumentNameRepositoryName = "Name";

        public const string ArgumentNameRepositoryUrl = "Url";

        public readonly string[] AlternativeCommandNames = new[] { "reps", "repositories", "repoconfigs" };

        public readonly string[] AllowedActions =
            Enum.GetValues(typeof(RepositoryConfigurationCommandAction)).Cast<RepositoryConfigurationCommandAction>().Where(
                d => d != RepositoryConfigurationCommandAction.Unrecognized).Select(d => d.ToString()).ToArray();

        private readonly IUserInterface userInterface;

        private readonly IRepositoryConfigurationCommandActionParser repositoryConfigurationCommandActionParser;

        private readonly ISourceRepositoryProvider sourceRepositoryProvider;

        public RepositorySourceConfigurationCommand(IUserInterface userInterface, IRepositoryConfigurationCommandActionParser repositoryConfigurationCommandActionParser, ISourceRepositoryProvider sourceRepositoryProvider)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (repositoryConfigurationCommandActionParser == null)
            {
                throw new ArgumentNullException("repositoryConfigurationCommandActionParser");
            }

            if (sourceRepositoryProvider == null)
            {
                throw new ArgumentNullException("sourceRepositoryProvider");
            }

            this.userInterface = userInterface;
            this.repositoryConfigurationCommandActionParser = repositoryConfigurationCommandActionParser;
            this.sourceRepositoryProvider = sourceRepositoryProvider;

            this.Attributes = new CommandAttributes
                {
                    CommandName = CommandName,
                    AlternativeCommandNames = this.AlternativeCommandNames,
                    RequiredArguments = new[] { ArgumentNameAction, ArgumentNameRepositoryName, ArgumentNameRepositoryUrl },
                    PositionalArguments = new[] { ArgumentNameAction, ArgumentNameRepositoryName, ArgumentNameRepositoryUrl },
                    Description = Resources.RepositorySourceConfigurationCommand.CommandDescriptionText,
                    Usage =
                        string.Format(
                            "{0} <{1}> <{2}> <{3}>", CommandName, string.Join("|", this.AllowedActions), ArgumentNameRepositoryName, ArgumentNameRepositoryUrl),
                    Examples =
                        new Dictionary<string, string>
                            {
                                {
                                    string.Format(
                                        "{0} {1} \"{2}\" \"{3}\"",
                                        CommandName,
                                        RepositoryConfigurationCommandAction.Add,
                                        Resources.RepositorySourceConfigurationCommand.SampleRepositoryName,
                                        NuDeployConstants.DefaultFeedUrl),
                                    Resources.RepositorySourceConfigurationCommand.AddCommandExampleDescriptionNamedArguments
                                    },
                                {
                                    string.Format(
                                        "{0} -{1}={2} -{3}=\"{4}\" -{5}=\"{6}\"",
                                        CommandName,
                                        ArgumentNameAction,
                                        RepositoryConfigurationCommandAction.Add,
                                        ArgumentNameRepositoryName,
                                        Resources.RepositorySourceConfigurationCommand.SampleRepositoryName,
                                        ArgumentNameRepositoryUrl,
                                        NuDeployConstants.DefaultFeedUrl),
                                    Resources.RepositorySourceConfigurationCommand.AddCommandExampleDescriptionNamedArguments
                                    },
                                {
                                    string.Format(
                                        "{0} {1} \"{2}\"",
                                        CommandName,
                                        RepositoryConfigurationCommandAction.Delete,
                                        Resources.RepositorySourceConfigurationCommand.SampleRepositoryName),
                                    Resources.RepositorySourceConfigurationCommand.DeleteCommandExampleDescriptionPositionalArguments
                                    },
                                {
                                    string.Format("{0} {1}", CommandName, RepositoryConfigurationCommandAction.Reset),
                                    Resources.RepositorySourceConfigurationCommand.ResetCommandExampleDescription
                                    },
                                {
                                    string.Format("{0} {1}", CommandName, RepositoryConfigurationCommandAction.List),
                                    Resources.RepositorySourceConfigurationCommand.ListCommandExampleDescription
                                    }
                            },
                    ArgumentDescriptions =
                        new Dictionary<string, string>
                            {
                                {
                                    ArgumentNameAction,
                                    string.Format(
                                        Resources.RepositorySourceConfigurationCommand.ArgumentDescriptionRepositoryActionTemplate,
                                        string.Join(", ", this.AllowedActions))
                                    },
                                { ArgumentNameRepositoryName, Resources.RepositorySourceConfigurationCommand.ArgumentDescriptionRepositoryName },
                                { ArgumentNameRepositoryUrl, Resources.RepositorySourceConfigurationCommand.ArgumentDescriptionRepositoryUrl }
                            }
                };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public bool Execute()
        {
            // identify the action (required parameter)
            string suppliedActionName = this.Arguments.ContainsKey(ArgumentNameAction) ? this.Arguments[ArgumentNameAction] : string.Empty;
            var action = this.repositoryConfigurationCommandActionParser.ParseAction(suppliedActionName);

            switch (action)
            {
                case RepositoryConfigurationCommandAction.Unrecognized:
                {
                    this.userInterface.WriteLine(
                        string.Format(Resources.RepositorySourceConfigurationCommand.InvalidActionNameMessageTemplate, string.Join(", ", this.AllowedActions)));

                    return false;
                }

                case RepositoryConfigurationCommandAction.Add:
                {
                    if (!this.Arguments.ContainsKey(ArgumentNameRepositoryName) || !this.Arguments.ContainsKey(ArgumentNameRepositoryUrl))
                    {
                        // abort
                        this.userInterface.WriteLine(
                            string.Format(string.Format(Resources.RepositorySourceConfigurationCommand.AddCommandInvalidArgumentCountMessageTemplate, RepositoryConfigurationCommandAction.Add, 2)));

                        return false;
                    }

                    string sourceRepositoryName = this.Arguments[ArgumentNameRepositoryName];
                    string repositoryUrlString = this.Arguments[ArgumentNameRepositoryUrl];
                    if (!this.sourceRepositoryProvider.SaveRepositoryConfiguration(sourceRepositoryName, repositoryUrlString))
                    {
                        // failure
                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.RepositorySourceConfigurationCommand.SaveSourceRepositoryConfigurationFailedMessageTemplate,
                                sourceRepositoryName,
                                repositoryUrlString));

                        return false;
                    }

                    // success
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.RepositorySourceConfigurationCommand.SaveSourceRepositoryConfigurationSucceededMessageTemplate,
                            sourceRepositoryName,
                            repositoryUrlString));

                    return true;
                }

                case RepositoryConfigurationCommandAction.Delete:
                {
                    if (!this.Arguments.ContainsKey(ArgumentNameRepositoryName))
                    {
                        // abort
                        this.userInterface.WriteLine(
                            string.Format(Resources.RepositorySourceConfigurationCommand.DeleteSourceRepositoryConfigurationNoRepositoryNameSuppliedMessage));

                        return false;
                    }

                    string sourceRepositoryName = this.Arguments[ArgumentNameRepositoryName] ?? this.Arguments.Values.Skip(1).Take(1).FirstOrDefault();
                    if (!this.sourceRepositoryProvider.DeleteRepositoryConfiguration(sourceRepositoryName))
                    {
                        // failure
                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.RepositorySourceConfigurationCommand.DeleteSourceRepositoryConfigurationFailedMessageTemplate, sourceRepositoryName));

                        return false;
                    }

                    // success
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.RepositorySourceConfigurationCommand.DeleteSourceRepositoryConfigurationSucceededMessageTemplate, sourceRepositoryName));

                    return true;
                }

                case RepositoryConfigurationCommandAction.List:
                {
                    var repositoryConfigurations = this.sourceRepositoryProvider.GetRepositoryConfigurations().ToList();
                    if (repositoryConfigurations.Count == 0)
                    {
                        this.userInterface.WriteLine(Resources.RepositorySourceConfigurationCommand.ListSourceRepositoryConfigurationsNoRepositoriesConfiguredMessage);
                        return true;
                    }

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
                    return true;
                }

                case RepositoryConfigurationCommandAction.Reset:
                {
                    if (!this.sourceRepositoryProvider.ResetRepositoryConfiguration())
                    {
                        this.userInterface.WriteLine(Resources.RepositorySourceConfigurationCommand.ResetSourceRepositoryConfigurationFailedMessage);
                        return false;
                    }

                    this.userInterface.WriteLine(Resources.RepositorySourceConfigurationCommand.ResetSourceRepositoryConfigurationSuccessMessage);
                    return true;
                }
            }

            return false;
        }
    }
}