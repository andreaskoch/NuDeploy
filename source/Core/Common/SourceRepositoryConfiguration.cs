using Newtonsoft.Json;

namespace NuDeploy.Core.Common
{
    public class SourceRepositoryConfiguration
    {
        public string Name { get; set; }

        public string Url { get; set; }

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Name) && !string.IsNullOrWhiteSpace(this.Url);
            }
        }
    }
}