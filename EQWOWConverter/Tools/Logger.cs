//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2025 Nathan Handley
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class Logger
    {
        // Rows written to the console, for tracking
        private static Dictionary<int, string> ConsoleRows = new Dictionary<int, string>();

        public static void ResetLog()
        {
            if (File.Exists("log.txt"))
                File.Delete("log.txt");
        }

        private static void WriteToConsole(string text, bool outputNewLine = true)
        {
            int row = Console.CursorTop;
            int col = Console.CursorLeft;

            if (ConsoleRows.ContainsKey(row) == false)
                ConsoleRows[row] = new string(' ', Console.BufferWidth);

            char[] rowBuffer = ConsoleRows[row].ToCharArray();
            for (int i = 0; i < text.Length && col + i < Console.BufferWidth; i++)
                rowBuffer[col + i] = text[i];

            ConsoleRows[row] = new string(rowBuffer);
            if (outputNewLine == true)
                Console.WriteLine(text);
            else
                Console.Write(text);
        }

        public static void WriteInfo(string text, bool outputNewLine = true, bool includeLeaderBlock = true)
        {
            string outputLine;
            if (includeLeaderBlock == true)
                outputLine = "[ ] Info | " + text;
            else
                outputLine = text;
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 1)
                WriteToConsole(outputLine, outputNewLine);
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 1)
                File.AppendAllText("log.txt", outputLine + "\n");
        }

        public static void WriteDetail(string text)
        {
            string outputLine = "[.] Detail| " + text;
            if (Configuration.CONFIG_LOGGING_CONSOLE_MIN_LEVEL >= 3)
                WriteToConsole(outputLine);
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 3)
                File.AppendAllText("log.txt", outputLine + "\n");
        }

        public static void WriteError(string text)
        {
            string outputLine = "[*] Error| " + text;
            if (Configuration.CONFIG_LOGGING_CONSOLE_MIN_LEVEL >= 2)
                WriteToConsole(outputLine);
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 2)
                File.AppendAllText("log.txt", outputLine + "\n");
        }

        public static void WriteCounter(int number, int startPosition, int totalNumber = 0)
        {
            string outputString;
            if (totalNumber != 0)
                outputString = "(" + number.ToString() + " of " + totalNumber.ToString() + ")";
            else
                outputString = "(" + number.ToString() + ")";
            int cursorTop = Console.CursorTop - 1;
            Console.SetCursorPosition(startPosition, cursorTop);
            Console.Write(outputString);
            Console.SetCursorPosition(0, cursorTop + 1);
        }

        public static int GetConsolePriorRowCursorLeft()
        {
            int currentRow = Console.CursorTop;
            if (currentRow == 0)
                return 0;
            int previousRow = currentRow - 1;
            if (ConsoleRows.ContainsKey(previousRow) == false)
                return 0;
            string rowChars = ConsoleRows[previousRow];
            for (int i = rowChars.Length - 1; i >= 0; i--)
            {
                if (rowChars[i] != ' ')
                    return i;
            }
            return 0;
        }
    }
}
