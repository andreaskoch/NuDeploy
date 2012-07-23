using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;

using NuDeploy.Core.Common.UserInterface;

namespace CommandLine.Tests
{
    public class LoggingUserInterface
    {
        private readonly StringBuilder output;

        private readonly Mock<IUserInterface> loggingUserInterface;

        public LoggingUserInterface()
        {
            this.output = new StringBuilder();
            this.loggingUserInterface = new Mock<IUserInterface>();

            this.loggingUserInterface.Setup(u => u.Write(It.IsAny<string>())).Callback((string text) => this.output.Append(text));

            this.loggingUserInterface.Setup(u => u.WriteLine(It.IsAny<string>())).Callback((string text) => this.output.AppendLine(text));

            this.loggingUserInterface.Setup(u => u.ShowIndented(It.IsAny<string>(), It.IsAny<int>())).Callback((string text, int indent) => this.output.AppendLine(text));

            this.loggingUserInterface.Setup(u => u.ShowKeyValueStore(It.IsAny<IDictionary<string, string>>(), It.IsAny<int>())).Callback(
                (IDictionary<string, string> keyValueStore, int distanceBetweenColumns) =>
                {
                    var flatList = keyValueStore.Select(pair => pair.Key + " " + pair.Value).ToList();
                    this.output.AppendLine(string.Join(Environment.NewLine, flatList));
                });

            this.loggingUserInterface.Setup(u => u.ShowKeyValueStore(It.IsAny<IDictionary<string, string>>(), It.IsAny<int>(), It.IsAny<int>())).Callback(
                (IDictionary<string, string> keyValueStore, int distanceBetweenColumns, int indentation) =>
                {
                    var flatList = keyValueStore.Select(pair => pair.Key + " " + pair.Value).ToList();
                    this.output.AppendLine(string.Join(Environment.NewLine, flatList));
                });

            this.loggingUserInterface.Setup(u => u.ShowLabelValuePair(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Callback(
                (string label, string value, int distanceBetweenLabelAndValue) => this.output.AppendLine(label + " " + value));    
        }

        public string UserInterfaceOutput
        {
            get
            {
                return this.output.ToString();
            }
        }

        public IUserInterface UserInterface
        {
            get
            {
                return this.loggingUserInterface.Object;
            }
        }
    }
}