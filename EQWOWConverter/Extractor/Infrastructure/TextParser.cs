using System;
using System.Collections.Generic;
using System.Linq;

namespace LanternExtractor.Infrastructure
{
    /// <summary>
    /// A simple class that parses text in the way that it is output/loaded by the Lantern extractor and Unity importer
    /// </summary>
    public static class TextParser
    {
        /// <summary>
        /// Parses a string by new line pruning any empty lines
        /// Can also ignore comment lines
        /// </summary>
        /// <param name="text">The text file to be parsed</param>
        /// <param name="commentChar">The character that denotes a comment line</param>
        /// <returns>A list of parsed lines</returns>
        public static List<string> ParseTextByNewline(string text, char commentChar = '#')
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            string[] textLines = text.Split(new[] {Environment.NewLine, "\r\n", "\r", "\n"}, StringSplitOptions.None);


            return textLines.Where(line => !string.IsNullOrEmpty(line))
                .Where(line => !line.StartsWith(commentChar.ToString())).ToList();
        }

        /// <summary>
        /// Parses text using an empty line to split the text
        /// </summary>
        /// <param name="text">The text file to be parsed</param>
        /// <param name="commentChar">The character that denotes a comment line</param>
        /// <returns>A list of parsed lines</returns>
        public static List<string> ParseTextByEmptyLines(string text, char commentChar = '#')
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            string[] textLines = text.Split(new[] {Environment.NewLine + Environment.NewLine}, StringSplitOptions.None);

            return textLines.Where(line => !string.IsNullOrEmpty(line))
                .Where(line => !line.StartsWith(commentChar.ToString())).ToList();
        }

        /// <summary>
        /// First parses the text into lines and then splits the line by a specific character
        /// Also supports comment characters
        /// </summary>
        /// <param name="text">The text file to be parsed</param>
        /// <param name="delimiter">The character to split the line by</param>
        /// <param name="commentChar">The character that denotes a comment line</param>
        /// <returns></returns>
        public static List<List<string>> ParseTextByDelimitedLines(string text, char delimiter, char commentChar)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            if (string.IsNullOrEmpty(delimiter.ToString()))
            {
                return null;
            }

            List<string> parsedLines = ParseTextByNewline(text, commentChar);

            var parsedOutput = new List<List<string>>();

            foreach (var line in parsedLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] splitLine = line.Split(delimiter);

                parsedOutput.Add(splitLine.ToList());
            }

            return parsedOutput;
        }

        /// <summary>
        /// Parses lines with two string separated by a delimiter into a dictionary - useful for settings files
        /// E.g. "PlayerHealth = 10"
        /// Supports comment characters
        /// </summary>
        /// <param name="text">The text file to be parsed</param>
        /// <param name="delimiter">The character to split the line by</param>
        /// <param name="commentChar">The character that denotes a comment line</param>
        /// <returns>A dictionary containing the values</returns>
        public static Dictionary<string, string> ParseTextToDictionary(string text, char delimiter, char commentChar)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            if (string.IsNullOrEmpty(delimiter.ToString()))
            {
                return null;
            }

            List<string> parsedLines = ParseTextByNewline(text, commentChar);

            var parsedOutput = new Dictionary<string, string>();

            foreach (var line in parsedLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] splitLine = line.Split(delimiter);

                if (splitLine.Length != 2)
                {
                    continue;
                }

                parsedOutput[splitLine[0].Trim()] = splitLine[1].Trim();
            }

            return parsedOutput;
        }

        public static List<string> ParseStringToList(string text)
        {
            List<string> returnList = new List<string>();

            if (string.IsNullOrEmpty(text))
            {
                return returnList;
            }

            string[] strings = text.Split(';');

            return strings.ToList();
        }
    }
}