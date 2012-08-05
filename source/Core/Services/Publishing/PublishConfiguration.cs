using System;

using Newtonsoft.Json;

namespace NuDeploy.Core.Services.Publishing
{
    public class PublishConfiguration
    {
        public PublishConfiguration()
        {
            this.Name = string.Empty;
            this.PublishLocation = string.Empty;
            this.ApiKey = string.Empty;
        }

        public string Name { get; set; }

        public string PublishLocation { get; set; }

        public string ApiKey { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Name) && !string.IsNullOrWhiteSpace(this.PublishLocation);
            }
        }

        [JsonIgnore]
        public bool IsLocal
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.PublishLocation))
                {
                    return true;
                }

                Uri locationUri;
                if (Uri.TryCreate(this.PublishLocation, UriKind.Absolute, out locationUri))
                {
                    return locationUri.IsUnc || locationUri.IsFile;
                }

                return true;
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                return string.Format(
                    "{0} (Location: {1}, Api Key: {2})",
                    this.Name,
                    string.IsNullOrWhiteSpace(this.PublishLocation) ? "not-set" : this.PublishLocation,
                    string.IsNullOrWhiteSpace(this.ApiKey) ? "not-set" : this.ApiKey);
            }

            return typeof(PublishConfiguration).Name;
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

            var otherObj = obj as PublishConfiguration;
            if (otherObj != null)
            {
                if (this.Name.Equals(otherObj.Name, StringComparison.OrdinalIgnoreCase)
                    && this.PublishLocation.Equals(otherObj.PublishLocation) && this.ApiKey.Equals(otherObj.ApiKey))
                {
                    return true;
                }

                return this.Name == otherObj.Name && this.PublishLocation == otherObj.PublishLocation && this.ApiKey == otherObj.ApiKey;
            }

            return false;
        }
    }
}