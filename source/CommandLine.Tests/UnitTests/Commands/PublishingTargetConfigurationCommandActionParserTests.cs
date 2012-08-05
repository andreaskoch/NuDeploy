using NuDeploy.CommandLine.Commands;

using NUnit.Framework;

namespace NuDeploy.CommandLine.Tests.UnitTests.Commands
{
    [TestFixture]
    public class PublishingTargetConfigurationCommandActionParserTests
    {
        private IPublishingTargetConfigurationCommandActionParser publishingTargetConfigurationCommandActionParser;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.publishingTargetConfigurationCommandActionParser = new PublishingTargetConfigurationCommandActionParser();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ParseAction_SuppliedActionNameIsInvalid_ResultIsDefaultAction(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandActionParser.DefaultAction, result);
        }

        [TestCase("dasdasdsa")]
        [TestCase("update")]
        [TestCase("remove")]
        public void ParseAction_SuppliedActionNameIsUnknown_ResultIsDefaultAction(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandActionParser.DefaultAction, result);
        }

        [TestCase("Unrecognized")]
        [TestCase("UNRECOGNIZED")]
        [TestCase("unrecognized")]
        [TestCase(" unrecognized")]
        [TestCase("unrecognized ")]
        [TestCase(" unrecognized ")]
        public void ParseAction_SuppliedActionNameIs_Unrecognized_CommandActionIsParsedCorrectly(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandAction.Unrecognized, result);
        }

        [TestCase("Add")]
        [TestCase("ADD")]
        [TestCase("add")]
        [TestCase(" add")]
        [TestCase("add ")]
        [TestCase(" add ")]
        public void ParseAction_SuppliedActionNameIs_Add_CommandActionIsParsedCorrectly(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandAction.Add, result);
        }

        [TestCase("Delete")]
        [TestCase("DELETE")]
        [TestCase("delete")]
        [TestCase(" delete")]
        [TestCase("delete ")]
        [TestCase(" delete ")]
        public void ParseAction_SuppliedActionNameIs_Delete_CommandActionIsParsedCorrectly(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandAction.Delete, result);
        }

        [TestCase("List")]
        [TestCase("LIST")]
        [TestCase("list")]
        [TestCase(" list")]
        [TestCase("list ")]
        [TestCase(" list ")]
        public void ParseAction_SuppliedActionNameIs_List_CommandActionIsParsedCorrectly(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandAction.List, result);
        }

        [TestCase("Reset")]
        [TestCase("RESET")]
        [TestCase("reset")]
        [TestCase(" reset")]
        [TestCase("reset ")]
        [TestCase(" reset ")]
        public void ParseAction_SuppliedActionNameIs_Reset_CommandActionIsParsedCorrectly(string actionName)
        {
            // Act
            var result = this.publishingTargetConfigurationCommandActionParser.ParseAction(actionName);

            // Assert
            Assert.AreEqual(PublishingTargetConfigurationCommandAction.Reset, result);
        }
    }
}