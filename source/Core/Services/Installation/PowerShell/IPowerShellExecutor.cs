namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public interface IPowerShellExecutor
    {
        bool ExecuteScript(string scriptPath, params string[] parameters);
    }
}