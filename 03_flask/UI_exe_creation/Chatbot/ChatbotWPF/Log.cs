using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ChatbotWPF
{
    internal class Log
    {
        public static DateTime startTotal;
        public static DateTime doneTotal;

        public static string logFilePath = Consts.DEVICE == "laptop"? "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop\\03_flask\\testresults.md" : "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\PC\\03_flask\\testresults.md";
        public static void History ()
        {
            if (Consts.HISTORY)
            {
                File.AppendAllText(logFilePath, "HISTORY ENABLED: " + Consts.HISTORY + Environment.NewLine);
            }
        }

        public static void Language()
        {
            if (Consts.LANGUAGE.Equals("DE"))
            {
                File.AppendAllText(logFilePath, "LANGUAGE: " + Consts.LANGUAGE + Environment.NewLine);
            }
        }
        public static void TimeLog(DateTime start, DateTime done, string purpose)
        {
            double seconds = (done - start).TotalSeconds;
            string line = $"{Environment.NewLine}{Environment.NewLine}{purpose}: {seconds:F2} s{Environment.NewLine}";
            File.AppendAllText(logFilePath, line);
            File.AppendAllText(logFilePath, Environment.NewLine + "---" + Environment.NewLine);
        }

        public static void StringLog(string type, string log)
        {
            File.AppendAllText(logFilePath, type + ": " + log + Environment.NewLine);
        }

        public static void RamUsage(string purpose, int RAM)
        {
            File.AppendAllText(logFilePath, purpose + "RAM usage: " + RAM + " MB" + Environment.NewLine);
            File.AppendAllText(logFilePath, "---" + Environment.NewLine);
        }
    }
}
