using System;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class AssemblyFileResourceInfo
    {
        public AssemblyFileResourceInfo(string resourceName, string resourcePath)
        {
            this.ResourceName = resourceName;
            this.ResourcePath = resourcePath;
        }

        public string ResourceName { get; set; }

        public string ResourcePath { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.ResourceName))
            {
                return string.Format("{0} ({1})", this.ResourceName, string.IsNullOrWhiteSpace(this.ResourcePath) ? "path-not-set" : this.ResourcePath);
            }

            return typeof(AssemblyFileResourceInfo).Name;
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

            var otherObj = obj as AssemblyFileResourceInfo;
            if (otherObj != null)
            {
                if (this.ResourceName != null && otherObj.ResourceName != null && this.ResourceName.Equals(otherObj.ResourceName, StringComparison.OrdinalIgnoreCase)
                       && this.ResourcePath != null && otherObj.ResourcePath != null && this.ResourcePath.Equals(otherObj.ResourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return this.ResourceName == otherObj.ResourceName && this.ResourcePath == otherObj.ResourcePath;
            }

            return false;
        }
    }
}