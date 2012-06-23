using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

using NuDeploy.Core.Exceptions;

namespace NuDeploy.Core.PowerShell
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor
    {
        public PowerShellScriptExecutor()
        {
            Environment.SetEnvironmentVariable("PSExecutionPolicyPreference", "RemoteSigned", EnvironmentVariableTarget.Process);
        }

        public string ExecuteCommand(string scriptText)
        {
            if (scriptText == null)
            {
                throw new ArgumentNullException("scriptText");
            }

            // create Powershell runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it
            runspace.Open();

            // create a pipeline and feed it the script text
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            // add an extra command to transform the script
            // output objects into nicely formatted strings

            // remove this line to get the actual objects
            // that the script returns. For example, the script

            // "Get-Process" returns a collection
            // of System.Diagnostics.Process instances.
            pipeline.Commands.Add("Out-String");

            // execute the script
            Collection<PSObject> results;
            try
            {
                results = pipeline.Invoke();
            }
            catch (Exception powerShellException)
            {
                throw new PowerShellException(powerShellException.Message, powerShellException);
            }

            // close the runspace
            runspace.Close();

            // convert the script result into a single string
            var stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            return stringBuilder.ToString();
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
