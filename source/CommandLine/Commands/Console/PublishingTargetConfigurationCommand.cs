using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;
using NuDeploy.Core.Services.Publishing;

namespace NuDeploy.CommandLine.Commands.Console
{
    public class PublishingTargetConfigurationCommand : ICommand
    {
        public const string CommandName = "targets";

        public const string ArgumentNameAction = "Action";

        public const string ArgumentNamePublishConfigurationName = "Name";

        public const string ArgumentNamePublishLocation = "Location";

        public const string ArgumentNameApiKey = "ApiKey";

        public readonly string[] AlternativeCommandNames = new[] { "publishingtargets", "push" };

        public readonly string[] AllowedActions =
            Enum.GetValues(typeof(PublishingTargetConfigurationCommandAction)).Cast<PublishingTargetConfigurationCommandAction>().Where(
                d => d != PublishingTargetConfigurationCommandAction.Unrecognized).Select(d => d.ToString()).ToArray();

        private readonly IUserInterface userInterface;

        private readonly IPublishingTargetConfigurationCommandActionParser publishingTargetConfigurationCommandActionParser;

        private readonly IPublishConfigurationAccessor publishConfigurationAccessor;

        public PublishingTargetConfigurationCommand(IUserInterface userInterface, IPublishingTargetConfigurationCommandActionParser publishingTargetConfigurationCommandActionParser, IPublishConfigurationAccessor publishConfigurationAccessor)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (publishingTargetConfigurationCommandActionParser == null)
            {
                throw new ArgumentNullException("publishingTargetConfigurationCommandActionParser");
            }

            if (publishConfigurationAccessor == null)
            {
                throw new ArgumentNullException("publishConfigurationAccessor");
            }

            this.userInterface = userInterface;
            this.publishingTargetConfigurationCommandActionParser = publishingTargetConfigurationCommandActionParser;
            this.publishConfigurationAccessor = publishConfigurationAccessor;

            this.Attributes = new CommandAttributes
                {
                    CommandName = CommandName,
                    AlternativeCommandNames = this.AlternativeCommandNames,
                    RequiredArguments = new[] { ArgumentNameAction, ArgumentNamePublishConfigurationName, ArgumentNamePublishLocation, ArgumentNameApiKey },
                    PositionalArguments = new[] { ArgumentNameAction, ArgumentNamePublishConfigurationName, ArgumentNamePublishLocation, ArgumentNameApiKey },
                    Description = Resources.PublishingTargetConfigurationCommand.CommandDescriptionText,
                    Usage =
                        string.Format(
                            "{0} <{1}> <{2}> <{3}>", CommandName, string.Join("|", this.AllowedActions), ArgumentNamePublishConfigurationName, ArgumentNamePublishLocation),
                    Examples =
                        new Dictionary<string, string>
                            {
                                {
                                    string.Format(
                                        "{0} {1} \"{2}\" \"{3}\" \"{4}\"",
                                        CommandName,
                                        PublishingTargetConfigurationCommandAction.Add,
                                        Resources.PublishingTargetConfigurationCommand.SamplePublishingTargetName,
                                        Resources.PublishingTargetConfigurationCommand.SamplePublishingLocation,
                                        Resources.PublishingTargetConfigurationCommand.SampleApiKey),
                                    Resources.PublishingTargetConfigurationCommand.AddCommandExampleDescriptionNamedArguments
                                    },
                                {
                                    string.Format(
                                        "{0} -{1}={2} -{3}=\"{4}\" -{5}=\"{6}\" -{7}=\"{8}\"",
                                        CommandName,
                                        ArgumentNameAction,
                                        PublishingTargetConfigurationCommandAction.Add,
                                        ArgumentNamePublishConfigurationName,
                                        Resources.PublishingTargetConfigurationCommand.SamplePublishingTargetName,
                                        ArgumentNamePublishLocation,
                                        Resources.PublishingTargetConfigurationCommand.SamplePublishingLocation,
                                        ArgumentNameApiKey,
                                        Resources.PublishingTargetConfigurationCommand.SampleApiKey),
                                    Resources.PublishingTargetConfigurationCommand.AddCommandExampleDescriptionNamedArguments
                                    },
                                {
                                    string.Format(
                                        "{0} {1} \"{2}\"",
                                        CommandName,
                                        PublishingTargetConfigurationCommandAction.Delete,
                                        Resources.PublishingTargetConfigurationCommand.SamplePublishingTargetName),
                                    Resources.PublishingTargetConfigurationCommand.DeleteCommandExampleDescriptionPositionalArguments
                                    },
                                {
                                    string.Format("{0} {1}", CommandName, PublishingTargetConfigurationCommandAction.Reset),
                                    Resources.PublishingTargetConfigurationCommand.ResetCommandExampleDescription
                                    },
                                {
                                    string.Format("{0} {1}", CommandName, PublishingTargetConfigurationCommandAction.List),
                                    Resources.PublishingTargetConfigurationCommand.ListCommandExampleDescription
                                    }
                            },
                    ArgumentDescriptions =
                        new Dictionary<string, string>
                            {
                                {
                                    ArgumentNameAction,
                                    string.Format(
                                        Resources.PublishingTargetConfigurationCommand.ArgumentDescriptionPublishingTargetActionTemplate,
                                        string.Join(", ", this.AllowedActions))
                                    },
                                { ArgumentNamePublishConfigurationName, Resources.PublishingTargetConfigurationCommand.ArgumentDescriptionPublishConfigurationName },
                                { ArgumentNamePublishLocation, Resources.PublishingTargetConfigurationCommand.ArgumentDescriptionPublishLocation },
                                { ArgumentNameApiKey, Resources.PublishingTargetConfigurationCommand.ArgumentDescriptionApiKey }
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
            var action = this.publishingTargetConfigurationCommandActionParser.ParseAction(suppliedActionName);

            switch (action)
            {
                case PublishingTargetConfigurationCommandAction.Add:
                {
                    if (!this.Arguments.ContainsKey(ArgumentNamePublishConfigurationName) || !this.Arguments.ContainsKey(ArgumentNamePublishLocation))
                    {
                        // abort
                        this.userInterface.WriteLine(
                            string.Format(string.Format(Resources.PublishingTargetConfigurationCommand.AddCommandInvalidArgumentCountMessageTemplate, PublishingTargetConfigurationCommandAction.Add, 2)));

                        return false;
                    }

                    string publishConfigurationName = this.Arguments[ArgumentNamePublishConfigurationName];
                    string publishLocation = this.Arguments[ArgumentNamePublishLocation];
                    string apiKey = this.Arguments.ContainsKey(ArgumentNameApiKey) ? this.Arguments[ArgumentNameApiKey] : null;

                    IServiceResult addResult = this.publishConfigurationAccessor.AddOrUpdatePublishConfiguration(
                        publishConfigurationName, publishLocation, apiKey);

                    if (addResult.Status == ServiceResultType.Failure)
                    {
                        // failure
                        this.userInterface.Display(addResult);

                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.PublishingTargetConfigurationCommand.SavePublishingConfigurationFailedMessageTemplate,
                                publishConfigurationName,
                                publishLocation));

                        return false;
                    }

                    // success
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.PublishingTargetConfigurationCommand.SavePublishingConfigurationSucceededMessageTemplate,
                            publishConfigurationName,
                            publishLocation));

