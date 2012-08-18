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

        public IServiceResult ExecuteScript(string scriptPath, params string[] parameters)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
            {
                return new FailureResult(Resources.PowerShellExecutor.ScriptPathCannotBeNullOrEmpty);
            }

            using (var powerShellSession = this.powerShellSessionFactory.GetSession())
            {
                if (powerShellSession == null)
                {
                    return new FailureResult(
                        Resources.PowerShellExecutor.CannotCreatePowerShellSessionMessageTemplate, scriptPath, string.Join(", ", parameters));
                }

                try
                {
                    string scriptOutput = powerShellSession.ExecuteScript(scriptPath, parameters);
                    return new SuccessResult(
                        Resources.PowerShellExecutor.ScriptHasBeenSuccessfullyExecutedMessageTemplate, scriptPath, string.Join(", ", parameters), scriptOutput);
                }
                catch (Exception powerShellException)
                {
                    return new FailureResult(
                        Resources.PowerShellExecutor.ScriptExecutionFailedWithExceptionMessageTemplate,
                        scriptPath,
                        string.Join(", ", parameters),
                        powerShellException.Message);
                }
            }
        }
    }
}