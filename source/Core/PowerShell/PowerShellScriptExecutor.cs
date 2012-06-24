using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;

namespace NuDeploy.Core.PowerShell
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor, IDisposable
    {
        private readonly PSHost powerShellHost;

        private readonly Runspace runspace;

        private readonly System.Management.Automation.PowerShell powerShell;

        private readonly StringBuilder pipelineOutput;

        private PipelineExecutor pipelineExecutor;

        public PowerShellScriptExecutor(PSHost powerShellHost)
        {
            this.powerShellHost = powerShellHost;
            this.pipelineOutput = new StringBuilder();

            Environment.SetEnvironmentVariable("PSExecutionPolicyPreference", "RemoteSigned", EnvironmentVariableTarget.Process);
            this.runspace = RunspaceFactory.CreateRunspace(this.powerShellHost);
            this.runspace.Open();

            this.powerShell = System.Management.Automation.PowerShell.Create();
            this.powerShell.Runspace = this.runspace;
        }

        public string ExecuteCommand(string scriptText)
        {
            if (scriptText == null)
            {
                throw new ArgumentNullException("scriptText");
            }

            this.pipelineExecutor = new PipelineExecutor(this.powerShell.Runspace, scriptText);
            this.pipelineExecutor.OnDataReady += this.PipelineExecutorOnDataReady;
            this.pipelineExecutor.OnErrorReady += this.PipelineExecutorOnErrorReady;
            this.pipelineExecutor.Start();

            while (!this.pipelineExecutor.Pipeline.Output.EndOfPipeline)
            {
                Thread.Sleep(500);
            }

            return this.pipelineOutput.ToString();
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

        private void PipelineExecutorOnDataReady(PipelineExecutor sender, ICollection<PSObject> data)
        {
            foreach (PSObject obj in data)
            {
                string message = obj.ToString();
                this.pipelineOutput.AppendLine(message);
                this.powerShellHost.UI.WriteLine(message);
            }
        }

        private void PipelineExecutorOnErrorReady(PipelineExecutor sender, ICollection<object> data)
        {
            foreach (object e in data)
            {
                string message = "Error : " + e;
                this.pipelineOutput.AppendLine(message);
                this.powerShellHost.UI.WriteLine(message);
            }
        }

        public void Dispose()
        {
            this.powerShell.Stop();
            this.runspace.Close();
        }
    }
}
