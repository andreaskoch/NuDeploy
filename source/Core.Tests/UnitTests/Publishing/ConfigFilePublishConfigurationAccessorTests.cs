using System;
using System.Linq;

using Moq;

using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Persistence;
using NuDeploy.Core.Services.Publishing;

using NUnit.Framework;

namespace NuDeploy.Core.Tests.UnitTests.Publishing
{
    [TestFixture]
    public class ConfigFilePublishConfigurationAccessorTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            // Act
            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Assert
            Assert.IsNotNull(configFilePublishConfigurationAccessor);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            // Act
            new ConfigFilePublishConfigurationAccessor(null, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishConfigurationFactoryParametersIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            // Act
            new ConfigFilePublishConfigurationAccessor(applicationInformation, null, publishConfigurationPersistence.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_PublishConfigurationPersistenceParametersIsNull_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();

            // Act
            new ConfigFilePublishConfigurationAccessor(applicationInformation, publishConfigurationFactory.Object, null);
        }

        #endregion

        #region GetPublishConfiguration (string configurationName)

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPublishConfiguration_ConfigurationNameParameterIsInvalid_ArgumentExceptionIsThrown(string configurationName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.GetPublishConfiguration(configurationName);
        }

        [Test]
        public void GetPublishConfiguration_ConfigurationNameParameterIsValid_PublishConfigurationPersistenceReturnsNull_ResultIsNull()
        {
            // Arrange
            string configurationName = "Some non existing Configuration";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            PublishConfiguration[] persitedConfigurations = null;
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfiguration(configurationName);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetPublishConfiguration_ConfigurationNameParameterIsValid_PublishConfigurationPersistenceReturnsNoEntries_ResultIsNull()
        {
            // Arrange
            string configurationName = "Some non existing Configuration";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new PublishConfiguration[] { };
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfiguration(configurationName);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetPublishConfiguration_ConfigurationNameParameterIsValid_PublishConfigurationPersistenceReturnsNoMatchingEntries_ResultIsNull()
        {
            // Arrange
            string configurationName = "Some Other Name";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var publishConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(publishConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfiguration(configurationName);

            // Assert
            Assert.IsNull(result);
        }

        [TestCase("some name 1")]
        [TestCase("some NAME 1")]
        [TestCase("SOME NAME 1")]
        public void GetPublishConfiguration_ConfigurationNameParameterIsValid_PublishConfigurationPersistenceContainsMatchingEntry_ResultIsMatchingEntry(string configurationName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var publishConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(publishConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfiguration(configurationName);

            // Assert
            Assert.AreEqual(configurationName.ToLower(), result.Name.ToLower());
        }

        #endregion

        #region GetPublishConfigurations

        [Test]
        public void GetPublishConfigurations_PublishConfigurationPersistenceReturnsNull_ResultIsEmptyList()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            PublishConfiguration[] persitedConfigurations = null;
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfigurations().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetPublishConfigurations_PublishConfigurationPersistenceContainsNoEntries_ResultIsEmptyList()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new PublishConfiguration[] { };
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfigurations().ToList();

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetPublishConfigurations_PublishConfigurationPersistenceContainsEntries_AllEntriesAreReturned()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            var result = configFilePublishConfigurationAccessor.GetPublishConfigurations().ToList();

            // Assert
            Assert.AreEqual(persitedConfigurations.Count(), result.Count);
        }

        #endregion

        #region AddOrUpdatePublishConfiguration

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddOrUpdatePublishConfiguration_ConfigurationNameParameterIsInvalid_ArgumentExceptionIsThrown(string configurationName)
        {
            // Arrange
            string publishLocation = "http://nuget.org/api/v2";
            string apiKey = null;

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.AddOrUpdatePublishConfiguration(configurationName, publishLocation, apiKey);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void AddOrUpdatePublishConfiguration_PublishLocationParameterIsInvalid_ArgumentExceptionIsThrown(string publishLocation)
        {
            // Arrange
            string configurationName = "Some Config Name";
            string apiKey = null;

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.AddOrUpdatePublishConfiguration(configurationName, publishLocation, apiKey);
        }

        [Test]
        public void AddOrUpdatePublishConfiguration_PublishConfigurationFactoryReturnsNull_ResultIsFalse()
        {
            // Arrange
            string configurationName = "Some Config Name";
            string publishLocation = "http://nuget.org/api/v2";
            string apiKey = Guid.NewGuid().ToString();

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            PublishConfiguration createdPublishConfiguration = null;
            publishConfigurationFactory.Setup(p => p.GetPublishConfiguration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createdPublishConfiguration);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.AddOrUpdatePublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AddOrUpdatePublishConfiguration_PublishConfigurationFactoryReturnsInvalidConfiguration_ResultIsFalse()
        {
            // Arrange
            string configurationName = "Some Config Name";
            string publishLocation = "http://nuget.org/api/v2";
            string apiKey = Guid.NewGuid().ToString();

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var createdPublishConfiguration = new PublishConfiguration();
            publishConfigurationFactory.Setup(p => p.GetPublishConfiguration(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(createdPublishConfiguration);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.AddOrUpdatePublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AddOrUpdatePublishConfiguration_ConfigDoesNotYetExist_NewEntryIsPersisted()
        {
            // Arrange
            string configurationName = "Some Config Name";
            string publishLocation = "http://nuget.org/api/v2";
            string apiKey = Guid.NewGuid().ToString();

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var createdPublishConfiguration = new PublishConfiguration { Name = configurationName, PublishLocation = publishLocation, ApiKey = apiKey };
            publishConfigurationFactory.Setup(p => p.GetPublishConfiguration(configurationName, publishLocation, apiKey)).Returns(createdPublishConfiguration);

            var existingConfigurations = new PublishConfiguration[] { };
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(existingConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.AddOrUpdatePublishConfiguration(configurationName, publishLocation, apiKey);

            // Assert
            publishConfigurationPersistence.Verify(
                p => p.Save(It.Is<PublishConfiguration[]>(configs => configs.First().Equals(createdPublishConfiguration)), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void AddOrUpdatePublishConfiguration_ConfigDoesNotYetExist_UpdatedEntryIsPersisted()
        {
            // Arrange
            string configurationName = "Some Config Name";
            string newPublishLocation = "http://new.nuget.org/api/v2";
            string newApiKey = "New-Api-Key";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var createdPublishConfiguration = new PublishConfiguration { Name = configurationName, PublishLocation = newPublishLocation, ApiKey = newApiKey };
            publishConfigurationFactory.Setup(p => p.GetPublishConfiguration(configurationName, newPublishLocation, newApiKey)).Returns(createdPublishConfiguration);

            var existingConfigurations = new[]
                {
                    new PublishConfiguration { Name = configurationName, PublishLocation = "http://old.nuget.org/api/v2", ApiKey = "Old-Api-Key" }
                };
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(existingConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.AddOrUpdatePublishConfiguration(configurationName, newPublishLocation, newApiKey);

            // Assert
            publishConfigurationPersistence.Verify(
                p =>
                p.Save(
                    It.Is<PublishConfiguration[]>(
                        configs =>
                        configs.First().Name.Equals(configurationName) && configs.First().PublishLocation.Equals(newPublishLocation)
                        && configs.First().ApiKey.Equals(newApiKey)),
                    It.IsAny<string>()),
                Times.Once());
        }

        #endregion

        #region DeletePublishConfiguration

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void DeletePublishConfiguration_ConfigurationNameIsInvalid_ArgumentExceptionIsThrown(string configurationName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);            
        }

        [Test]
        public void DeletePublishConfiguration_PublishConfigurationPersistenceReturnsNull_ResultIsFalse()
        {
            // Arrange
            string configurationName = "Some Configuration Name";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            PublishConfiguration[] persitedConfigurations = null;
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeletePublishConfiguration_PublishConfigurationPersistenceReturnsEmptyList_ResultIsFalse()
        {
            // Arrange
            string configurationName = "Some Configuration Name";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new PublishConfiguration[] { };
            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeletePublishConfiguration_PublishConfigurationPersistenceReturnsNoMatchingEntries_ResultIsFalse()
        {
            // Arrange
            string configurationName = "Some Other Name";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);

            // Assert
            Assert.IsFalse(result);
        }

        [TestCase("Some Name 1")]
        [TestCase("Some name 1")]
        [TestCase("some name 1")]
        [TestCase("SOME NAME 1")]
        public void DeletePublishConfiguration_PublishConfigurationPersistenceReturnsAMatchingEntry_ReducedListIsPersisted(string configurationName)
        {
            // Arrange
            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);

            // Assert
            publishConfigurationPersistence.Verify(
                p =>
                p.Save(
                    It.Is<PublishConfiguration[]>(
                        configs => configs.Length == persitedConfigurations.Length - 1 && configs.Any(c => c.Name.Equals(configurationName)) == false),
                    It.IsAny<string>()),
                Times.Once());
        }

        [Test]
        public void DeletePublishConfiguration_PublishConfigurationPersistenceReturnsAMatchingEntry_PersistReturnsFalse_ResultIsFalse()
        {
            // Arrange
            string configurationName = "Some Name 1";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);
            publishConfigurationPersistence.Setup(p => p.Save(It.IsAny<PublishConfiguration[]>(), It.IsAny<string>())).Returns(false);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeletePublishConfiguration_PublishConfigurationPersistenceReturnsAMatchingEntry_PersistReturnsTrue_ResultIsTrue()
        {
            // Arrange
            string configurationName = "Some Name 1";

            var applicationInformation = new ApplicationInformation { ConfigurationFileFolder = Environment.CurrentDirectory };
            var publishConfigurationFactory = new Mock<IPublishConfigurationFactory>();
            var publishConfigurationPersistence = new Mock<IFilesystemPersistence<PublishConfiguration[]>>();

            var persitedConfigurations = new[]
                {
                    new PublishConfiguration { Name = "Some Name 1", PublishLocation = "http://some-nuget-server-1.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 2", PublishLocation = "http://some-nuget-server-2.com/api/v2" },
                    new PublishConfiguration { Name = "Some Name 3", PublishLocation = "http://some-nuget-server-3.com/api/v2" }
                };

            publishConfigurationPersistence.Setup(p => p.Load(It.IsAny<string>())).Returns(persitedConfigurations);
            publishConfigurationPersistence.Setup(p => p.Save(It.IsAny<PublishConfiguration[]>(), It.IsAny<string>())).Returns(true);

            var configFilePublishConfigurationAccessor = new ConfigFilePublishConfigurationAccessor(
                applicationInformation, publishConfigurationFactory.Object, publishConfigurationPersistence.Object);

            // Act
            bool result = configFilePublishConfigurationAccessor.DeletePublishConfiguration(configurationName);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}