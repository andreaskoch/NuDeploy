using System;
using System.IO;
using System.Reflection;

namespace NuDeploy.Core.Common.Infrastructure
{
    public static class ApplicationInformationProvider
    {
        public static ApplicationInformation GetApplicationInformation()
        {
            return new ApplicationInformation
                {
                    ApplicationName = NuDeployConstants.ApplicationName,
                    NameOfExecutable = NuDeployConstants.ExecutableName,
                    ApplicationVersion = Assembly.GetAssembly(typeof(ApplicationInformationProvider)).GetName().Version,
                    StartupFolder = Environment.CurrentDirectory,
                    ConfigurationFileFolder = Environment.CurrentDirectory,
                    LogFolder = Path.Combine(Environment.CurrentDirectory, "NuDeployLogs"),
                    BuildFolder = Path.Combine(Environment.CurrentDirectory, "NuDeployBuilds"),
                    PrePackagingFolder = Path.Combine(Environment.CurrentDirectory, "NuDeployPrepackaging"),
                    PackagingFolder = Path.Combine(Environment.CurrentDirectory, "NuDeployPackages"),
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