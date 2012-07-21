using NuDeploy.CommandLine.Commands;

using NUnit.Framework;

namespace CommandLine.Tests.UnitTests.Commands
{
    public static class CommandTestUtilities
    {
        public static void ValidateCommandAttributes(CommandAttributes commandAttributes)
        {
            Assert.IsNotNull(commandAttributes, "The command attributes cannot be null.");
            Assert.IsNotNullOrEmpty(commandAttributes.CommandName, string.Format("The {0} cannot be null or empty.", "command name"));
            Assert.IsNotNullOrEmpty(commandAttributes.Description, string.Format("The {0} cannot be null or empty.", "command description"));
            Assert.IsNotNullOrEmpty(commandAttributes.Usage, string.Format("The {0} cannot be null or empty.", "command usage description"));
            Assert.IsTrue(commandAttributes.Examples.Count > 0, "The command attributes should contain at least one example");
        }
    }
}
