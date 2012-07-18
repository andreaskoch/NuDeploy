using System;

namespace NuDeploy.Core.Services.Installation.PowerShell
{
    public class PowerShellExecutor : IPowerShellExecutor
    {
        private readonly IPowerShellSessionFactory powerShellSessionFactory;

        public PowerShellExecutor(IPowerShellSessionFactory powerShellSessionFactory)
        {
            if (powerShellSessionFactory == null)
            {
                throw new ArgumentNullException("powerShellSessionFactory");
            }

            this.powerShellSessionFactory = powerShellSessionFactory;
        }

        public bool ExecuteScript(string scriptPath, params string[] parameters)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                return false;
            }

            using (var powerShellSession = this.powerShellSessionFactory.GetSession())
            {
                if (powerShellSession == null)
                {
                    return false;
                }

                try
                {
                    powerShellSession.ExecuteScript(scriptPath, parameters);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}