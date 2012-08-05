namespace NuDeploy.Core.Services
{
    public class NoResult : ServiceResult
    {
        public NoResult(string message, params object[] messageArguments)
            : base(ServiceResultType.NoResult, message, messageArguments)
        {
        }
    }
}