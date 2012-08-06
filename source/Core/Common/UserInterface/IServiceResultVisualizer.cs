using NuDeploy.Core.Services;

namespace NuDeploy.Core.Common.UserInterface
{
    public interface IServiceResultVisualizer
    {
        void Display(IUserInterface userInterface, IServiceResult serviceResult);
    }
}