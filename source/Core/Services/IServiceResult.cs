namespace NuDeploy.Core.Services
{
    public interface IServiceResult
    {
        ServiceResultType Status { get; }

        string Message { get; }

        IServiceResult InnerResult { get; }

        string ResultArtefact { get; }
    }
}