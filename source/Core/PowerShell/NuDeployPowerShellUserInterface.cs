﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.PowerShell
{
    public class NuDeployPowerShellUserInterface : PSHostUserInterface
    {
        private readonly IUserInterface userInterface;

        /// <summary>
        /// An instance of the PSRawUserInterface class.
        /// </summary>
        private NuDeployRawPowerShellUserInterface rawUi;

        public NuDeployPowerShellUserInterface(IUserInterface userInterface)
        {
            this.userInterface = userInterface;
        }

        /// <summary>
        /// Gets an instance of the PSRawUserInterface class for this host
        /// application.
        /// </summary>
        public override PSHostRawUserInterface RawUI
        {
            get
            {
                return this.rawUi ?? (this.rawUi = new NuDeployRawPowerShellUserInterface());
            }
        }

        /// <summary>
        /// Prompts the user for input. In this example this functionality is not 
        /// needed so the method throws a NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption or title of the prompt.</param>
        /// <param name="message">The text of the prompt.</param>
        /// <param name="descriptions">A collection of FieldDescription objects that 
        /// describe each field of the prompt.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
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

        /// <summary>
        /// Provides a set of choices that enable the user to choose a 
        /// single option from a set of options. In this example this 
        /// functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <param name="caption">Text that proceeds (a title) the choices.</param>
        /// <param name="message">A message that describes the choice.</param>
        /// <param name="choices">A collection of ChoiceDescription objects that describes 
        /// each choice.</param>
        /// <param name="defaultChoice">The index of the label in the Choices parameter 
        /// collection. To indicate no default choice, set to -1.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override int PromptForChoice(string caption, string message, System.Collections.ObjectModel.Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Prompts the user for credentials with a specified prompt window caption, 
        /// prompt message, user name, and target name. In this example this 
        /// functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption for the message window.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="userName">The user name whose credential is to be prompted for.</param>
        /// <param name="targetName">The name of the target for which the credential is collected.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Prompts the user for credentials by using a specified prompt window caption, 
        /// prompt message, user name and target name, credential types allowed to be 
        /// returned, and UI behavior options. In this example this functionality 
        /// is not needed so the method throws a NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption for the message window.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="userName">The user name whose credential is to be prompted for.</param>
        /// <param name="targetName">The name of the target for which the credential is collected.</param>
        /// <param name="allowedCredentialTypes">A PSCredentialTypes constant that 
        /// identifies the type of credentials that can be returned.</param>
        /// <param name="options">A PSCredentialUIOptions constant that identifies the UI 
        /// behavior when it gathers the credentials.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
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

        /// <summary>
        /// Reads characters that are entered by the user until a newline 
        /// (carriage return) is encountered.
        /// </summary>
        /// <returns>The characters that are entered by the user.</returns>
        public override string ReadLine()
        {
            return this.userInterface.GetInput();
        }

        /// <summary>
        /// Reads characters entered by the user until a newline (carriage return) 
        /// is encountered and returns the characters as a secure string. In this 
        /// example this functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <returns>Throws a NotImplemented exception.</returns>
        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Writes characters to the output display of the host.
        /// </summary>
        /// <param name="value">The characters to be written.</param>
        public override void Write(string value)
        {
            this.userInterface.Write(value);
        }

        /// <summary>
        /// Writes characters to the output display of the host and specifies the 
        /// foreground and background colors of the characters. This implementation 
        /// ignores the colors.
        /// </summary>
        /// <param name="foregroundColor">The color of the characters.</param>
        /// <param name="backgroundColor">The backgound color to use.</param>
        /// <param name="value">The characters to be written.</param>
        public override void Write(
                                   ConsoleColor foregroundColor,
                                   ConsoleColor backgroundColor,
                                   string value)
        {
            this.userInterface.Write(value);
        }

        /// <summary>
        /// Writes a debug message to the output display of the host.
        /// </summary>
        /// <param name="message">The debug message that is displayed.</param>
        public override void WriteDebugLine(string message)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
        }

        /// <summary>
        /// Writes an error message to the output display of the host.
        /// </summary>
        /// <param name="value">The error message that is displayed.</param>
        public override void WriteErrorLine(string value)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "ERROR: {0}", value));
        }

        /// <summary>
        /// Writes a newline character (carriage return) 
        /// to the output display of the host. 
        /// </summary>
        public override void WriteLine()
        {
            this.userInterface.WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a line of characters to the output display of the host 
        /// and appends a newline character(carriage return). 
        /// </summary>
        /// <param name="value">The line to be written.</param>
        public override void WriteLine(string value)
        {
            this.userInterface.WriteLine(value);
        }

        /// <summary>
        /// Writes a line of characters to the output display of the host 
        /// with foreground and background colors and appends a newline (carriage return). 
        /// </summary>
        /// <param name="foregroundColor">The forground color of the display. </param>
        /// <param name="backgroundColor">The background color of the display. </param>
        /// <param name="value">The line to be written.</param>
        public override void WriteLine(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            // Write to the output stream, ignore the colors
            this.userInterface.WriteLine(value);
        }

        /// <summary>
        /// Writes a progress report to the output display of the host.
        /// </summary>
        /// <param name="sourceId">Unique identifier of the source of the record. </param>
        /// <param name="record">A ProgressReport object.</param>
        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        /// <summary>
        /// Writes a verbose message to the output display of the host.
        /// </summary>
        /// <param name="message">The verbose message that is displayed.</param>
        public override void WriteVerboseLine(string message)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        /// <summary>
        /// Writes a warning message to the output display of the host.
        /// </summary>
        /// <param name="message">The warning message that is displayed.</param>
        public override void WriteWarningLine(string message)
        {
            this.userInterface.WriteLine(string.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }
    }
}