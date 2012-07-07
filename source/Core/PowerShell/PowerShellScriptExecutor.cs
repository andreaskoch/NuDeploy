using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.PowerShell
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor, IDisposable
    {
        private readonly PSHost powerShellHost;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly Runspace runspace;

        private readonly System.Management.Automation.PowerShell powerShell;

        private readonly StringBuilder pipelineOutput;

        private PipelineExecutor pipelineExecutor;

        public PowerShellScriptExecutor(PSHost powerShellHost, IFilesystemAccessor filesystemAccessor)
        {
            this.powerShellHost = powerShellHost;
            this.filesystemAccessor = filesystemAccessor;
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
                throw new ArgumentException(Resources.Exceptions.InvalidPowerShellScriptPath);
            }

            if (!this.filesystemAccessor.FileExists(scriptPath))
            {
                throw new FileNotFoundException(Resources.Exceptions.PowerShellScriptNotFound, scriptPath);
            }

            string commandText = string.Format("& '{0}'", scriptPath);
            if (parameters != null && parameters.Length > 0)
            {
                commandText += " " + string.Join(" ", parameters);
            }

            return this.ExecuteCommand(commandText);
        }

        public void Dispose()
        {
            this.powerShell.Stop();
            this.runspace.Close();
        }

        private void PipelineExecutorOnDataReady(PipelineExecutor sender, ICollection<PSObject> data)
        {
            foreach (PSObject obj in data.Where(o => o != null))
            {
                string message = obj.ToString();
                this.pipelineOutput.AppendLine(message);
                this.powerShellHost.UI.WriteLine(message);
            }
        }

        private void PipelineExecutorOnErrorReady(PipelineExecutor sender, ICollection<object> data)
        {
            foreach (object obj in data.Where(o => o != null))
            {
                string message = "Error : " + obj;
                this.pipelineOutput.AppendLine(message);
                this.powerShellHost.UI.WriteLine(message);
            }
        }
    }
}
