using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;

namespace NuDeploy.Core.Services.Publishing
{
    public class ConfigFilePublishConfigurationAccessor : IPublishConfigurationAccessor
    {
        public const string ConfigurationFileName = "NuDeploy.Publish.config";

        private readonly ApplicationInformation applicationInformation;

        private readonly IPublishConfigurationFactory publishConfigurationFactory;

        private readonly IFilesystemPersistence<PublishConfiguration[]> publishConfigurationPersistence;

        private readonly string configurationFilePath;

        public ConfigFilePublishConfigurationAccessor(ApplicationInformation applicationInformation, IPublishConfigurationFactory publishConfigurationFactory, IFilesystemPersistence<PublishConfiguration[]> publishConfigurationPersistence)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (publishConfigurationPersistence == null)
            {
                throw new ArgumentNullException("publishConfigurationPersistence");
            }

            this.applicationInformation = applicationInformation;
            this.publishConfigurationFactory = publishConfigurationFactory;
            this.publishConfigurationPersistence = publishConfigurationPersistence;

            this.configurationFilePath = this.GetConfigurationFilePath();
        }

        public PublishConfiguration GetPublishConfiguration(string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException("configurationName");
            }

            return this.GetExistingPublishConfigurationList().FirstOrDefault(c => c.Name.Equals(configurationName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<PublishConfiguration> GetPublishConfigurations()
        {
            return this.GetExistingPublishConfigurationList();
        }

        public bool AddOrUpdatePublishConfiguration(string configurationName, string publishLocation, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException("configurationName");
            }

            if (string.IsNullOrWhiteSpace(publishLocation))
            {
                throw new ArgumentException("publishLocation");
            }

            PublishConfiguration publishConfiguration = this.publishConfigurationFactory.GetPublishConfiguration(configurationName, publishLocation, apiKey);
            if (publishConfiguration == null || !publishConfiguration.IsValid)
            {
                return false;
            }

            // add or update
            var preExistingConfigurations = this.GetExistingPublishConfigurationList().ToDictionary(configuration => configuration.Name, configuration => configuration);
            string existingKey = preExistingConfigurations.Keys.FirstOrDefault(k => k.Equals(publishConfiguration.Name, StringComparison.OrdinalIgnoreCase));

            if (existingKey == null)
            {
                preExistingConfigurations.Add(publishConfiguration.Name, publishConfiguration);
            }
            else
            {
                preExistingConfigurations[existingKey] = publishConfiguration;
            }

            return this.SaveNewPublishConfigurationList(preExistingConfigurations.Values.ToArray());
        }

        public bool DeletePublishConfiguration(string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException("configurationName");
            }

            var existingConfigurations = this.GetExistingPublishConfigurationList().ToList();
            if (!existingConfigurations.Any(r => r.Name.Equals(configurationName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var newConfigurationList = existingConfigurations.Where(r => r.Name.Equals(configurationName, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            return this.SaveNewPublishConfigurationList(newConfigurationList);
        }

        private IEnumerable<PublishConfiguration> GetExistingPublishConfigurationList()
        {
            return this.publishConfigurationPersistence.Load(this.configurationFilePath) ?? new PublishConfiguration[] { };
        }

        private bool SaveNewPublishConfigurationList(PublishConfiguration[] configurations)
        {
            return this.publishConfigurationPersistence.Save(configurations, this.configurationFilePath);
        }

        private string GetConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, ConfigurationFileName);
        }
    }
}