using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace NuDeploy.Core.PowerShell
{
    public class PipelineExecutor
    {
        #region Constants and Fields

        /// <summary>
        ///   The powershell script pipeline that will be executed asynchronously.
        /// </summary>
        private readonly Pipeline pipeline;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineExecutor"/> class. 
        /// Constructor, creates a new PipelineExecutor for the given powershell script.
        /// </summary>
        /// <param name="runSpace">
        /// Powershell runspace to use for creating and executing the script. 
        /// </param>
        /// <param name="command">
        /// The script to run 
        /// </param>
        public PipelineExecutor(Runspace runSpace, string command)
        {
            // create a pipeline and feed it the script text
            this.pipeline = runSpace.CreatePipeline(command);
            this.pipeline.Commands.Add("out-default");
            this.pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

            // we'll listen for script output data by way of the DataReady event
            this.pipeline.Output.DataReady += this.OutputDataReady;
            this.pipeline.Error.DataReady += this.ErrorDataReady;
        }

        #endregion

        #region Delegates

        public delegate void DataEndDelegate(PipelineExecutor sender);

        public delegate void DataReadyDelegate(PipelineExecutor sender, ICollection<PSObject> data);

        public delegate void ErrorReadyDelegate(PipelineExecutor sender, ICollection<object> data);

        #endregion

        #region Public Events

        /// <summary>
        ///   Occurs when there is new data available from the powershell script.
        /// </summary>
        public event DataReadyDelegate OnDataReady;

        /// <summary>
        ///   Occurs when there is new errordata available from the powershell script.
        /// </summary>
        public event ErrorReadyDelegate OnErrorReady;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the powershell Pipeline associated with this PipelineExecutor
        /// </summary>
        public Pipeline Pipeline
        {
            get
            {
                return this.pipeline;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///   Start executing the script in the background.
        /// </summary>
        public void Start()
        {
            if (this.pipeline.PipelineStateInfo.State == PipelineState.NotStarted)
            {
                // close the pipeline input. If you forget to do 
                // this it won't start processing the script.
                this.pipeline.Input.Close();

                // invoke the pipeline. This will cause it to process the script in the background.
                this.pipeline.InvokeAsync();
            }
        }

        /// <summary>
        ///   Stop executing the script.
        /// </summary>
        public void Stop()
        {
            // then tell the pipeline to stop the script
            this.pipeline.Stop();
        }

        #endregion

        #region Methods

        private void ErrorDataReady(object sender, EventArgs e)
        {
            // fetch all available objects
            Collection<object> data = this.pipeline.Error.NonBlockingRead();
            if (this.OnErrorReady != null)
            {
                this.OnErrorReady(this, data);
            }
        }

        /// <summary>
        /// Event handler for the DataReady event of the powershell script pipeline.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void OutputDataReady(object sender, EventArgs e)
        {
            // fetch all available objects
            Collection<PSObject> data = this.pipeline.Output.NonBlockingRead();
            if (this.OnDataReady != null)
            {
                this.OnDataReady(this, data);
            }
        }

        #endregion
    }
}