using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using NuDeploy.CommandLine.Commands;
using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.CommandLine.DependencyResolution;
using NuDeploy.CommandLine.Infrastructure.Console;
using NuDeploy.Core.Common.FileEncoding;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services.Installation.Repositories;

using NUnit.Framework;

using StructureMap;

namespace NuDeploy.CommandLine.Tests.IntegrationTests.CommandLine
{
    [TestFixture]
    public class ProgramRunTests
    {
        #region Test Setup
        
        private readonly Mutex sequentialTestExecutionMonitor;

        private Program program;

        private ApplicationInformation applicationInformation;

        private IUserInterface userInterface;

        private IActionLogger logger;

        private ICommandLineArgumentInterpreter commandLineArgumentInterpreter;

        private IHelpCommand helpCommand;

        private ICommandProvider commandProvider;

        private IEncodingProvider encodingProvider;

        public ProgramRunTests()
        {
            this.sequentialTestExecutionMonitor = new Mutex(false);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            this.sequentialTestExecutionMonitor.WaitOne();

            CommandLineIntegrationTestUtilities.RemoveAllFilesAndFoldersWhichAreCreatedOnStartup();

            StructureMapSetup.Setup();
            this.encodingProvider = ObjectFactory.GetInstance<IEncodingProvider>();
            this.applicationInformation = ObjectFactory.GetInstance<ApplicationInformation>();
            this.commandProvider = ObjectFactory.GetInstance<ICommandProvider>();
            this.userInterface = ObjectFactory.GetInstance<IUserInterface>();
            this.logger = ObjectFactory.GetInstance<IActionLogger>();
            this.commandLineArgumentInterpreter = ObjectFactory.GetInstance<ICommandLineArgumentInterpreter>();
            this.helpCommand = ObjectFactory.GetInstance<IHelpCommand>();

            this.program = new Program(this.applicationInformation, this.userInterface, this.logger, this.commandLineArgumentInterpreter, this.helpCommand);
        }

        [TearDown]
        public void TearDown()
        {
            this.sequentialTestExecutionMonitor.ReleaseMutex();
        }

        #endregion

        #region General

        [Test]
        public void NoCommandLineArgumentsSupplied_ResultIsZero()
        {
            // Arrange
            var commandlineArguments = new string[] { };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestCase("UnknownCommand")]
        [TestCase("asjdjsakdjsakldj")]
        [TestCase("help install")]
        [TestCase("help package")]
        [TestCase("install")]
        public void RandomCommandsArePassedToTheApplication_CommandIsLoggedToDisc(string commandLine)
        {
            // Arrange
            var commandlineArguments = commandLine.Split(' ');

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            var logFilePath = Directory.GetFiles(this.applicationInformation.LogFolder, "*.log").First();
            var logFileContent = File.ReadAllText(logFilePath, this.encodingProvider.GetEncoding());
            Assert.IsTrue(logFileContent.Contains(commandLine));
        }

        #endregion

        #region Help Overview

        [Test]
        public void NoCommandLineArguments_HelpOverviewIsDisplayed()
        {
            // Arrange
            var commandlineArguments = new string[] { };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            this.Verify_HelpOverviewIsDisplayed();
        }

        [TestCase("UnknownCommand")]
        [TestCase("asjdjsakdjsakldj")]
        [TestCase("")]
        [TestCase(" ")]
        public void CommandIsNotRecognized_HelpOverviewIsDisplayed(string commandName)
        {
            // Arrange
            var commandlineArguments = new[] { commandName };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            this.Verify_HelpOverviewIsDisplayed();
        }

        [Test]
        public void HelpCommandNameSupplied_HelpOverviewIsDisplayed()
        {
            // Arrange
            var commandlineArguments = new[] { HelpCommand.CommandName };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            this.Verify_HelpOverviewIsDisplayed();
        }

        [Test]
        public void HelpAlternativeCommandNameSupplied_HelpOverviewIsDisplayed()
        {
            // Arrange
            var commandlineArguments = new[] { "?" };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            this.Verify_HelpOverviewIsDisplayed();
        }

        #endregion

        #region Help (Command specific)

