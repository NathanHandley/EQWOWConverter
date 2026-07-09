//  Author: Nathan Handley (nathanhandley@protonmail.com)
//  Copyright (c) 2024 Nathan Handley
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
using System.Text;

namespace EQWOWConverter
{
    internal class Logger
    {
        // Rows written to the console, for tracking
        private static readonly object writeLock = new object();

        private static StreamWriter? fileWriter = null;

        public static void ResetLog()
        {
            lock (writeLock)
            {
                CloseFileWriter();
                try
                {
                    if (File.Exists("log.txt"))
                        File.Delete("log.txt");
                }
                catch { /* best effort */ }
            }
        }

        private static void AppendToLogFile(string text)
        {
            try
            {
                if (fileWriter == null)
                    fileWriter = new StreamWriter(new FileStream("log.txt", FileMode.Append, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
                fileWriter.Write(text);
            }
            catch {}
        }

        private static void CloseFileWriter()
        {
            try { fileWriter?.Dispose(); } catch { }
            fileWriter = null;
        }

        private static void WriteToConsole(string text, ConsoleColor consoleColor, bool outputNewLine = true)
        {
            lock (writeLock)
            {
                Console.ForegroundColor = consoleColor;
                if (outputNewLine == true)
                    Console.WriteLine(text);
                else
                    Console.Write(text);
                Console.ResetColor();
            }
        }

        public static void WriteInfoAndSetupCounter(string text, out int messageEndLeft, out int messageEndTop)
        {
            lock (writeLock)
            {
                messageEndLeft = 0;
                messageEndTop = 0;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("[ ] Info | ");
                stringBuilder.Append(text);

                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(stringBuilder.ToString());
                    Console.ResetColor();
                    if (Console.IsOutputRedirected == false)
                    {
                        messageEndLeft = Console.CursorLeft;
                        messageEndTop = Console.CursorTop;
                        int topBeforeNewLine = Console.CursorTop;
                        Console.WriteLine();
                        if (Console.CursorTop == topBeforeNewLine)
                            messageEndTop -= 1;
                    }
                    else
                        Console.WriteLine();

                    stringBuilder.Append("\n");
                    AppendToLogFile(stringBuilder.ToString());
                }
            }
        }

        public static void WriteForCounter(string outputString, int messageEndLeft, int messageEndTop)
        {
            lock (writeLock)
            {
                if (Console.IsOutputRedirected == true)
                    return;
                int currentCursorLeft = Console.CursorLeft;
                int currentCursorTop = Console.CursorTop;
                try
                {
                    Console.SetCursorPosition(messageEndLeft, messageEndTop);
                    Console.Write(outputString);
                    Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // The counter's line has scrolled out of the console buffer (or it was resized), so skip this refresh
                }
            }
        }

        public static void WriteInfo(string text, bool outputNewLine = true, bool includeLeaderBlock = true)
        {
            lock (writeLock)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (includeLeaderBlock == true)
                    stringBuilder.Append("[ ] Info | ");
                stringBuilder.Append(text);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                    WriteToConsole(stringBuilder.ToString(), ConsoleColor.White, outputNewLine);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                {
                    stringBuilder.Append("\n");
                    AppendToLogFile(stringBuilder.ToString());
                }
            }
        }

        public static void WriteInfo(params string[] textParts)
        {
            lock (writeLock)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("[ ] Info | ");
                foreach (string text in textParts)
                    stringBuilder.Append(text);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                    WriteToConsole(stringBuilder.ToString(), ConsoleColor.White);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 1)
                {
                    stringBuilder.Append("\n");
                    AppendToLogFile(stringBuilder.ToString());
                }
            }
        }

        public static void WriteDebug(params string[] textParts)
        {
            if (Configuration.LOGGING_CONSOLE_MIN_LEVEL < 3 && Configuration.LOGGING_FILE_MIN_LEVEL < 3)
                return;

            lock (writeLock)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("[.] Detail| ");
                foreach (string text in textParts)
                    stringBuilder.Append(text);
                if (Configuration.LOGGING_CONSOLE_MIN_LEVEL >= 3)
                    WriteToConsole(stringBuilder.ToString(), ConsoleColor.Gray);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 3)
                {
                    stringBuilder.Append("\n");
                    AppendToLogFile(stringBuilder.ToString());
                }
            }
        }

        // TODO: Make this its own log level
        public static void WriteWarning(params string[] textParts)
        {
            if (Configuration.LOGGING_CONSOLE_MIN_LEVEL < 3 && Configuration.LOGGING_FILE_MIN_LEVEL < 3)
                return;

            lock (writeLock)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("[!] Warning| ");
                foreach (string text in textParts)
                    stringBuilder.Append(text);
                if (Configuration.LOGGING_CONSOLE_MIN_LEVEL >= 3)
                    WriteToConsole(stringBuilder.ToString(), ConsoleColor.Magenta);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 3)
                {
                    stringBuilder.Append("\n");
                    AppendToLogFile(stringBuilder.ToString());
                }
            }
        }

        public static void WriteError(params string[] textParts)
        {
            lock (writeLock)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("[*] Error| ");
                foreach(string text in textParts)
                    stringBuilder.Append(text);
                if (Configuration.LOGGING_CONSOLE_MIN_LEVEL >= 2)
                    WriteToConsole(stringBuilder.ToString(), ConsoleColor.Red);
                if (Configuration.LOGGING_FILE_MIN_LEVEL >= 2)
                {
                    stringBuilder.Append("\n");
                    AppendToLogFile(stringBuilder.ToString());
                }
            }
        }
    }
}
