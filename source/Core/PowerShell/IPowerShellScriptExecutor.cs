namespace NuDeploy.Core.PowerShell
{
    public interface IPowerShellScriptExecutor
    {
        void ExecuteCommand(string scriptText);

        void ExecuteScript(string scriptPath, params string[] parameters);
    }
}