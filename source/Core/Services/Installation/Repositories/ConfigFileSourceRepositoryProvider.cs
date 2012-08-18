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

        public IServiceResult SaveRepositoryConfiguration(string repositoryName, string repositoryUrl)
        {
            var repositoryConfiguration = this.sourceRepositoryConfigurationFactory.GetSourceRepositoryConfiguration(repositoryName, repositoryUrl);
            if (repositoryConfiguration == null)
            {
                return new FailureResult(
                    Resources.ConfigFileSourceRepositoryProvider.SaveFailedBecauseRepositoryCouldNotBeCreatedMessageTemplate, repositoryName, repositoryUrl);
            }

            // get existing repositoriesConfiguration
            var repositories = this.GetExistingSourceRepsitoryConfigurationList().ToDictionary(r => r.Name, r => r);

            // add or update
            string existingKey = repositories.Keys.FirstOrDefault(k => k.Equals(repositoryConfiguration.Name, StringComparison.OrdinalIgnoreCase));
            if (existingKey == null)
            {
                repositories.Add(repositoryConfiguration.Name, repositoryConfiguration);
            }
            else
            {
                repositories[existingKey] = repositoryConfiguration;
            }

            if (!this.SaveNewSourceRepositoryConfigurationList(repositories.Values.ToArray()))
            {
                return new FailureResult(Resources.ConfigFileSourceRepositoryProvider.SaveFailedMessageTemplate, repositoryConfiguration);
            }

            return new SuccessResult(Resources.ConfigFileSourceRepositoryProvider.SaveSucceededMessageTemplate, repositoryConfiguration);
        }

        public IServiceResult DeleteRepositoryConfiguration(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                throw new ArgumentException("repositoryName");
            }

            var repositories = this.GetExistingSourceRepsitoryConfigurationList().ToList();
            if (!repositories.Any(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase)))
            {
                return new FailureResult(Resources.ConfigFileSourceRepositoryProvider.DeleteFailedRepositoryDoesNotExistMessageTemplate, repositoryName);
            }

            var newRepositoryList = repositories.Where(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            if (!this.SaveNewSourceRepositoryConfigurationList(newRepositoryList))
            {
                return new FailureResult(Resources.ConfigFileSourceRepositoryProvider.DeleteFailedMessageTemplate, repositoryName);
            }

            return new SuccessResult(Resources.ConfigFileSourceRepositoryProvider.DeleteSucceededMessageTemplate, repositoryName);
        }

        public IServiceResult ResetRepositoryConfiguration()
        {
            var defaultSources = new[] { new SourceRepositoryConfiguration { Name = DefaultRepositoryName, Url = NuDeployConstants.DefaultFeedUrl } };
            if (!this.SaveNewSourceRepositoryConfigurationList(defaultSources))
            {
                return new FailureResult(Resources.ConfigFileSourceRepositoryProvider.ResetFailed);
            }

            return new SuccessResult(Resources.ConfigFileSourceRepositoryProvider.ResetSucceeded);
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