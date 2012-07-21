using System;
using System.Collections.Generic;
using System.Linq;

namespace NuDeploy.Core.Common.UserInterface.Console
{
    public class ConsoleTextManipulation : IConsoleTextManipulation
    {
        public string WrapText(string text, int maxWidth)
        {
            if (text.Length <= maxWidth)
            {
                return text;
            }

            List<string> lines = GetWrappedLines(text, maxWidth);
            return string.Join(Environment.NewLine, lines);
        }

        public string WrapLongTextWithHangingIndentation(string text, int maxWidth, int indentation)
        {
            if (text.Length <= maxWidth)
            {
                return text;
            }

            var indentationText = new string(' ', indentation);
            List<string> lines = GetWrappedLines(text, maxWidth - indentation);
            return lines.First() + Environment.NewLine + string.Join(Environment.NewLine, lines.Skip(1).Select(line => indentationText + line));
        }

        public string IndentText(string text, int windowWidth, int marginLeft)
        {
            var indentation = new string(' ', marginLeft);
            List<string> lines = GetWrappedLines(text, windowWidth - marginLeft);
            return string.Join(Environment.NewLine, lines.Select(line => indentation + line));
        }

        private static List<string> GetWrappedLines(string text, int maxLineWidth)
        {
            int pos, next;
            var lines = new List<string>();

            // Lucidity check
            if (maxLineWidth < 1)
            {
                return new List<string> { text };
            }

            // Parse each line of text
            for (pos = 0; pos < text.Length; pos = next)
            {
                // Find end of line
                int eol = text.IndexOf(Environment.NewLine, pos, StringComparison.Ordinal);
                if (eol == -1)
                {
                    next = eol = text.Length;
                }
                else
                {
                    next = eol + Environment.NewLine.Length;
                }

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;
                        if (len > maxLineWidth)
                        {
                            len = GetLineBreakPosition(text, pos, maxLineWidth);
                        }

                        lines.Add(text.Substring(pos, len));

                        // Trim whitespace following break
                        pos += len;
                        while (pos < eol && char.IsWhiteSpace(text[pos]))
                        {
                            pos++;
                        }
                    }
                    while (eol > pos);
                }
            }

            return lines;
        }

        private static int GetLineBreakPosition(string text, int startPosition, int maxLineLength)
        {
            // Find last whitespace in line
            int i = maxLineLength - 1;

            while (i >= 0 && !char.IsWhiteSpace(text[startPosition + i]))
            {
                i--;
            }

            if (i < 0)
            {
                // No whitespace found; break at maximum length
                return maxLineLength;
            }

            // Find start of whitespace
            while (i >= 0 && char.IsWhiteSpace(text[startPosition + i]))
            {
                i--;
            }

            // Return length of text before whitespace
            return i + 1;
        }
    }
}