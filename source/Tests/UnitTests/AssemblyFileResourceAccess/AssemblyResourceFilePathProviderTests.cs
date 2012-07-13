using System;

using NuDeploy.Core.Services.AssemblyResourceAccess;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.AssemblyFileResourceAccess
{
    [TestFixture]
    public class AssemblyResourceFilePathProviderTests
    {
        private AssemblyResourceFilePathProvider assemblyResourceFilePathProvider = new AssemblyResourceFilePathProvider();

        [TestFixtureSetUp]
        public void Setup()
        {
            this.assemblyResourceFilePathProvider = new AssemblyResourceFilePathProvider();
        }

        [TestCase(null, "AssemblyName.Resources.File.txt")]
        [TestCase("", "AssemblyName.Resources.File.txt")]
        [TestCase(" ", "AssemblyName.Resources.File.txt")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRelativeFilePath_BaseNamespaceParameterIsNull_ArgumentExceptionIsThrown(string baseNamespace, string assemblyResourceName)
        {
            // Act
            this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);
        }

        [TestCase("AnotherAssembly", "AssemblyName.Resources.File.txt")]
        [TestCase("AssemblyName.Resources.SubResource", "AssemblyName.Resources.File.txt")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRelativeFilePath_BaseNamespaceIsNotPartOfTheSuppliedResource_ArgumentExceptionIsThrown(string baseNamespace, string assemblyResourceName)
        {
            // Act
            this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);
        }

        [TestCase("AssemblyName", null)]
        [TestCase("AssemblyName", "")]
        [TestCase("AssemblyName", " ")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetRelativeFilePath_AssemblyResourceNameIsValid_ArgumentExceptionIsThrown(string baseNamespace, string assemblyResourceName)
        {
            // Act
            this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);
        }

        [TestCase("AssemblyName.Resources.DeploymentScripts", "AssemblyName.Resources.DeploymentScripts.PostDeploy.ps1", Result = "PostDeploy.ps1")]
        [TestCase("AssemblyName.Resources", "AssemblyName.Resources.DeploymentScripts.PostDeploy.ps1", Result = @"DeploymentScripts\PostDeploy.ps1")]
        [TestCase("AssemblyName", "AssemblyName.Resources.DeploymentScripts.PostDeploy.ps1", Result = @"Resources\DeploymentScripts\PostDeploy.ps1")]
        public string GetRelativeFilePath_BaseNamespaceParameterIsValid_AssemblyResourceNameIsValid_ResultIsCorrectResourcePath(string baseNamespace, string assemblyResourceName)
        {
            // Act
            return this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);
        }

        [Test]
        public void GetRelativeFilePath_BaseNamespaceParameterIsValid_AssemblyResourceHasNoFileExtension_ResultIsCorrectResourcePath()
        {
            // Arrange
            string fileName = "PostDeploy";
            string baseNamespace = "AssemblyName.Resources.DeploymentScripts";
            string assemblyResourceName = string.Format("AssemblyName.Resources.DeploymentScripts.{0}", fileName);

            // Act
            var result = this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);

            // Assert
            Assert.AreEqual(fileName, result);
        }

        [TestCase("AssemblyName.Resources.DeploymentScripts", "AssemblyName.Resources.DeploymentScripts.{0}", ".settings")]
        [TestCase("AssemblyName.Resources", "AssemblyName.Resources.{0}", @"DeploymentScripts\.settings")]
        public void GetRelativeFilePath_BaseNamespaceParameterIsValid_AssemblyResourceHasNoFileExtension_AndStartsWithADot_ResultIsCorrectResourcePath(string baseNamespace, string assemblyResourceNamePattern, string filename)
        {
            // Arrange
            string assemblyResourceName = string.Format(assemblyResourceNamePattern, filename);

            // Act
            var result = this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);

            // Assert
            Assert.AreEqual(filename, result);
        }

        [TestCase("AssemblyName.Resources.DeploymentScripts", "AssemblyName.Resources.DeploymentScripts.{0}", ".settings.txt")]
        [TestCase("AssemblyName.Resources.DeploymentScripts", "AssemblyName.Resources.DeploymentScripts.{0}", ".settings.backup.txt")]
        [TestCase("AssemblyName.Resources.DeploymentScripts", "AssemblyName.Resources.DeploymentScripts.{0}", "settings.backup.txt")]
        public void GetRelativeFilePath_UnsupportedFileNames(string baseNamespace, string assemblyResourceNamePattern, string filename)
        {
            // Arrange
            string assemblyResourceName = string.Format(assemblyResourceNamePattern, filename);

            // Act
            var result = this.assemblyResourceFilePathProvider.GetRelativeFilePath(baseNamespace, assemblyResourceName);

            // Assert
            Console.WriteLine("The correct result would be \"{0}\", but the class currently cannot handle these file names and returns \"{1}\" instead.", filename, result);
            Assert.AreNotEqual(filename, result);
        }
    }
}