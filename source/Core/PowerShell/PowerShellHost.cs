using System;
using System.Globalization;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.PowerShell
{
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

        public override CultureInfo CurrentCulture
        {
            get { return this.originalCultureInfo; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return this.originalUICultureInfo; }
        }

        public override Guid InstanceId
        {
            get { return this.instanceId; }
        }

        public override string Name
        {
            get
            {
                return string.Format("{0} {1}", this.applicationInformation.ApplicationName, "PowerShell Host");
            }
        }

        public override PSHostUserInterface UI
        {
            get
            {
                return this.userInterface;
            }
        }

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

        public override void NotifyBeginApplication()
        {
        }

        public override void NotifyEndApplication()
        {
        }

        public override void SetShouldExit(int exitCode)
        {
        }
    }
}