        [TestCase(CleanupCommand.CommandName)]
        [TestCase(HelpCommand.CommandName)]
        [TestCase(InstallationStatusCommand.CommandName)]
        [TestCase(InstallCommand.CommandName)]
        [TestCase(PackageSolutionCommand.CommandName)]
        [TestCase(RepositorySourceConfigurationCommand.CommandName)]
        [TestCase(SelfUpdateCommand.CommandName)]
        [TestCase(UninstallCommand.CommandName)]
        public void HelpCommandIsCalled_ArgumentIsKnownCommand_CommandSpecificHelpIsDisplayed(string commandName)
        {
            // Arrange
            var commandlineArguments = new[] { HelpCommand.CommandName, commandName };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            var command = this.commandProvider.GetAvailableCommands().FirstOrDefault(c => c.Attributes.CommandName.Equals(commandName));

            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.CommandName), string.Format("The command help should display the command name \"{0}\".", command.Attributes.CommandName));
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.Description), string.Format("The command help should display the command description text \"{0}\".", command.Attributes.Description));

            foreach (var argument in command.Attributes.RequiredArguments)
            {
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(argument), string.Format("The command help should display the command argument \"{0}\".", argument));
            }
        }

        #endregion

        #region Repository Configuration

        [TestCase("ad")]
        [TestCase("aaadd")]
        [TestCase("")]
        [TestCase(" ")]
        public void RepositorySourceConfiguration_InvalidActionName_ListOfAvailableActionsIsDisplayed(string actionName)
        {
            // Arrange
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, actionName, "Nuget Repository", "http://nuget.org/api/v2" };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            var repositoryConfigCommand =
                this.commandProvider.GetAvailableCommands().FirstOrDefault(c => c.Attributes.CommandName.Equals(RepositorySourceConfigurationCommand.CommandName)) as RepositorySourceConfigurationCommand;

            foreach (var allowedActionName in repositoryConfigCommand.AllowedActions)
            {
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(allowedActionName));
            }
        }

        [TestCase("Remote HTTP Repository", "http://nuget.org/api/v2")]
        [TestCase("Local Repository", "C:\\local-nuget-repository")]
        [TestCase("UNC Repository", @"\\unc-repository")]
        public void RepositorySourceConfiguration_AddRepository_PositionalArguments_MessageContainingTheNameAndTheUrlOfTheNewRepositoryIsWrittenToUserInterface(string repositoryName, string repositoryUrl)
        {
            // Arrange
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "add",  repositoryName, repositoryUrl };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(repositoryName));
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(repositoryUrl));
        }

        [TestCase("Remote HTTP Repository", "http://nuget.org/api/v2")]
        [TestCase("Local Repository", "C:\\local-nuget-repository")]
        [TestCase("UNC Repository", @"\\unc-repository")]
        public void RepositorySourceConfiguration_AddRepository_NamedArguments_MessageContainingTheNameAndTheUrlOfTheNewRepositoryIsWrittenToUserInterface(string repositoryName, string repositoryUrl)
        {
            // Arrange
            var commandlineArguments = new[]
                {
                    RepositorySourceConfigurationCommand.CommandName,
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameAction + "=" + "Add",
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryName + "=" + repositoryName,
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryUrl + "=" + repositoryUrl
                };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(repositoryName));
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(repositoryUrl));
        }

        [TestCase("Remote HTTP Repository", "http://nuget.org/api/v2")]
        [TestCase("Local Repository", "C:\\local-nuget-repository")]
        [TestCase("UNC Repository", @"\\unc-repository")]
        public void RepositorySourceConfiguration_AddRepository_PositionalArguments_EntryIsSavedToConfigFile(string repositoryName, string repositoryUrl)
        {
            // Arrange
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "add", repositoryName, repositoryUrl };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            string configFilePath = this.GetSourceRepositoryConfigurationFilePath();
            string fileContent = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());

            Assert.IsTrue(fileContent.Contains(repositoryName));
            Assert.IsTrue(fileContent.Contains(new Uri(repositoryUrl).ToString()));
        }

        [TestCase("Remote HTTP Repository", "http://nuget.org/api/v2")]
        [TestCase("Local Repository", "C:\\local-nuget-repository")]
        [TestCase("UNC Repository", @"\\unc-repository")]
        public void RepositorySourceConfiguration_AddRepository_NamedArguments_EntryIsSavedToConfigFile(string repositoryName, string repositoryUrl)
        {
            // Arrange
            var commandlineArguments = new[]
                {
                    RepositorySourceConfigurationCommand.CommandName,
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameAction + "=" + "Add",
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryName + "=" + repositoryName,
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryUrl + "=" + repositoryUrl
                };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            string configFilePath = this.GetSourceRepositoryConfigurationFilePath();
            string fileContent = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());

            Assert.IsTrue(fileContent.Contains(repositoryName));
            Assert.IsTrue(fileContent.Contains(new Uri(repositoryUrl).ToString()));
        }

        [Test]
        public void RepositorySourceConfiguration_DeleteRepository_PositionalArguments_RepositoryDoesNotExist_ResultIsZero()
        {
            // Arrange
            string repositoryName = "Non Existing Repository Name 31827321kdljsak";
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "delete", repositoryName };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void RepositorySourceConfiguration_DeleteRepository_NamedArguments_RepositoryDoesNotExist_ResultIsZero()
        {
            // Arrange
            string repositoryName = "Non Existing Repository Name 31827321kdljsak";

            var commandlineArguments = new[]
                {
                    RepositorySourceConfigurationCommand.CommandName, 
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameAction + "=" + "delete",
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryName + "=" + repositoryName
                };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void RepositorySourceConfiguration_DeleteRepository_PositionalArguments_RepositoryDoesNotExist_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string repositoryName = "Non Existing Repository Name 31827321kdljsak";
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "delete", repositoryName };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(repositoryName));
        }

        [Test]
        public void RepositorySourceConfiguration_DeleteRepository_NamedArguments_RepositoryDoesNotExist_MessageIsWrittenToUserInterface()
        {
            // Arrange
            string repositoryName = "Non Existing Repository Name 31827321kdljsak";

            var commandlineArguments = new[]
                {
                    RepositorySourceConfigurationCommand.CommandName, 
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameAction + "=" + "delete",
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryName + "=" + repositoryName
                };

            // Act
            this.program.Run(commandlineArguments);

            // Assert
            Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(repositoryName));
        }

        [Test]
        public void RepositorySourceConfiguration_DeleteRepository_PositionalArguments_RepositoryExists_RepositoryIsRemovedFromConfigFile_ResultIsZero()
        {
            // Arrange
            string repositoryName = "Test Repository";
            string repositoryUrl = "C:\\local-nuget-repository";

            // add repository and make sure it was added
            ObjectFactory.GetInstance<ISourceRepositoryProvider>().SaveRepositoryConfiguration(repositoryName, repositoryUrl);

            string configFilePath = this.GetSourceRepositoryConfigurationFilePath();
            var fileContentBeforeDelete = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());
            Assert.IsTrue(fileContentBeforeDelete.Contains(repositoryName));
            Assert.IsTrue(fileContentBeforeDelete.Contains(new Uri(repositoryUrl).ToString()));

            // prepare delete command
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "delete", repositoryName };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert that the repository is no longer available in the config file
            var fileContentAfterDelete = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());
            Assert.IsFalse(fileContentAfterDelete.Contains(repositoryName));
            Assert.IsFalse(fileContentAfterDelete.Contains(new Uri(repositoryUrl).ToString()));

            // Assert that the result is zero
            Assert.AreEqual(0, result);
        }

        [Test]
        public void RepositorySourceConfiguration_DeleteRepository_NamedArguments_RepositoryExists_RepositoryIsRemovedFromConfigFile_ResultIsZero()
        {
            // Arrange
            string repositoryName = "Test Repository";
            string repositoryUrl = "C:\\local-nuget-repository";

            // add repository and make sure it was added
            ObjectFactory.GetInstance<ISourceRepositoryProvider>().SaveRepositoryConfiguration(repositoryName, repositoryUrl);

            string configFilePath = this.GetSourceRepositoryConfigurationFilePath();
            var fileContentBeforeDelete = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());
            Assert.IsTrue(fileContentBeforeDelete.Contains(repositoryName));
            Assert.IsTrue(fileContentBeforeDelete.Contains(new Uri(repositoryUrl).ToString()));

            // prepare delete command
            var commandlineArguments = new[]
                {
                    RepositorySourceConfigurationCommand.CommandName, 
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameAction + "=" + "delete", 
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameRepositoryName + "=" + repositoryName
                };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert that the repository is no longer available in the config file
            var fileContentAfterDelete = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());
            Assert.IsFalse(fileContentAfterDelete.Contains(repositoryName));
            Assert.IsFalse(fileContentAfterDelete.Contains(new Uri(repositoryUrl).ToString()));

            // Assert that the result is zero
            Assert.AreEqual(0, result);
        }

        [Test]
        public void RepositorySourceConfiguration_ListRepositories_PositionalArguments_EachRepositoryNameAndUrlIsWrittenToUserInterface_ResultIsZero()
        {
            // Arrange
            var repositories = new Dictionary<string, string>
                {
                    { "Test Repository 1", "C:\\local-nuget-repository-1" },
                    { "Test Repository 2", "C:\\local-nuget-repository-2" },
                    { "Test Repository 3", "C:\\local-nuget-repository-3" }
                };

            foreach (var keyValuePair in repositories)
            {
                ObjectFactory.GetInstance<ISourceRepositoryProvider>().SaveRepositoryConfiguration(keyValuePair.Key, keyValuePair.Value);
            }

            // prepare list command
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "list" };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert that the repository is no longer available in the config file
            foreach (var repository in repositories)
            {
                string name = repository.Key;
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(name));
            }

            // Assert that the result is zero
            Assert.AreEqual(0, result);
        }

        [Test]
        public void RepositorySourceConfiguration_ListRepositories_NamedArguments_EachRepositoryNameAndUrlIsWrittenToUserInterface_ResultIsZero()
        {
            // Arrange
            var repositories = new Dictionary<string, string>
                {
                    { "Test Repository 1", "C:\\local-nuget-repository-1" },
                    { "Test Repository 2", "C:\\local-nuget-repository-2" },
                    { "Test Repository 3", "C:\\local-nuget-repository-3" }
                };

            foreach (var keyValuePair in repositories)
            {
                ObjectFactory.GetInstance<ISourceRepositoryProvider>().SaveRepositoryConfiguration(keyValuePair.Key, keyValuePair.Value);
            }

            // prepare list command
            var commandlineArguments = new[]
                {
                    RepositorySourceConfigurationCommand.CommandName, 
                    "-" + RepositorySourceConfigurationCommand.ArgumentNameAction + "=" + "list"
                };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert that the repository is no longer available in the config file
            foreach (var repository in repositories)
            {
                string name = repository.Key;
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(name));
            }

            // Assert that the result is zero
            Assert.AreEqual(0, result);
        }

        [Test]
        public void RepositorySourceConfiguration_ResetRepositories_PositionalArguments_RepositoriesAreNoLongerStoredInConfig_ResultIsZero()
        {
            // Arrange
            var repositories = new Dictionary<string, string>
                {
                    { "Test Repository 1", "C:\\local-nuget-repository-1" },
                    { "Test Repository 2", "C:\\local-nuget-repository-2" },
                    { "Test Repository 3", "C:\\local-nuget-repository-3" }
                };

            foreach (var keyValuePair in repositories)
            {
                ObjectFactory.GetInstance<ISourceRepositoryProvider>().SaveRepositoryConfiguration(keyValuePair.Key, keyValuePair.Value);
            }

            // Assert that the repositories are available in the config file
            string configFilePath = this.GetSourceRepositoryConfigurationFilePath();
            var fileContentBeforeReset = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());
            foreach (var keyValuePair in repositories)
            {
                Assert.IsTrue(fileContentBeforeReset.Contains(keyValuePair.Key));
                Assert.IsTrue(fileContentBeforeReset.Contains(new Uri(keyValuePair.Value).ToString()));
            }

            // prepare reset command
            var commandlineArguments = new[] { RepositorySourceConfigurationCommand.CommandName, "reset" };

            // Act
            int result = this.program.Run(commandlineArguments);

            // Assert that the repositories are no longer available listed
            var fileContentAfterReset = File.ReadAllText(configFilePath, this.encodingProvider.GetEncoding());
            foreach (var keyValuePair in repositories)
            {
                Assert.IsFalse(fileContentAfterReset.Contains(keyValuePair.Key));
                Assert.IsFalse(fileContentAfterReset.Contains(new Uri(keyValuePair.Value).ToString()));
            }

            // Assert that the result is zero
            Assert.AreEqual(0, result);
        }

        #endregion

        #region utility methods
        
        private void Verify_HelpOverviewIsDisplayed()
        {
            foreach (var command in this.commandProvider.GetAvailableCommands())
            {
                Assert.IsTrue(this.userInterface.UserInterfaceContent.Contains(command.Attributes.CommandName));
            }            
        }

        private string GetSourceRepositoryConfigurationFilePath()
        {
            return Path.Combine(
                this.applicationInformation.ConfigurationFileFolder, ConfigFileSourceRepositoryProvider.SourceRepositoryConfigurationFileName);
        }

        #endregion
    }
}