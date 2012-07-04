using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuDeploy.Core.Common
{
    public class ConsoleUserInterface : IUserInterface
    {
        private readonly IConsoleTextManipulation textManipulation;

        private readonly IActionLogger logger;

        public ConsoleUserInterface(IConsoleTextManipulation textManipulation, IActionLogger logger)
        {
            this.textManipulation = textManipulation;
            this.logger = logger;
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

        public string GetInput()
        {
            this.logger.Log("Requesting input from user.");

            string input = Console.ReadLine();

            this.logger.Log("User entered {0}", input);
            return input;
        }

        public void ShowIndented(string text, int marginLeft)
        {       
            string indentedText = this.textManipulation.IndentText(text, this.WindowWidth, marginLeft);
            Console.WriteLine(indentedText);
            this.logger.Log(text);
        }

        public void Write(string text)
        {
            Console.Write(text);
            this.logger.Log(text);
        }

        public void WriteLine(string text)
        {
            string wrappedText = this.textManipulation.WrapText(text, this.WindowWidth);

            Console.WriteLine(wrappedText);
            this.logger.Log(text);
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
                this.logger.Log(text);
            }
        }
    }
}