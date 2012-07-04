using System;

using NuDeploy.Core.DependencyResolution;

namespace NuDeploy.Core.Common
{
    public class ApplicationInformation
    {
        public string NameOfExecutable { get; set; }

        public Version ApplicationVersion { get; set; }

        public string ApplicationName { get; set; }

        public string StartupFolder { get; set; }

        public string ConfigurationFileFolder { get; set; }

        public UserProperties ExecutingUser { get; set; }
    }
}