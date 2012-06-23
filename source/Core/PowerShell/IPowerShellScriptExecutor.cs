namespace NuDeploy.Core.PowerShell
{
    public interface IPowerShellScriptExecutor
    {
        string RunScript(string scriptText);
    }
}