                    return true;
                }

                case PublishingTargetConfigurationCommandAction.Delete:
                {
                    if (!this.Arguments.ContainsKey(ArgumentNamePublishConfigurationName))
                    {
                        // abort
                        this.userInterface.WriteLine(
                            string.Format(Resources.PublishingTargetConfigurationCommand.DeletePublishingConfigurationNoNameSuppliedMessage));

                        return false;
                    }

                    string publishConfigurationName = this.Arguments[ArgumentNamePublishConfigurationName] ?? this.Arguments.Values.Skip(1).Take(1).FirstOrDefault();

                    IServiceResult deleteResult = this.publishConfigurationAccessor.DeletePublishConfiguration(publishConfigurationName);
                    if (deleteResult.Status == ServiceResultType.Failure)
                    {
                        // failure
                        this.userInterface.Display(deleteResult);

                        this.userInterface.WriteLine(
                            string.Format(
                                Resources.PublishingTargetConfigurationCommand.DeletePublishingConfigurationFailedMessageTemplate, publishConfigurationName));

                        return false;
                    }

                    // success
                    this.userInterface.WriteLine(
                        string.Format(
                            Resources.PublishingTargetConfigurationCommand.DeletePublishingConfigurationSucceededMessageTemplate, publishConfigurationName));

                    return true;
                }

                case PublishingTargetConfigurationCommandAction.List:
                {
                    var publishConfigurations = this.publishConfigurationAccessor.GetPublishConfigurations().ToList();
                    if (publishConfigurations.Count == 0)
                    {
                        this.userInterface.WriteLine(Resources.PublishingTargetConfigurationCommand.ListPublishingConfigurationsNoConfigsAvailableMessage);
                        return true;
                    }

                    var dataToDisplay = new Dictionary<string, string>
                    {
                        { Resources.PublishingTargetConfigurationCommand.PublishingConfigurationTableHeadlineColumn1, Resources.PublishingTargetConfigurationCommand.PublishingConfigurationTableHeadlineColumn2 },
                        { new string('-', Resources.PublishingTargetConfigurationCommand.PublishingConfigurationTableHeadlineColumn1.Length + 3), new string('-', Resources.PublishingTargetConfigurationCommand.PublishingConfigurationTableHeadlineColumn2.Length + 3) },
                        { string.Empty, string.Empty },
                    };

                    foreach (var publishConfiguration in publishConfigurations)
                    {
                        dataToDisplay.Add(publishConfiguration.Name, publishConfiguration.PublishLocation);
                    }

                    this.userInterface.ShowKeyValueStore(dataToDisplay, 4);
                    return true;
                }

                case PublishingTargetConfigurationCommandAction.Reset:
                {
                    IServiceResult resetResult = this.publishConfigurationAccessor.ResetPublishConfiguration();
                    if (resetResult.Status == ServiceResultType.Failure)
                    {
                        // failure
                        this.userInterface.Display(resetResult);
                        this.userInterface.WriteLine(Resources.PublishingTargetConfigurationCommand.ResetPublishingConfigurationFailedMessage);
                        return false;
                    }

                    this.userInterface.WriteLine(Resources.PublishingTargetConfigurationCommand.ResetPublishingConfigurationSuccessMessage);
                    return true;
                }
            }

            this.userInterface.WriteLine(
                string.Format(Resources.PublishingTargetConfigurationCommand.InvalidActionNameMessageTemplate, string.Join(", ", this.AllowedActions)));

            return false;
        }
    }
}