namespace NuDeploy.Core.Services
{
    public class FailureResult : ServiceResult
    {
        public FailureResult() : base(ServiceResultType.Failure, string.Empty)
        {
        }

        public FailureResult(string message, params object[] messageArguments) : base(ServiceResultType.Failure, message, messageArguments)
        {
        }
    }
}