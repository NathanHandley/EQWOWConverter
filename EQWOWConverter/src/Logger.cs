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

        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
            File.AppendAllText("log.txt", text + "\n");
        }
    }
}
