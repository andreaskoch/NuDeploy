using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace NuDeploy.Core.PowerShell
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor, IDisposable
    {
        private readonly PSHost powerShellHost;

        private readonly Action ready;

        private readonly Runspace runspace;

        private readonly System.Management.Automation.PowerShell powerShell;

        private PipelineExecutor pipelineExecutor;

        public PowerShellScriptExecutor(PSHost powerShellHost, Action ready)
        {
            this.powerShellHost = powerShellHost;
            this.ready = ready;

            Environment.SetEnvironmentVariable("PSExecutionPolicyPreference", "RemoteSigned", EnvironmentVariableTarget.Process);
            this.runspace = RunspaceFactory.CreateRunspace(this.powerShellHost);
            this.runspace.Open();

            this.powerShell = System.Management.Automation.PowerShell.Create();
            this.powerShell.Runspace = this.runspace;
        }

        public void ExecuteCommand(string scriptText)
        {
            if (scriptText == null)
            {
                throw new ArgumentNullException("scriptText");
            }

            this.pipelineExecutor = new PipelineExecutor(this.powerShell.Runspace, scriptText);
            this.pipelineExecutor.OnDataReady += this.pipelineExecutor_OnDataReady;
            this.pipelineExecutor.OnErrorReady += this.pipelineExecutor_OnErrorReady;
            this.pipelineExecutor.Start();
        }

        public void ExecuteScript(string scriptPath, params string[] parameters)
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

            this.ExecuteCommand(commandText);
        }

        private void pipelineExecutor_OnDataReady(PipelineExecutor sender, ICollection<PSObject> data)
        {
            foreach (PSObject obj in data)
            {
                this.powerShellHost.UI.WriteLine(obj.ToString());
            }
        }

        void pipelineExecutor_OnErrorReady(PipelineExecutor sender, ICollection<object> data)
        {
            foreach (object e in data)
            {
                this.powerShellHost.UI.WriteLine("Error : " + e.ToString());
            }
        }

        public void Dispose()
        {
            this.powerShell.Stop();
            this.runspace.Close();
        }
    }
}
