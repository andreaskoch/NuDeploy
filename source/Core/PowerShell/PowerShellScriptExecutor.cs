using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace NuDeploy.Core.PowerShell
{
    public class PowerShellScriptExecutor : IPowerShellScriptExecutor, ISynchronizeInvoke, IDisposable
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

            this.pipelineExecutor = new PipelineExecutor(this.powerShell.Runspace, this, scriptText);
            this.pipelineExecutor.OnDataReady += this.pipelineExecutor_OnDataReady;
            this.pipelineExecutor.OnDataEnd += this.pipelineExecutor_OnDataEnd;
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

        private void StopScript()
        {
            if (this.pipelineExecutor != null)
            {
                this.pipelineExecutor.OnDataReady -= this.pipelineExecutor_OnDataReady;
                this.pipelineExecutor.OnDataEnd -= this.pipelineExecutor_OnDataEnd;
                this.pipelineExecutor.Stop();
                this.pipelineExecutor = null;
            }
        }

        private void pipelineExecutor_OnDataEnd(PipelineExecutor sender)
        {
            if (sender.Pipeline.PipelineStateInfo.State == PipelineState.Failed)
            {
                this.powerShellHost.UI.WriteLine(string.Format("Error in script: {0}", sender.Pipeline.PipelineStateInfo.Reason));
            }
            else
            {
                this.powerShellHost.UI.WriteLine("Ready.");
            }

            this.ready();
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

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public bool InvokeRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            this.powerShell.Stop();
            this.runspace.Close();
        }
    }
}
