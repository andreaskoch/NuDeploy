namespace NuDeploy.Core.Common.Logging
{
    public interface IActionLogger
    {
        void Log(string message, params object[] args);
    }
}