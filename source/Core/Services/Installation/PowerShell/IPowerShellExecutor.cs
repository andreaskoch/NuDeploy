namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public interface IPowerShellExecutor
    {
        IServiceResult ExecuteScript(string scriptPath, params string[] parameters);
    }
}