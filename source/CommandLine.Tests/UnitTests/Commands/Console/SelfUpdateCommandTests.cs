using System;
using System.Reflection;
using System.Runtime.InteropServices;

using Moq;

using NuDeploy.CommandLine.Commands.Console;
using NuDeploy.Core.Common.Infrastructure;
using NuDeploy.Core.Services.Update;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands.Console
{
    [TestFixture]
    public class SelfUpdateCommandTests
    {
        #region constructor

        [Test]
        public void Constructor_AllParametersAreSet_ObjectIsInstantiated()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var selfUpdateService = new Mock<ISelfUpdateService>();
            var targetAssembly = new Mock<_Assembly>();

            // Act
            var selfupdateCommand = new SelfUpdateCommand(applicationInformation, selfUpdateService.Object, targetAssembly.Object);

            // Assert
            Assert.IsNotNull(selfupdateCommand);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ApplicationInformationParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var selfUpdateService = new Mock<ISelfUpdateService>();
            var targetAssembly = new Mock<_Assembly>();

            // Act
            new SelfUpdateCommand(null, selfUpdateService.Object, targetAssembly.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_SelfUpdateServiceParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var targetAssembly = new Mock<_Assembly>();

            // Act
            new SelfUpdateCommand(applicationInformation, null, targetAssembly.Object);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_TargetAssemblyParametersIsNotSet_ArgumentNullExceptionIsThrown()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var selfUpdateService = new Mock<ISelfUpdateService>();

            // Act
            new SelfUpdateCommand(applicationInformation, selfUpdateService.Object, null);
        }

        #endregion

        #region Execute

        [Test]
        public void Constructor_SelfUpdateIsCalled()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var selfUpdateService = new Mock<ISelfUpdateService>();
            var targetAssembly = new Mock<_Assembly>();

            string assemblyPath = @"C:\assembly.dll";
            var assemblyVersion = new Version(1, 0, 0);
            var assemblyName = new AssemblyName("jkjdkjas") { Version = assemblyVersion };

            targetAssembly.Setup(a => a.Location).Returns(assemblyPath);
            targetAssembly.Setup(a => a.GetName()).Returns(assemblyName);

            var selfupdateCommand = new SelfUpdateCommand(applicationInformation, selfUpdateService.Object, targetAssembly.Object);

            // Act
            selfupdateCommand.Execute();

            // Assert
            selfUpdateService.Verify(s => s.SelfUpdate(It.IsAny<string>(), It.IsAny<Version>()), Times.Once());
        }

        [Test]
        public void Constructor_SelfUpdateIsCalledWithTheAssembliesLocationAndVersion()
        {
            // Arrange
            var applicationInformation = new ApplicationInformation();
            var selfUpdateService = new Mock<ISelfUpdateService>();
            var targetAssembly = new Mock<_Assembly>();

            string assemblyPath = @"C:\assembly.dll";
            var assemblyVersion = new Version(1, 0, 0);
            var assemblyName = new AssemblyName("jkjdkjas") { Version = assemblyVersion };

            targetAssembly.Setup(a => a.Location).Returns(assemblyPath);
            targetAssembly.Setup(a => a.GetName()).Returns(assemblyName);

            var selfupdateCommand = new SelfUpdateCommand(applicationInformation, selfUpdateService.Object, targetAssembly.Object);

            // Act
            selfupdateCommand.Execute();

            // Assert
            selfUpdateService.Verify(s => s.SelfUpdate(assemblyPath, assemblyVersion), Times.Once());
        }

        #endregion
    }
}