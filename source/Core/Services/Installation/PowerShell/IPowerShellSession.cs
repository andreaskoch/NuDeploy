using System;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public interface IPowerShellSession : IDisposable
    {
        string ExecuteCommand(string scriptText);

        string ExecuteScript(string scriptPath, params string[] parameters);
    }
}