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
            return string.Format("{0} ({1})", this.Name, this.Url);
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