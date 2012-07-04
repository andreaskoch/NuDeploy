using System;
using System.IO;
using System.Reflection;

using NuDeploy.Core.DependencyResolution;

namespace NuDeploy.Core.Common
{
    public static class ApplicationInformationProvider
    {
        public static ApplicationInformation GetApplicationInformation()
        {
            return new ApplicationInformation
                {
                    ApplicationName = NuDeployConstants.ApplicationName,
                    NameOfExecutable = NuDeployConstants.ExecutableName,
                    ApplicationVersion = Assembly.GetAssembly(typeof(StructureMapSetup)).GetName().Version,
                    StartupFolder = Environment.CurrentDirectory,
                    ConfigurationFileFolder = Environment.CurrentDirectory,
                    LogFolder = Path.Combine(Environment.CurrentDirectory, "NuDeployLogs"),
                    ExecutingUser =
                        new UserProperties
                            {
                                Username = Environment.UserName, 
                                Domain = Environment.UserDomainName, 
                                IsInteractiveUser = Environment.UserInteractive
                            },
                    MachineName = Environment.MachineName
                };
        }
    }
}