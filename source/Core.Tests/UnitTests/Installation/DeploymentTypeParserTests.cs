using NuDeploy.Core.Services.Installation;

using NUnit.Framework;

namespace NuDeploy.Tests.UnitTests.Installation
{
    [TestFixture]
    public class DeploymentTypeParserTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetDeploymentType_SuppliedStringIsInvalid_ResultIsDefaultDeploymentType(string deploymentTypeString)
        {
            // Arrange
            var deploymentTypeParser = new DeploymentTypeParser();

            // Act
            var result = deploymentTypeParser.GetDeploymentType(deploymentTypeString);

            // Assert
            Assert.AreEqual(DeploymentTypeParser.DefaultDeploymentType, result);
        }

        [TestCase("Invalid-Deployment-Type")]
        [TestCase("Total-Deployment")]
        [TestCase("DFJKLSDKLSJKLJSDKL")]
        [TestCase("F")]
        [TestCase("U")]
        public void GetDeploymentType_SuppliedStringIsNotAValidDeploymentType_ResultIsDefaultDeploymentType(string deploymentTypeString)
        {
            // Arrange
            var deploymentTypeParser = new DeploymentTypeParser();

            // Act
            var result = deploymentTypeParser.GetDeploymentType(deploymentTypeString);

            // Assert
            Assert.AreEqual(DeploymentTypeParser.DefaultDeploymentType, result);
        }

        [TestCase("Update")]
        [TestCase("update")]
        [TestCase("UPDATE")]
        [TestCase("Update ")]
        [TestCase(" Update")]
        [TestCase(" Update ")]
        public void GetDeploymentType_SuppliedStringIsRepresentsTheUpdateDeploymentType_ResultIsDefaultDeploymentType(string deploymentTypeString)
        {
            // Arrange
            var deploymentTypeParser = new DeploymentTypeParser();

            // Act
            var result = deploymentTypeParser.GetDeploymentType(deploymentTypeString);

            // Assert
            Assert.AreEqual(DeploymentType.Update, result);
        }

        [TestCase("Full")]
        [TestCase("full")]
        [TestCase("FULL")]
        [TestCase("Full ")]
        [TestCase(" Full")]
        [TestCase(" Full ")]
        public void GetDeploymentType_SuppliedStringIsRepresentsTheFullDeploymentType_ResultIsDefaultDeploymentType(string deploymentTypeString)
        {
            // Arrange
            var deploymentTypeParser = new DeploymentTypeParser();

            // Act
            var result = deploymentTypeParser.GetDeploymentType(deploymentTypeString);

            // Assert
            Assert.AreEqual(DeploymentType.Full, result);
        }
    }
}