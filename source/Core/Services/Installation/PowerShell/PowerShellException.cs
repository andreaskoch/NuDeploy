using System;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public class PowerShellException : Exception
    {
        public PowerShellException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}