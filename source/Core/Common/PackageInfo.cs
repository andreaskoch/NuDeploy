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
            if (!string.IsNullOrWhiteSpace(this.Id))
            {
                return string.Format("{0} ({1})", this.Id, this.Version ?? "version-not-set");
            }

            return typeof(PackageInfo).Name;
        }

        public override int GetHashCode()
        {
            int hash = 37;
            hash = (hash * 23) + this.ToString().GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherObj = obj as PackageInfo;
            if (otherObj != null)
            {
                if (this.Id.Equals(otherObj.Id, StringComparison.OrdinalIgnoreCase)
                       && this.Version.Equals(otherObj.Version, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return this.Id == otherObj.Id && this.Version == otherObj.Version;
            }

            return false;
        }
    }
}