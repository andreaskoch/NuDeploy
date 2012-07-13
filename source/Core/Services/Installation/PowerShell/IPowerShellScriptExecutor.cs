namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public interface IPowerShellScriptExecutor
    {
        string ExecuteCommand(string scriptText);

        string ExecuteScript(string scriptPath, params string[] parameters);
    }
}