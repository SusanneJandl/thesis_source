using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Test_Logger
{
    internal class Log
    {
        public static bool laptop = false;
        public static DateTime startTotal;
        public static DateTime doneTotal;

        public static string logFilePath = laptop? "C:\\Users\\susan\\Documents\\bachelor-thesis_data\\tests\\laptop\\01_widget\\testresults.md" : "C:\\Users\\Utente\\Documents\\repos\\bachelor-thesis_data\\tests\\PC_8\\01_widget\\testresults.md";
        
        public static void TimeLog(DateTime start, DateTime done, string purpose)
        {
            double seconds = (done - start).TotalSeconds;
            string line = $" | {purpose}: {seconds:F2} s{Environment.NewLine}";
            File.AppendAllText(logFilePath, line);
        }

        public static void StringLog(string type, string log, string? semicolon = ": ")
        {
            File.AppendAllText(logFilePath, type + semicolon + log + Environment.NewLine);
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
