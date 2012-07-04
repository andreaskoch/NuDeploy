namespace NuDeploy.Core.Common
{
    public interface IActionLogger
    {
        void Log(string message, params object[] args);
    }
}