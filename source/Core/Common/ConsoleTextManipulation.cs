using System;
using System.Linq;
using System.Text;

namespace NuDeploy.Core.Common
{
    public class ConsoleTextManipulation : IConsoleTextManipulation
    {
        public string WrapLongTextWithHangingIndentation(string text, int maxWidth, int indentation)
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

        public string IndentText(string text, int windowWidth, int marginLeft)
        {
            var indentation = new string(' ', marginLeft);
            return this.WrapLongTextWithHangingIndentation(indentation + text, windowWidth, marginLeft);
        }
    }
}