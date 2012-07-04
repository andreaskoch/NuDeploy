using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public class ConfigFileSourceRepositoryProvider : ISourceRepositoryProvider
    {
        public const string SourceRepositoryConfigurationFileName = "sources.config";

        private static readonly Encoding ConfigurationFileEncoding = Encoding.UTF8;

        private readonly ApplicationInformation applicationInformation;

        private readonly string sourceConfigurationFilePath;

        public ConfigFileSourceRepositoryProvider(ApplicationInformation applicationInformation)
        {
            this.applicationInformation = applicationInformation;
            this.sourceConfigurationFilePath = this.GetSourceConfigurationFilePath();
        }

        public IEnumerable<SourceRepositoryConfiguration> GetRepositoryConfigurations()
        {
            return this.Load();
        }

        public void SaveRepositoryConfiguration(SourceRepositoryConfiguration sourceRepositoryConfiguration)
        {
            if (sourceRepositoryConfiguration == null)
            {
                throw new ArgumentNullException("sourceRepositoryConfiguration");
            }

            if (!sourceRepositoryConfiguration.IsValid)
            {
                throw new ArgumentException(Resources.Exceptions.SourceRepositoryConfigurationInvalid, "sourceRepositoryConfiguration");
            }

            // get existing repositoriesConfiguration
            var repositories = this.Load().ToDictionary(r => r.Name, r => r);

            // add or update
            string existingKey = repositories.Keys.FirstOrDefault(k => k.Equals(sourceRepositoryConfiguration.Name, StringComparison.OrdinalIgnoreCase));
            if (existingKey == null)
            {
                repositories.Add(sourceRepositoryConfiguration.Name, sourceRepositoryConfiguration);
            }
            else
            {
                repositories[existingKey] = sourceRepositoryConfiguration;
            }

            // save
            this.Save(repositories.Values.ToArray());
        }

        public void DeleteRepositoryConfiguration(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                throw new ArgumentException("repositoryName");
            }

            var repositories = this.Load().Where(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase) == false);

            // save
            this.Save(repositories.ToArray());
        }

        public void ResetRepositoryConfiguration()
        {
            this.CreateDefaultConfiguration();
        }

        private void CreateDefaultConfiguration()
        {
            var defaultSources = new[] { new SourceRepositoryConfiguration { Name = "Default Repository", Url = NuDeployConstants.DefaultFeedUrl } };
            this.Save(defaultSources);
        }

        private IEnumerable<SourceRepositoryConfiguration> Load()
        {
            if (!File.Exists(this.sourceConfigurationFilePath))
            {
                this.CreateDefaultConfiguration();
            }

            string json = File.ReadAllText(this.sourceConfigurationFilePath, ConfigurationFileEncoding);
            return JsonConvert.DeserializeObject<SourceRepositoryConfiguration[]>(json);
        }

        private void Save(SourceRepositoryConfiguration[] repositoriesConfiguration)
        {
            string json = JsonConvert.SerializeObject(repositoriesConfiguration);
            File.WriteAllText(this.sourceConfigurationFilePath, json, ConfigurationFileEncoding);
        }

        private string GetSourceConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, SourceRepositoryConfigurationFileName);
        }
    }
}