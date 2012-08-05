namespace NuDeploy.Core.Services
{
    public class SuccessResult : ServiceResult
    {
        public SuccessResult() : base(ServiceResultType.Failure, string.Empty)
        {
        }

        public SuccessResult(string message, params object[] messageArguments)
            : base(ServiceResultType.Success, message, messageArguments)
        {
        }
    }
}