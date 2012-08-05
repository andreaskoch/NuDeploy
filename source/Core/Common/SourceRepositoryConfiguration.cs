using System;

using Newtonsoft.Json;

namespace NuDeploy.Core.Common
{
    public class SourceRepositoryConfiguration
    {
        public string Name { get; set; }

        public Uri Url { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Name) && this.Url != null;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                return string.Format("{0} (Url: {1})", this.Name, this.Url != null ? this.Url.ToString() : "not-set");
            }

            return typeof(SourceRepositoryConfiguration).Name;
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

            var otherObj = obj as SourceRepositoryConfiguration;
            if (otherObj != null)
            {
                if (this.Name.Equals(otherObj.Name, StringComparison.OrdinalIgnoreCase)
                       && this.Url.Equals(otherObj.Url))
                {
                    return true;
                }

                return this.Name == otherObj.Name && this.Url == otherObj.Url;
            }

            return false;
        }
    }
}