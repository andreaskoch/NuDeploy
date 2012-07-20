using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class ConfigFileSourceRepositoryProvider : ISourceRepositoryProvider
    {
        public const string SourceRepositoryConfigurationFileName = "NuDeploy.Sources.config";

        public const string DefaultRepositoryName = "Default Repository";

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly string sourceConfigurationFilePath;

        public ConfigFileSourceRepositoryProvider(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor)
        {
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
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
            var defaultSources = new[] { new SourceRepositoryConfiguration { Name = DefaultRepositoryName, Url = NuDeployConstants.DefaultFeedUrl } };
            this.Save(defaultSources);
        }

        private IEnumerable<SourceRepositoryConfiguration> Load()
        {
            if (!this.filesystemAccessor.FileExists(this.sourceConfigurationFilePath))
            {
                this.CreateDefaultConfiguration();
            }

            string json = this.filesystemAccessor.GetFileContent(this.sourceConfigurationFilePath);
            return JsonConvert.DeserializeObject<SourceRepositoryConfiguration[]>(json);
        }

        private void Save(SourceRepositoryConfiguration[] repositoriesConfiguration)
        {
            string json = JsonConvert.SerializeObject(repositoriesConfiguration);
            this.filesystemAccessor.WriteContentToFile(json, this.sourceConfigurationFilePath);
        }

        private string GetSourceConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, SourceRepositoryConfigurationFileName);
        }
    }
}