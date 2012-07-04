using System;

using Newtonsoft.Json;

namespace NuDeploy.Core.Common
{
    public class PackageInfo
    {
        public string Id { get; set; }

        public string Version { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Id) && !string.IsNullOrWhiteSpace(this.Version);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", this.Id, this.Version);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherPackage = obj as PackageInfo;
            if (otherPackage != null)
            {
                return this.Id.Equals(otherPackage.Id, StringComparison.OrdinalIgnoreCase)
                       && this.Version.Equals(otherPackage.Version, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}