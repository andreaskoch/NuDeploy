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

        private readonly ISourceRepositoryConfigurationFactory sourceRepositoryConfigurationFactory;

        private readonly string sourceConfigurationFilePath;

        public ConfigFileSourceRepositoryProvider(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, ISourceRepositoryConfigurationFactory sourceRepositoryConfigurationFactory)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (sourceRepositoryConfigurationFactory == null)
            {
                throw new ArgumentNullException("sourceRepositoryConfigurationFactory");
            }

            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.sourceRepositoryConfigurationFactory = sourceRepositoryConfigurationFactory;
            this.sourceConfigurationFilePath = this.GetSourceConfigurationFilePath();
        }

        public IEnumerable<SourceRepositoryConfiguration> GetRepositoryConfigurations()
        {
            return this.Load();
        }

        public bool SaveRepositoryConfiguration(string repositoryName, string repositoryUrl)
        {
            var sourceRepositoryConfiguration = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);
            if (sourceRepositoryConfiguration == null)
            {
                return false;
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

            return this.Save(repositories.Values.ToArray());
        }

        public bool DeleteRepositoryConfiguration(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                throw new ArgumentException("repositoryName");
            }

            var repositories = this.Load().Where(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase) == false);
            return this.Save(repositories.ToArray());
        }

        public bool ResetRepositoryConfiguration()
        {
            return this.CreateDefaultConfiguration();
        }

        private bool CreateDefaultConfiguration()
        {
            var defaultSources = new[] { new SourceRepositoryConfiguration { Name = DefaultRepositoryName, Url = NuDeployConstants.DefaultFeedUrl } };
            return this.Save(defaultSources);
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

        private bool Save(SourceRepositoryConfiguration[] repositoriesConfiguration)
        {
            string json = JsonConvert.SerializeObject(repositoriesConfiguration);
            this.filesystemAccessor.WriteContentToFile(json, this.sourceConfigurationFilePath);
            return true;
        }

        private string GetSourceConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, SourceRepositoryConfigurationFileName);
        }
    }
}