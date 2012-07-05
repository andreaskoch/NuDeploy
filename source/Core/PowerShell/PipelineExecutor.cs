using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace NuDeploy.Core.PowerShell
{
    public class PipelineExecutor
    {
        private readonly Pipeline pipeline;

        public PipelineExecutor(Runspace runSpace, string command)
        {
            this.pipeline = runSpace.CreatePipeline(command);
            this.pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

            this.pipeline.Output.DataReady += this.OutputDataReady;
            this.pipeline.Error.DataReady += this.ErrorDataReady;
        }

        public delegate void DataEndDelegate(PipelineExecutor sender);

        public delegate void DataReadyDelegate(PipelineExecutor sender, ICollection<PSObject> data);

        public delegate void ErrorReadyDelegate(PipelineExecutor sender, ICollection<object> data);

        public event DataReadyDelegate OnDataReady;

        public event ErrorReadyDelegate OnErrorReady;

        public Pipeline Pipeline
        {
            get
            {
                return this.pipeline;
            }
        }

        public void Start()
        {
            if (this.pipeline.PipelineStateInfo.State == PipelineState.NotStarted)
            {
                this.pipeline.Input.Close();
                this.pipeline.InvokeAsync();
            }
        }

        public void Stop()
        {
            this.pipeline.Stop();
        }

        private void ErrorDataReady(object sender, EventArgs e)
        {
            Collection<object> data = this.pipeline.Error.NonBlockingRead();
            if (this.OnErrorReady != null)
            {
                this.OnErrorReady(this, data);
            }
        }

        private void OutputDataReady(object sender, EventArgs e)
        {
            Collection<PSObject> data = this.pipeline.Output.NonBlockingRead();
            if (this.OnDataReady != null)
            {
                this.OnDataReady(this, data);
            }
        }
    }
}