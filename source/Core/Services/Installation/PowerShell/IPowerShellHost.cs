using System;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public interface IPowerShellHost
    {
        CultureInfo CurrentCulture { get; }

        CultureInfo CurrentUICulture { get; }

        Guid InstanceId { get; }

        string Name { get; }

        PSHostUserInterface UI { get; }

        Version Version { get; }

        PSObject PrivateData { get; }

        void EnterNestedPrompt();

        void ExitNestedPrompt();

        void NotifyBeginApplication();

        void NotifyEndApplication();

        void SetShouldExit(int exitCode);
    }
}