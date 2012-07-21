using System.Collections.Generic;

using Moq;

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

        public static ICommand GetCommand(string commandName)
        {
            var commandAttributes = new CommandAttributes
                {
                    CommandName = commandName,
                    Description = "Yada yada " + commandName,
                    AlternativeCommandNames = new string[] { },
                    Examples = new Dictionary<string, string>
                        {
                            { commandName + " 1", "Some description for " + commandName + " 1" },
                            { commandName + " 2", "Some description for " + commandName + " 2" }
                        },
                    ArgumentDescriptions = new Dictionary<string, string>
                        {
                            { "arg1", "Some description for " + commandName + " arg1" },
                            { "arg2", "Some description for " + commandName + " arg2" },
                            { "arg3", "Some description for " + commandName + " arg3" }
                        }
                };

            var command = new Mock<ICommand>();
            command.Setup(c => c.Attributes).Returns(commandAttributes);

            return command.Object;
        }
    }
}
