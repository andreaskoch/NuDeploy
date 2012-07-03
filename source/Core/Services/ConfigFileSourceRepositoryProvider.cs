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

        public IEnumerable<SourceRepository> GetRepositories()
        {
            return this.GetRepositoriesFromConfigFile();
        }

        public void SaveRepository(SourceRepository sourceRepository)
        {
            if (sourceRepository == null)
            {
                throw new ArgumentNullException("sourceRepository");
            }

            if (!sourceRepository.IsValid)
            {
                throw new ArgumentException("The supplied SourceRepository is not valid.", "sourceRepository");
            }

            // get existing repositories
            var repositories = this.GetRepositoriesFromConfigFile().ToDictionary(r => r.Name, r => r);

            // add or update
            string existingKey = repositories.Keys.FirstOrDefault(k => k.Equals(sourceRepository.Name, StringComparison.OrdinalIgnoreCase));
            if (existingKey == null)
            {
                repositories.Add(sourceRepository.Name, sourceRepository);
            }
            else
            {
                repositories[existingKey] = sourceRepository;
            }

            // save
            this.Save(repositories.Values.ToArray());
        }

        public void DeleteRepository(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                throw new ArgumentException("repositoryName");
            }

            var repositories = this.GetRepositoriesFromConfigFile().Where(r => r.Name.Equals(repositoryName, StringComparison.OrdinalIgnoreCase) == false);

            // save
            this.Save(repositories.ToArray());
        }

        private IEnumerable<SourceRepository> GetRepositoriesFromConfigFile()
        {
            if (!File.Exists(SourceRepositoryConfigurationFileName))
            {
                this.CreateDefaultConfiguration();
            }

            string json = File.ReadAllText(SourceRepositoryConfigurationFileName, ConfigurationFileEncoding);
            return JsonConvert.DeserializeObject<SourceRepository[]>(json);
        }

        private void CreateDefaultConfiguration()
        {
            var defaultSources = new[] { new SourceRepository { Name = "Default Repository", Url = NuDeployConstants.DefaultFeedUrl } };
            this.Save(defaultSources);
        }

        private void Save(SourceRepository[] repositories)
        {
            string json = JsonConvert.SerializeObject(repositories);
            File.WriteAllText(SourceRepositoryConfigurationFileName, json, ConfigurationFileEncoding);            
        }
    }
}