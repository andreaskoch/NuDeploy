using System;

using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;

namespace NuDeploy.CommandLine.UserInterface
{
    public class ConsoleServiceResultVisualizer : IServiceResultVisualizer
    {
        public void Display(IUserInterface userInterface, IServiceResult serviceResult)
        {
            if (userInterface == null)
            {
                throw new ArgumentNullException("userInterface");
            }

            if (serviceResult == null)
            {
                return;
            }

            if (serviceResult.Status == ServiceResultType.NoResult)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(serviceResult.Message))
            {
                return;
            }

            if (serviceResult.InnerResult != null)
            {
                this.Display(userInterface, serviceResult.InnerResult);
            }

            string resultTypeLabel = Resources.ServiceResultVisualizer.ResourceManager.GetString("ServiceResultType" + serviceResult.Status);
            userInterface.WriteLine(resultTypeLabel + ": " + serviceResult.Message);

            if (!string.IsNullOrWhiteSpace(serviceResult.ResultArtefact))
            {
                userInterface.WriteLine(Resources.ServiceResultVisualizer.ReturnValue + ": " + serviceResult.ResultArtefact);
            }
        }
    }
}