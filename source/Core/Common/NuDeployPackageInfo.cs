using NuGet;

namespace NuDeploy.Core.Common
{
    public class NuDeployPackageInfo
    {
        public SemanticVersion Version { get; set; }

        public object Id { get; set; }

        public object Folder { get; set; }
    }
}