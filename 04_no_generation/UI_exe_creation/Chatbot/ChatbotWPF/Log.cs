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

        public static string logFilePath = Consts.DEVICE == "laptop"? "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop\\04_no_generation\\testresults.md" : "C:\\Users\\Utente\\Documents\\repos\\bachelor-thesis_data\\tests\\PC\\04_no_generation\\testresults.md";
        
        public static void Language()
        {
            File.AppendAllText(logFilePath, "LANGUAGE: " + Consts.LANGUAGE + Environment.NewLine);
        }

        public static void Accuracy()
        {
            File.AppendAllText(logFilePath, "ACCURACY: " + Consts.ACCURACY + Environment.NewLine);
        }
        public static void TimeLog(DateTime start, DateTime done, string purpose)
        {
            double seconds = (done - start).TotalSeconds;
            string line = $" | {purpose}: {seconds:F2} s{Environment.NewLine}";
            File.AppendAllText(logFilePath, line);
        }

        public static void RamUsage(string purpose, int MIN, int MAX, int AVG)
        {
            File.AppendAllText(logFilePath, Environment.NewLine + purpose + ": MIN=" + MIN + " MB | MAX=" + MAX + " MB | AVG=" + AVG + " MB");
        }
        public static void End()
        {
            File.AppendAllText(logFilePath, Environment.NewLine + Environment.NewLine + "================================================================" + Environment.NewLine);
        }
    }
}
