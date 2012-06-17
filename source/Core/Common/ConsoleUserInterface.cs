using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NuDeploy.Core.Common
{
    public class ConsoleUserInterface : IUserInterface
    {
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

        public void ShowIndented(string text, int marginLeft)
        {
            var indentation = new string(' ', marginLeft);
            string wrappedText = this.WrapLongTextWithHangingIndentation(indentation + text, this.WindowWidth, marginLeft);
            Console.WriteLine(wrappedText);
        }

        public void Show(string text)
        {
            Console.WriteLine(text);
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
                string valueColumnText = this.WrapLongTextWithHangingIndentation(keyValuePair.Value, valueColumnWidth, keyColumnWidth);

                Console.Write(keyColumnText);
                Console.Write(valueColumnText);
                Console.Write(Environment.NewLine);
            }
        }

        private string WrapLongTextWithHangingIndentation(string text, int maxWidth, int indentation)
        {
            if (text.Length <= maxWidth)
            {
                return text;
            }

            char[] chars = text.ToCharArray();
            var stringBuilder = new StringBuilder();

            string paddingString = string.Empty.PadLeft(indentation);
            int charsToSkip = maxWidth;
            stringBuilder.Append(new string(chars.Take(maxWidth).ToArray()));

            while ((chars = chars.Skip(charsToSkip).Take(maxWidth).ToArray()).Any())
            {
                string line = new string(chars.Take(maxWidth).ToArray()).TrimStart();
                var indentedLine = paddingString + line;

                stringBuilder.Append(Environment.NewLine + indentedLine);
            }

            return stringBuilder.ToString();
        }
    }
}