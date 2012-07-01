using NuGet;

namespace NuDeploy.Core.Common
{
    public class NuDeployPackageInfo
    {
        public SemanticVersion Version { get; set; }

        public string Id { get; set; }

        public string Folder { get; set; }

        public bool IsInstalled { get; set; }
    }
}