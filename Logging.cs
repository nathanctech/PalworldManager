using System;
using System.Collections.Generic;
using System.Text;

namespace PalworldManager
{
    internal class Logging
    {
        public static void Info(string message)
        {
            var st = $"{DateTime.Now} [INFO] {message}";
            if (Program.Config?.LogToFile == true)
            {
                System.IO.File.AppendAllText("log.txt", st + Environment.NewLine);
            }
            Console.WriteLine(st);
        }

        public static void Warn(string message)
        {
            var st = $"{DateTime.Now} [WARN] {message}";
            if (Program.Config?.LogToFile == true)
            {
                System.IO.File.AppendAllText("log.txt", st + Environment.NewLine);
            }
            Console.WriteLine(st);
        }

        public static void Error(string message)
        {
            var st = $"{DateTime.Now} [ERROR] {message}";
            if (Program.Config?.LogToFile == true)
            {
                System.IO.File.AppendAllText("log.txt", st + Environment.NewLine);
            }
            Console.Error.WriteLine(st);
        }

        public static void Debug(string message)
        {
            if (Program.Config?.Debugging == true)
            {
                var st = $"{DateTime.Now} [DEBUG] {message}";
                if (Program.Config?.LogToFile == true)
                {
                    System.IO.File.AppendAllText("log.txt", st + Environment.NewLine);
                }
                Console.WriteLine(st);
            }
        }
    }
}
