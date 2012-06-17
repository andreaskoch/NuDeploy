using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuDeploy.Core.Common
{
    public class ConsoleTextManipulation : IConsoleTextManipulation
    {
        public string WrapText(string text, int maxWidth)
        {
            if (text.Length <= maxWidth)
            {
                return text;
            }

            List<string> lines = WordWrap(text, maxWidth);
            return string.Join(Environment.NewLine, lines);
        }

        public string WrapLongTextWithHangingIndentation(string text, int maxWidth, int indentation)
        {
            if (text.Length <= maxWidth)
            {
                return text;
            }

            string indentationText = new string(' ', indentation);
            List<string> lines = WordWrap(text, maxWidth - indentation);
            return lines.First() + Environment.NewLine + string.Join(Environment.NewLine, lines.Skip(1).Select(line => indentationText + line));
        }

        public string IndentText(string text, int windowWidth, int marginLeft)
        {
            var indentation = new string(' ', marginLeft);
            List<string> lines = WordWrap(text, windowWidth - marginLeft);
            return string.Join(Environment.NewLine, lines.Select(line => indentation + line));
        }

        /// <summary>
        /// Word wraps the given text to fit within the specified width.
        /// </summary>
        /// <param name="text">Text to be word wrapped</param>
        /// <param name="width">Width, in characters, to which the text
        /// should be word wrapped</param>
        /// <returns>The modified text</returns>
        private static List<string> WordWrap(string text, int width)
        {
            int pos, next;
            var lines = new List<string>();

            // Lucidity check
            if (width < 1)
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
                        if (len > width)
                        {
                            len = BreakLine(text, pos, width);
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

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        private static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;

            while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            if (i < 0)
            {
                return max; // No whitespace found; break at maximum length
            }

            // Find start of whitespace
            while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
            {
                i--;
            }

            // Return length of text before whitespace
            return i + 1;
        }
    }
}