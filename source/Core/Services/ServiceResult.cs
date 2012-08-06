namespace NuDeploy.Core.Services
{
    public class ServiceResult : IServiceResult
    {
        private readonly ServiceResultType status;

        private readonly string message;

        public ServiceResult(ServiceResultType status, string message, params object[] messageArguments)
        {
            this.status = status;
            this.message = message != null ? string.Format(message, messageArguments) : null;
        }

        public ServiceResultType Status
        {
            get
            {
                return this.status;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }

        public IServiceResult InnerResult { get; set; }

        public string ResultArtefact { get; set; }
    }
}