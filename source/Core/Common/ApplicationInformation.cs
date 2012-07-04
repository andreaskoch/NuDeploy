using System;

namespace NuDeploy.Core.Common
{
    public class ApplicationInformation
    {
        public string NameOfExecutable { get; set; }

        public Version ApplicationVersion { get; set; }

        public string ApplicationName { get; set; }

        public string StartupFolder { get; set; }

        public string ConfigurationFileFolder { get; set; }

        public string LogFolder { get; set; }

        public UserProperties ExecutingUser { get; set; }

        public string MachineName { get; set; }
    }
}