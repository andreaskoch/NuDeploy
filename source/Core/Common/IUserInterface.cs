namespace NuDeploy.Core.Common
{
    public interface IUserInterface
    {
        void Show(string messageFormatString, params object[] args);
    }
}