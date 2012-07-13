using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;

using NuDeploy.Core.Common.UserInterface;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public class NuDeployPowerShellUserInterface : PSHostUserInterface
    {
        private readonly IUserInterface userInterface;

        private NuDeployRawPowerShellUserInterface rawUi;

        public NuDeployPowerShellUserInterface(IUserInterface userInterface)
        {
            this.userInterface = userInterface;
        }

        public override PSHostRawUserInterface RawUI
        {
            get
            {
                return this.rawUi ?? (this.rawUi = new NuDeployRawPowerShellUserInterface());
            }
        }

        public override Dictionary<string, PSObject> Prompt(
                                                            string caption,
                                                            string message,
                                                            System.Collections.ObjectModel.Collection<FieldDescription> descriptions)
        {
            var results = new Dictionary<string, PSObject>();

            this.userInterface.WriteLine(message);
            foreach (var fieldDescription in descriptions)
            {
                string key = fieldDescription.Name;

                this.userInterface.Write(key + ": ");
                var inputString = this.userInterface.GetInput();
                if (string.IsNullOrWhiteSpace(inputString) == false)
                {
                    results.Add(key, new PSObject(inputString));
                }
            }

            return results;
        }

        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName,
                                                         PSCredentialTypes allowedCredentialTypes,
                                                         PSCredentialUIOptions options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override string ReadLine()
        {
            return this.userInterface.GetInput();
        }

        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public override void Write(string value)
        {
            this.userInterface.Write(value);
        }

        public override void Write(
                                   ConsoleColor foregroundColor,
                                   ConsoleColor backgroundColor,
                                   string value)
        {
            this.userInterface.Write(value);
        }

        public override void WriteDebugLine(string message)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
        }

        public override void WriteErrorLine(string value)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "ERROR: {0}", value));
        }

        public override void WriteLine()
        {
            this.userInterface.WriteLine(string.Empty);
        }

        public override void WriteLine(string value)
        {
            this.userInterface.WriteLine(value);
        }

        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            this.userInterface.WriteLine(value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        public override void WriteVerboseLine(string message)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        public override void WriteWarningLine(string message)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }
    }
}
