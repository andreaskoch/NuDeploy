using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;

namespace NuDeploy.Core.Services.Installation.Repositories
{
    public class ConfigFileSourceRepositoryProvider : ISourceRepositoryProvider
    {
        public const string SourceRepositoryConfigurationFileName = "NuDeploy.Sources.config";

        public const string DefaultRepositoryName = "Default Repository";

        private readonly ApplicationInformation applicationInformation;

        private readonly ISourceRepositoryConfigurationFactory sourceRepositoryConfigurationFactory;

        private readonly IFilesystemPersistence<SourceRepositoryConfiguration[]> filesystemPersistence;

        private readonly string sourceConfigurationFilePath;

        public ConfigFileSourceRepositoryProvider(ApplicationInformation applicationInformation, ISourceRepositoryConfigurationFactory sourceRepositoryConfigurationFactory, IFilesystemPersistence<SourceRepositoryConfiguration[]> filesystemPersistence)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (sourceRepositoryConfigurationFactory == null)
            {
                throw new ArgumentNullException("sourceRepositoryConfigurationFactory");
            }

            if (filesystemPersistence == null)
            {
                throw new ArgumentNullException("filesystemPersistence");
            }

            this.applicationInformation = applicationInformation;
            this.sourceRepositoryConfigurationFactory = sourceRepositoryConfigurationFactory;
            this.filesystemPersistence = filesystemPersistence;
            this.sourceConfigurationFilePath = this.GetSourceConfigurationFilePath();
        }

        public IEnumerable<SourceRepositoryConfiguration> GetRepositoryConfigurations()
        {
            var configurations = this.GetExistingSourceRepsitoryConfigurationList().ToList();
            return configurations;
        }

        public bool SaveRepositoryConfiguration(string repositoryName, string repositoryUrl)
        {
            var sourceRepositoryConfiguration = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);
            if (sourceRepositoryConfiguration == null)
            {
                return false;
            }

            // get existing repositoriesConfiguration
            var repositories = this.GetExistingSourceRepsitoryConfigurationList().ToDictionary(r => r.Name, r => r);

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

            return this.SaveNewSourceRepositoryConfigurationList(repositories.Values.ToArray());
        }

        public bool DeleteRepositoryConfiguration(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                throw new ArgumentException("repositoryName");
            }

            var repositories = this.GetExistingSourceRepsitoryConfigurationList().ToList();
            if (!repositories.Any(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var newRepositoryList = repositories.Where(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            return this.SaveNewSourceRepositoryConfigurationList(newRepositoryList);
        }

        public bool ResetRepositoryConfiguration()
        {
            var defaultSources = new[] { new SourceRepositoryConfiguration { Name = DefaultRepositoryName, Url = NuDeployConstants.DefaultFeedUrl } };
            return this.SaveNewSourceRepositoryConfigurationList(defaultSources);
        }

        private IEnumerable<SourceRepositoryConfiguration> GetExistingSourceRepsitoryConfigurationList()
        {
            return this.filesystemPersistence.Load(this.sourceConfigurationFilePath) ?? new SourceRepositoryConfiguration[] { };
        }

        private bool SaveNewSourceRepositoryConfigurationList(SourceRepositoryConfiguration[] repositoriesConfiguration)
        {
            return this.filesystemPersistence.Save(repositoriesConfiguration, this.sourceConfigurationFilePath);
        }

        private string GetSourceConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, SourceRepositoryConfigurationFileName);
        }
    }
}