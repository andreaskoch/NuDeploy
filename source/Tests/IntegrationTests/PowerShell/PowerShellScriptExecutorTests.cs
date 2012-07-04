using System;
using System.IO;
using System.Management.Automation.Host;

using NuDeploy.Core.Common;
using NuDeploy.Core.PowerShell;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.PowerShell
{
    [TestFixture]
    public class PowerShellScriptExecutorTests
    {
        private IPowerShellScriptExecutor powerShellScriptExecutor;

        private PSHostUserInterface powerShellUserInterface;

        private IUserInterface userInterface;

        [SetUp]
        public void Setup()
        {
            var applicationInformation = new ApplicationInformation
                {
                    ApplicationName = "NuDeploy.Tests",
                    ApplicationVersion = new Version(1, 0),
                    NameOfExecutable = "NuDeploy.Tests.exe",
                    StartupFolder = Environment.CurrentDirectory,
                    ConfigurationFileFolder = Environment.CurrentDirectory
                };

            IConsoleTextManipulation consoleTextManipulation = new ConsoleTextManipulation();

            this.userInterface = new ConsoleUserInterface(consoleTextManipulation);
            this.powerShellUserInterface = new NuDeployPowerShellUserInterface(this.userInterface);
            PSHost powerShellHost = new PowerShellHost(this.powerShellUserInterface, applicationInformation);

            this.powerShellScriptExecutor = new PowerShellScriptExecutor(powerShellHost);
        }

        #region ExecuteCommand

        [Test]
        public void ExecuteCommand_ScriptIsEmpty_ResultIsEmpty()
        {
            // Arrange
            string script = string.Empty;

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExecuteCommand_ScriptIsNull_ArgumentNullExceptionIsThrown()
        {
            // Act
            this.powerShellScriptExecutor.ExecuteCommand(null);
        }

        [Test]
        public void ExecuteCommand_GetExecutionPolicy_ExecutionPolicyIsSetToRemoteSigned()
        {
            // Arrange
            string script = "get-executionpolicy";

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual("RemoteSigned", result.Trim());
        }

        [Test]
        public void ExecuteCommand_ErrorActionPreference_ResultIsContinue()
        {
            // Arrange
            string script = "$ErrorActionPreference";

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual("Continue", result.Trim());
        }

        [Test]
        public void ExecuteCommand_VerbosePreference_ResultIsSilentlyContinue()
        {
            // Arrange
            string script = "$VerbosePreference";

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual("SilentlyContinue", result.Trim());
        }

        [Test]
        public void ExecuteCommand_GetLocation_PowerShellLocationIsCurrentDirectory()
        {
            // Arrange
            string script = "(get-location).Path";

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual(Environment.CurrentDirectory, result.Trim());
        }

        [Test]
        public void ExecuteCommand_WriteHost_ResultEqualsMessage()
        {
            // Arrange
            string message = "test";
            string script = string.Format("Write-Host \"{0}\"", message);

            // Act
            var result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual(string.Empty, result.Trim());
        }

        [Test]
        public void ExecuteCommand_ExecuteScript_RelativePath_ScriptResultsAreReturned()
        {
            // Arrange
            string script = string.Format("& '{0}'", GetRelativeScriptPath("TestScript-01-ReturnResults.ps1"));

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual("TestScript-01-ReturnResults.ps1", result.Trim());
        }

        [Test]
        public void ExecuteCommand_ExecuteScript_AbsolutePath_ScriptResultsAreReturned()
        {
            // Arrange
            string script = string.Format("& '{0}'", GetAbsoluteScriptPath("TestScript-01-ReturnResults.ps1"));

            // Act
            string result = this.powerShellScriptExecutor.ExecuteCommand(script);

            // Assert
            Assert.AreEqual("TestScript-01-ReturnResults.ps1", result.Trim());
        }

        #endregion

        #region ExecuteScript

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteScript_ScriptPathIsNull_ArgumentExceptionIsThrown()
        {
            // Arrange
            string scriptPath = null;

            // Act
            this.powerShellScriptExecutor.ExecuteScript(scriptPath);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteScript_ScriptPathIsEmpty_ArgumentExceptionIsThrown()
        {
            // Arrange
            string scriptPath = string.Empty;

            // Act
            this.powerShellScriptExecutor.ExecuteScript(scriptPath);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteScript_ScriptPathIsWhitespace_ArgumentExceptionIsThrown()
        {
            // Arrange
            string scriptPath = " ";

            // Act
            this.powerShellScriptExecutor.ExecuteScript(scriptPath);
        }

        [Test]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ExecuteScript_SuppliedScriptPathDoesNotExist_FileNotFoundExceptionIsThrown()
        {
            string scriptPath = GetRelativeScriptPath("Non-Existent-Script.ps1");

            // Act
            this.powerShellScriptExecutor.ExecuteScript(scriptPath);
        }

        [Test]
        public void ExecuteScript_SimpleScriptNoParameters_RelativePath_NoParameters_ScriptResultsAreReturned()
        {
            // Arrange
            string scriptPath = GetRelativeScriptPath("TestScript-01-ReturnResults.ps1");

            // Act
            string result = this.powerShellScriptExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual("TestScript-01-ReturnResults.ps1", result.Trim());
        }

        [Test]
        public void ExecuteScript_SimpleScriptNoParameters_AbsolutePath_NoParameters_ScriptResultsAreReturned()
        {
            // Arrange
            string scriptPath = GetAbsoluteScriptPath("TestScript-01-ReturnResults.ps1");

            // Act
            string result = this.powerShellScriptExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual("TestScript-01-ReturnResults.ps1", result.Trim());
        }

        [Test]
        public void ExecuteScript_ParameterizedScript_RequiredParameterIsSupplied_Positional_ScriptReturnsParameterAsSupplied()
        {
            // Arrange
            string parameterValue = "Full";
            var parameters = new[] { parameterValue };

            string scriptPath = GetAbsoluteScriptPath("TestScript-02-RequiredParameter-Echo-Test.ps1");

            // Act
            string result = this.powerShellScriptExecutor.ExecuteScript(scriptPath, parameters);

            // Assert
            Assert.AreEqual(parameterValue, result.Trim());
        }

        [Test]
        public void ExecuteScript_ParameterizedScript_RequiredParameterIsSupplied_Named_ScriptReturnsParameterAsSupplied()
        {
            // Arrange
            string parameterName = "DeploymentType";
            string parameterValue = "Full";
            var parameters = new[] { string.Format("-{0} {1}", parameterName, parameterValue) };

            string scriptPath = GetAbsoluteScriptPath("TestScript-02-RequiredParameter-Echo-Test.ps1");

            // Act
            string result = this.powerShellScriptExecutor.ExecuteScript(scriptPath, parameters);

            // Assert
            Assert.AreEqual(parameterValue, result.Trim());
        }

        [Test]
        public void ExecuteScript_ScriptWhichCallsAnotherScript_DependentScriptIsExecuted()
        {
            // Arrange
            string scriptPath = GetAbsoluteScriptPath("TestScript-03-CallAnotherScript.ps1");

            // Act
            string result = this.powerShellScriptExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual("TestScript-03-AnotherScript.ps1", result.Trim());
        }

        [Test]
        public void ExecuteScript_ScriptRequiresInput_NothingIsReturned()
        {
            // Arrange
            string scriptPath = GetAbsoluteScriptPath("TestScript-04-InputRequired.ps1");

            // Act
            var result = this.powerShellScriptExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
        }

        [Test]
        public void ExecuteScript_ScriptImportsIISModule_LocationIsIIS()
        {
            // Arrange
            string scriptPath = GetAbsoluteScriptPath("TestScript-05-ImportModule-IIS.ps1");

            // Act
            string result = this.powerShellScriptExecutor.ExecuteScript(scriptPath);

            // Assert
            Assert.AreEqual("IIS:\\", result.Trim());
        }

        #endregion

        #region utility methods

        private static string GetAbsoluteScriptPath(string scriptName)
        {
            return Path.Combine(Environment.CurrentDirectory, GetRelativeScriptPath(scriptName));
        }

        private static string GetRelativeScriptPath(string scriptName)
        {
            return Path.Combine("IntegrationTests\\PowerShell", scriptName);
        }

        #endregion
    }
}
