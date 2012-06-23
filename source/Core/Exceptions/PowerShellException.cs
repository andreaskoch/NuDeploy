using System;

namespace NuDeploy.Core.Exceptions
{
    public class PowerShellException : Exception
    {
        public PowerShellException()
        {
        }

        public PowerShellException(string message) : base(message)
        {
        }

        public PowerShellException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
