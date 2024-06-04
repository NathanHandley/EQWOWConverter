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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQWOWConverter
{
    internal class Logger
    {
        public static void ResetLog()
        {
            if (File.Exists("log.txt"))
                File.Delete("log.txt");
        }

        public static void WriteInfo(string text, bool noNewLineInConsole = false)
        {
            string outputLine = "[ ] Info | " + text;
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 1)
            {
                if (noNewLineInConsole)
                    Console.Write(outputLine);
                else
                    Console.WriteLine(outputLine);
            }
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 1)
                File.AppendAllText("log.txt", outputLine + "\n");
        }

        public static void WriteDetail(string text)
        {
            string outputLine = "[.] Detail| " + text;
            if (Configuration.CONFIG_LOGGING_CONSOLE_MIN_LEVEL >= 3)
                Console.WriteLine(outputLine);
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 3)
                File.AppendAllText("log.txt", outputLine + "\n");
        }

        public static void WriteError(string text)
        {
            string outputLine = "[*] Error| " + text;
            if (Configuration.CONFIG_LOGGING_CONSOLE_MIN_LEVEL >= 2)
                Console.WriteLine(outputLine);
            if (Configuration.CONFIG_LOGGING_FILE_MIN_LEVEL >= 2)
                File.AppendAllText("log.txt", outputLine + "\n");
        }
    }
}
