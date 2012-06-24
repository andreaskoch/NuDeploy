using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace NuDeploy.Core.PowerShell
{
    public class PipelineExecutor
    {
        #region Constants and Fields

        /// <summary>
        ///   The object that is used to invoke the events on.
        /// </summary>
        private readonly ISynchronizeInvoke invoker;

        /// <summary>
        ///   The powershell script pipeline that will be executed asynchronously.
        /// </summary>
        private readonly Pipeline pipeline;

        /// <summary>
        ///   Event set when the user wants to stop script execution.
        /// </summary>
        private readonly ManualResetEvent stopEvent;

        /// <summary>
        ///   Local delegate to a private method
        /// </summary>
        private readonly DataEndDelegate synchDataEnd;

        /// <summary>
        ///   Local delegate to a private method
        /// </summary>
        private readonly DataReadyDelegate synchDataReady;

        /// <summary>
        ///   Local delegate to a private method
        /// </summary>
        private readonly ErrorReadyDelegate synchErrorReady;

        /// <summary>
        ///   Array of waithandles, used in the StoppableInvoke() method.
        /// </summary>
        private readonly WaitHandle[] waitHandles;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineExecutor"/> class. 
        /// Constructor, creates a new PipelineExecutor for the given powershell script.
        /// </summary>
        /// <param name="runSpace">
        /// Powershell runspace to use for creating and executing the script. 
        /// </param>
        /// <param name="invoker">
        /// The object to synchronize the DataReady and DataEnd events with. Normally you'd pass the form or component here. 
        /// </param>
        /// <param name="command">
        /// The script to run 
        /// </param>
        public PipelineExecutor(Runspace runSpace, ISynchronizeInvoke invoker, string command)
        {
            this.invoker = invoker;

            // initialize delegates
            this.synchDataReady = this.SynchDataReady;
            this.synchDataEnd = this.SynchDataEnd;
            this.synchErrorReady = this.SynchErrorReady;

            // initialize event members
            this.stopEvent = new ManualResetEvent(false);
            this.waitHandles = new WaitHandle[] { null, this.stopEvent };

            // create a pipeline and feed it the script text
            this.pipeline = runSpace.CreatePipeline(command);

            // we'll listen for script output data by way of the DataReady event
            this.pipeline.Output.DataReady += this.Output_DataReady;
            this.pipeline.Error.DataReady += new EventHandler(this.Error_DataReady);
        }

        #endregion

        #region Delegates

        public delegate void DataEndDelegate(PipelineExecutor sender);

        public delegate void DataReadyDelegate(PipelineExecutor sender, ICollection<PSObject> data);

        public delegate void ErrorReadyDelegate(PipelineExecutor sender, ICollection<object> data);

        #endregion

        #region Public Events

        /// <summary>
        ///   Occurs when powershell script completed its execution.
        /// </summary>
        public event DataEndDelegate OnDataEnd;

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
            // first make sure StoppableInvoke() stops blocking
            this.stopEvent.Set();

            // then tell the pipeline to stop the script
            this.pipeline.Stop();
        }

        #endregion

        #region Methods

        private void Error_DataReady(object sender, EventArgs e)
        {
            // fetch all available objects
            Collection<object> data = this.pipeline.Error.NonBlockingRead();

            // if there were any, invoke the ErrorReady event
            if (data.Count > 0)
            {
                this.StoppableInvoke(this.synchErrorReady, new object[] { this, data });
            }
        }

        /// <summary>
        /// Event handler for the DataReady event of the powershell script pipeline.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Output_DataReady(object sender, EventArgs e)
        {
            // fetch all available objects
            Collection<PSObject> data = this.pipeline.Output.NonBlockingRead();

            // if there were any, invoke the DataReady event
            if (data.Count > 0)
            {
                this.StoppableInvoke(this.synchDataReady, new object[] { this, data });
            }

            if (this.pipeline.Output.EndOfPipeline)
            {
                // we're done! invoke the DataEnd event
                this.StoppableInvoke(this.synchDataEnd, new object[] { this });
            }
        }

        /// <summary>
        /// Special Invoke method that operates similarly to <see cref="ISynchronizeInvoke.Invoke"/> but in addition to that, it can be aborted by setting the stopEvent. This avoids potential deadlocks that are possible when using the regular <see cref="ISynchronizeInvoke.Invoke"/> method.
        /// </summary>
        /// <param name="method">
        /// A <see cref="System.Delegate"/> to a method that takes parameters of the same number and type that are contained in <paramref name="args"/> 
        /// </param>
        /// <param name="args">
        /// An array of type <see cref="System.Object"/> to pass as arguments to the given method. This can be null if no arguments are needed 
        /// </param>
        /// <returns>
        /// The <see cref="Object"/> returned by the asynchronous operation 
        /// </returns>
        private object StoppableInvoke(Delegate method, object[] args)
        {
            IAsyncResult asyncResult = this.invoker.BeginInvoke(method, args);
            this.waitHandles[0] = asyncResult.AsyncWaitHandle;
            return (WaitHandle.WaitAny(this.waitHandles) == 0) ? this.invoker.EndInvoke(asyncResult) : null;
        }

        /// <summary>
        /// private DataEnd handling method that will pass the call on to any handlers that are attached to the OnDataEnd event of this <see cref="PipelineExecutor"/> instance.
        /// </summary>
        /// <param name="sender">
        /// </param>
        private void SynchDataEnd(PipelineExecutor sender)
        {
            DataEndDelegate delegateDataEndCopy = this.OnDataEnd;
            if (delegateDataEndCopy != null)
            {
                delegateDataEndCopy(sender);
            }
        }

        /// <summary>
        /// private DataReady handling method that will pass the call on to any event handlers that are attached to the OnDataReady event of this <see cref="PipelineExecutor"/> instance.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="data">
        /// </param>
        private void SynchDataReady(PipelineExecutor sender, ICollection<PSObject> data)
        {
            DataReadyDelegate delegateDataReadyCopy = this.OnDataReady;
            if (delegateDataReadyCopy != null)
            {
                delegateDataReadyCopy(sender, data);
            }
        }

        /// <summary>
        /// private ErrorReady handling method that will pass the call on to any event handlers that are attached to the OnErrorReady event of this <see cref="PipelineExecutor"/> instance.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="data">
        /// </param>
        private void SynchErrorReady(PipelineExecutor sender, ICollection<object> data)
        {
            ErrorReadyDelegate delegateErrorReadyCopy = this.OnErrorReady;
            if (delegateErrorReadyCopy != null)
            {
                delegateErrorReadyCopy(sender, data);
            }
        }

        #endregion
    }
}