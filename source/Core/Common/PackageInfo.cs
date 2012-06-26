using NuGet;

namespace NuDeploy.Core.Common
{
    public class PackageInfo
    {
        public string Id { get; set; }

        public SemanticVersion Version { get; set; }
    }
}