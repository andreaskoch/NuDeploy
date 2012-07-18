namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public interface IPowerShellSessionFactory
    {
        IPowerShellSession GetSession();
    }
}