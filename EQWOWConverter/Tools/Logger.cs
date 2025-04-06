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

using Mysqlx.Crud;

namespace EQWOWConverter
{
    internal class Logger
    {
        // Rows written to the console, for tracking
        private static readonly object writeLock = new object();

        public static void ResetLog()
        {
            lock (writeLock)
            {
                if (File.Exists("log.txt"))
                    File.Delete("log.txt");
            }
        }

        private static void WriteToConsole(string text, bool outputNewLine = true)
        {
            lock (writeLock)
            {
                if (outputNewLine == true)
                    Console.WriteLine(text);
                else
                    Console.Write(text);
            }
        }

        public static void WriteInfo(string text, bool outputNewLine = true, bool includeLeaderBlock = true)
        {
            lock (writeLock)
            {
                string outputLine;
                if (includeLeaderBlock == true)
                    outputLine = "[ ] Info | " + text;
                else
                    outputLine = text;
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                    WriteToConsole(outputLine, outputNewLine);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                    File.AppendAllText("log.txt", outputLine + "\n");
            }
        }

        public static void WriteDebug(string text)
        {
            if (Configuration.LOGGING_CONSOLE_MIN_LEVEL < 3 && Configuration.LOGGING_FILE_MIN_LEVEL < 3)
                return;

            lock (writeLock)
            {
                string outputLine = "[.] Detail| " + text;
                if (Configuration.LOGGING_CONSOLE_MIN_LEVEL >= 3)
                    WriteToConsole(outputLine);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 3)
                    File.AppendAllText("log.txt", outputLine + "\n");
            }
        }

        public static void WriteError(string text)
        {
            lock (writeLock)
            {
                string outputLine = "[*] Error| " + text;
                if (Configuration.LOGGING_CONSOLE_MIN_LEVEL >= 2)
                    WriteToConsole(outputLine);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 2)
                    File.AppendAllText("log.txt", outputLine + "\n");
            }
        }
    }
}
