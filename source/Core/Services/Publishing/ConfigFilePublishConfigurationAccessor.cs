using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using NuDeploy.Core.Common.FilesystemAccess;
using NuDeploy.Core.Common.Infrastructure;

namespace NuDeploy.Core.Services.Publishing
{
    public class ConfigFilePublishConfigurationAccessor : IPublishConfigurationAccessor
    {
        public const string ConfigurationFileName = "NuDeploy.Publish.config";

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IPublishConfigurationFactory publishConfigurationFactory;

        private readonly string configurationFilePath;

        public ConfigFilePublishConfigurationAccessor(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IPublishConfigurationFactory publishConfigurationFactory)
        {
            if (applicationInformation == null)
            {
                throw new ArgumentNullException("applicationInformation");
            }

            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.publishConfigurationFactory = publishConfigurationFactory;
            this.configurationFilePath = this.GetConfigurationFilePath();
        }

        public PublishConfiguration GetPublishConfiguration(string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException("configurationName");
            }

            return this.Load().FirstOrDefault(c => c.Name.Equals(configurationName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<PublishConfiguration> GetPublishConfigurations()
        {
            return this.Load();
        }

        public bool SaveRepositoryConfiguration(string configurationName, string publishLocation, string apiKey)
        {
            PublishConfiguration publishConfiguration = this.publishConfigurationFactory.GetPublishConfiguration(configurationName, publishLocation, apiKey);
            if (publishConfiguration == null)
            {
                return false;
            }

            // get existing repositoriesConfiguration
            var configurations = this.Load().ToDictionary(configuration => configuration.Name, configuration => configuration);

            // add or update
            string existingKey = configurations.Keys.FirstOrDefault(k => k.Equals(publishConfiguration.Name, StringComparison.OrdinalIgnoreCase));
            if (existingKey == null)
            {
                configurations.Add(publishConfiguration.Name, publishConfiguration);
            }
            else
            {
                configurations[existingKey] = publishConfiguration;
            }

            return this.Save(configurations.Values.ToArray());
        }

        public bool DeletePublishConfiguration(string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException("configurationName");
            }

            var configurations = this.Load().ToList();
            if (!configurations.Any(r => r.Name.Equals(configurationName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var newConfigurationList = configurations.Where(r => r.Name.Equals(configurationName, StringComparison.OrdinalIgnoreCase) == false).ToArray();
            return this.Save(newConfigurationList);
        }

        private IEnumerable<PublishConfiguration> Load()
        {
            string json = this.filesystemAccessor.GetFileContent(this.configurationFilePath);
            return JsonConvert.DeserializeObject<PublishConfiguration[]>(json);
        }

        private bool Save(PublishConfiguration[] configurations)
        {
            string json = JsonConvert.SerializeObject(configurations);
            this.filesystemAccessor.WriteContentToFile(json, this.configurationFilePath);
            return true;
        }

        private string GetConfigurationFilePath()
        {
            return Path.Combine(this.applicationInformation.ConfigurationFileFolder, ConfigurationFileName);
        }
    }
}