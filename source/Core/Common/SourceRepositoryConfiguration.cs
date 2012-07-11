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
                return string.Format("{0} ({1})", this.Name, this.Url != null ? this.Url.ToString() : "url-not-set");
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

            var otherRepositoryConfiguration = obj as SourceRepositoryConfiguration;
            if (otherRepositoryConfiguration != null)
            {
                return this.Name.Equals(otherRepositoryConfiguration.Name, StringComparison.OrdinalIgnoreCase)
                       && this.Url.Equals(otherRepositoryConfiguration.Url);
            }

            return false;
        }
    }
}