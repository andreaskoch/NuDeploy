using System;

using NuDeploy.Core.Services.Installation.Repositories;

using NuGet;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Repositories
{
    [TestFixture]
    public class CommandLineRepositoryFactoryTests
    {
        private IPackageRepositoryFactory commandLineRepositoryFactory;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.commandLineRepositoryFactory = new CommandLineRepositoryFactory();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateRepository_PackageSourceParameterIsInvalid_ArgumentExceptionIsThrown(string packageSource)
        {
            // Act
            this.commandLineRepositoryFactory.CreateRepository(packageSource);
        }
    }
}