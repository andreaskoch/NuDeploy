using System;
using System.Globalization;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.PowerShell
{
    /// <summary>
    /// This is a sample implementation of the PSHost abstract class for 
    /// console applications. Not all members are implemented. Those that 
    /// are not implemented throw a NotImplementedException exception or 
    /// return nothing.
    /// </summary>
    public class PowerShellHost : PSHost
    {
        private readonly PSHostUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly CultureInfo originalCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

        private readonly CultureInfo originalUICultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;

        private readonly Guid instanceId = Guid.NewGuid();

        public PowerShellHost(PSHostUserInterface userInterface, ApplicationInformation applicationInformation)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
        }

        /// <summary>
        /// Return the culture information to use. This implementation 
        /// returns a snapshot of the culture information of the thread 
        /// that created this object.
        /// </summary>
        public override CultureInfo CurrentCulture
        {
            get { return this.originalCultureInfo; }
        }

        /// <summary>
        /// Return the UI culture information to use. This implementation 
        /// returns a snapshot of the UI culture information of the thread 
        /// that created this object.
        /// </summary>
        public override CultureInfo CurrentUICulture
        {
            get { return this.originalUICultureInfo; }
        }

        /// <summary>
        /// This implementation always returns the GUID allocated at 
        /// instantiation time.
        /// </summary>
        public override Guid InstanceId
        {
            get { return this.instanceId; }
        }

        /// <summary>
        /// Return a string that contains the name of the host implementation. 
        /// Keep in mind that this string may be used by script writers to
        /// identify when your host is being used.
        /// </summary>
        public override string Name
        {
            get
            {
                return string.Format("{0} {1}", this.applicationInformation.ApplicationName, "PowerShell Host");
            }
        }

        /// <summary>
        /// This sample does not implement a PSHostUserInterface component so
        /// this property simply returns null.
        /// </summary>
        public override PSHostUserInterface UI
        {
            get
            {
                return this.userInterface;
            }
        }

        /// <summary>
        /// Return the version object for this application. Typically this
        /// should match the version resource in the application.
        /// </summary>
        public override Version Version
        {
            get
            {
                return this.applicationInformation.ApplicationVersion;
            }
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException(
                "The method or operation is not implemented.");
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException(
                "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API is called before an external application process is 
        /// started. Typically it is used to save state so the parent can 
        /// restore state that has been modified by a child process (after 
        /// the child exits). In this example, this functionality is not  
        /// needed so the method returns nothing.
        /// </summary>
        public override void NotifyBeginApplication()
        {
        }

        /// <summary>
        /// This API is called after an external application process finishes.
        /// Typically it is used to restore state that a child process may
        /// have altered. In this example, this functionality is not  
        /// needed so the method returns nothing.
        /// </summary>
        public override void NotifyEndApplication()
        {
        }

        /// <summary>
        /// Indicate to the host application that exit has
        /// been requested. Pass the exit code that the host
        /// application should use when exiting the process.
        /// </summary>
        /// <param name="exitCode">The exit code to use.</param>
        public override void SetShouldExit(int exitCode)
        {
            // this.program.ShouldExit = true;
            // this.program.ExitCode = exitCode;
        }
    }
}