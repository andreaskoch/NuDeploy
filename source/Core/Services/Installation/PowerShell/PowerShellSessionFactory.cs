using System;

using NuDeploy.Core.Common.FilesystemAccess;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public class PowerShellSessionFactory : IPowerShellSessionFactory
    {
        private readonly IPowerShellHost powerShellHost;

        private readonly IFilesystemAccessor filesystemAccessor;

        public PowerShellSessionFactory(IPowerShellHost powerShellHost, IFilesystemAccessor filesystemAccessor)
        {
            if (powerShellHost == null)
            {
                throw new ArgumentNullException("powerShellHost");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.powerShellHost = powerShellHost;
            this.filesystemAccessor = filesystemAccessor;            
        }

        public IPowerShellSession GetSession()
        {
            return new PowerShellSession(this.powerShellHost, this.filesystemAccessor);
        }
    }
}