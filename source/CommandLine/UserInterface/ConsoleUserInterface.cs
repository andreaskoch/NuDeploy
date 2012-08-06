using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NuDeploy.Core.Common.Logging;
using NuDeploy.Core.Common.UserInterface;
using NuDeploy.Core.Services;

namespace NuDeploy.CommandLine.UserInterface
{
    public class ConsoleUserInterface : IUserInterface
    {
        private readonly IConsoleTextManipulation textManipulation;

        private readonly IActionLogger logger;

        private readonly IServiceResultVisualizer serviceResultVisualizer;

        private readonly StringBuilder userInterfaceContent = new StringBuilder();

        public ConsoleUserInterface(IConsoleTextManipulation textManipulation, IActionLogger logger, IServiceResultVisualizer serviceResultVisualizer)
        {
            if (textManipulation == null)
            {
                throw new ArgumentNullException("textManipulation");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (serviceResultVisualizer == null)
            {
                throw new ArgumentNullException("serviceResultVisualizer");
            }

            this.textManipulation = textManipulation;
            this.logger = logger;
            this.serviceResultVisualizer = serviceResultVisualizer;
        }

        public int WindowWidth
        {
            get
            {
                try
                {
                    return Console.WindowWidth;
                }
                catch (IOException)
                {
                    return 60;
                }
            }

            set
            {
                Console.WindowWidth = value;
            }
        }

        public string UserInterfaceContent
        {
            get
            {
                return this.userInterfaceContent.ToString();
            }
        }

        public string GetInput()
        {
            this.CaptureLine("Requesting input from user.");
            string input = Console.ReadLine();

            this.CaptureLine("User entered {0}", input);
            return input;
        }

        public void Display(IServiceResult serviceResult)
        {
            this.serviceResultVisualizer.Display(this, serviceResult);
        }

        public void ShowIndented(string text, int marginLeft)
        {       
            string indentedText = this.textManipulation.IndentText(text, this.WindowWidth, marginLeft);

            Console.WriteLine(indentedText);
            this.CaptureLine(text);
        }

        public void Write(string text)
        {
            Console.Write(text);
            this.Capture(text);
        }

        public void WriteLine(string text)
        {
            string wrappedText = this.textManipulation.WrapText(text, this.WindowWidth);

            Console.WriteLine(wrappedText);
            this.CaptureLine(text);
        }

        public void ShowLabelValuePair(string label, string value, int distanceBetweenLabelAndValue)
        {
            this.ShowKeyValueStore(new Dictionary<string, string> { { label, value } }, distanceBetweenLabelAndValue);
        }

        public void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns, int indentation)
        {
            var margin = new string(' ', indentation);
            this.ShowKeyValueStore(keyValueStore.ToDictionary(pair => margin + pair.Key, pair => pair.Value), distanceBetweenColumns);
        }

        public void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns)
        {
            int keyColumnWidth = keyValueStore.Keys.Max(k => k.Length) + distanceBetweenColumns;
            int valueColumnWidth = this.WindowWidth - keyColumnWidth - 4;

            foreach (var keyValuePair in keyValueStore)
            {
                string keyColumnText = string.Format("{0,-" + keyColumnWidth + "}", keyValuePair.Key);
                string valueColumnText = this.textManipulation.WrapLongTextWithHangingIndentation(keyValuePair.Value, valueColumnWidth, keyColumnWidth);

                string text = string.Concat(keyColumnText, valueColumnText);

                Console.Write(text + Environment.NewLine);
                this.CaptureLine(text);
            }
        }

        private void Capture(string text, params object[] arguments)
        {
            this.userInterfaceContent.Append(string.Format(text, arguments));
            this.logger.Log(text, arguments);
        }

        private void CaptureLine(string text, params object[] arguments)
        {
            this.userInterfaceContent.AppendLine(string.Format(text, arguments));
            this.logger.Log(text, arguments);
        }
    }
}