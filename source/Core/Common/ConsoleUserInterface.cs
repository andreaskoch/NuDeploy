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

        public void Show(string messageFormatString, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(messageFormatString);
            }
            else
            {
                Console.WriteLine(messageFormatString, args);
            }
        }

        public void ShowKeyValueStore(IDictionary<string, string> keyValueStore)
        {
            int keyColumnPadding = 4;
            int keyColumnWidth = keyValueStore.Keys.Max(k => k.Length) + keyColumnPadding;
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
                var line = paddingString + new string(chars.Take(maxWidth).ToArray());
                stringBuilder.Append(Environment.NewLine + line);
            }

            return stringBuilder.ToString();
        }
    }
}