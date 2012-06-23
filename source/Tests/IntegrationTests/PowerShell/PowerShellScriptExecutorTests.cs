using NuDeploy.Core.PowerShell;

using NUnit.Framework;

namespace NuDeploy.Tests.IntegrationTests.PowerShell
{
    [TestFixture]
    public class PowerShellScriptExecutorTests
    {
        private IPowerShellScriptExecutor powerShellScriptExecutor;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.powerShellScriptExecutor = new PowerShellScriptExecutor();
        }

        [Test]
        public void RunScript_ScriptIsEmpty_ResultIsEmpty()
        {
            // Arrange
            string script = string.Empty;

            // Act
            string result = this.powerShellScriptExecutor.RunScript(script);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }
    }
}
