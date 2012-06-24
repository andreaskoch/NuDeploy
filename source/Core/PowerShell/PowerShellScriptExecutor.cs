using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;

using NuDeploy.Core.Exceptions;

namespace NuDeploy.Core.PowerShell
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor
    {
        private readonly PSHost powerShellHost;

        public PowerShellScriptExecutor(PSHost powerShellHost)
        {
            this.powerShellHost = powerShellHost;
            Environment.SetEnvironmentVariable("PSExecutionPolicyPreference", "RemoteSigned", EnvironmentVariableTarget.Process);
        }

        public string ExecuteCommand(string scriptText)
        {
            if (scriptText == null)
            {
                throw new ArgumentNullException("scriptText");
            }

            // create Powershell runspace
            using (Runspace runspace = RunspaceFactory.CreateRunspace(this.powerShellHost))
            {
                // open it
                runspace.Open();

                var stringBuilder = new StringBuilder();
                using (System.Management.Automation.PowerShell powerShell = System.Management.Automation.PowerShell.Create())
                {
                    powerShell.Runspace = runspace;

                    // create a pipeline and feed it the script text
                    Pipeline pipeline = runspace.CreatePipeline();
                    pipeline.Commands.AddScript(scriptText);
                    pipeline.Commands.Add("Out-String");

                    // execute the script
                    Collection<PSObject> results;
                    try
                    {
                        results = pipeline.Invoke();
                    }
                    catch (RuntimeException powerShellRuntimeException)
                    {
                        string message = string.Format(
                            "PowerShell Runtime Exception: {0}: {1}\r\n{2}",
                            powerShellRuntimeException.ErrorRecord.InvocationInfo.InvocationName,
                            powerShellRuntimeException.Message,
                            powerShellRuntimeException.ErrorRecord.InvocationInfo.PositionMessage);

                            throw new PowerShellException(message, powerShellRuntimeException);
                    }
                    catch (Exception powerShellException)
                    {
                        throw new PowerShellException(powerShellException.Message, powerShellException);
                    }

                    // convert the script result into a single string                
                    foreach (PSObject obj in results)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }
                }

                // close the runspace
                runspace.Close();

                return stringBuilder.ToString();
            }
        }

        public string ExecuteScript(string scriptPath, params string[] parameters)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                throw new ArgumentException("The supplied script path cannot be null or empty.");
            }

            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("Script not found.", scriptPath);
            }

            string commandText = string.Format("& '{0}'", scriptPath);
            if (parameters != null && parameters.Length > 0)
            {
                commandText += " " + string.Join(" ", parameters);
            }

            return this.ExecuteCommand(commandText);
        }
    }
}
