using System;

namespace Test_Logger
{
    internal class Program
    {
        private static bool _running = false;
        private static int _runCount = 0;
        private const int MaxRuns = 8;

        static void Main(string[] args)
        {
            Console.WriteLine("=== RAM TRACKER ===");
            Console.WriteLine("Press ENTER to START.");
            Console.WriteLine("Press ENTER again to STOP and log.");
            Console.WriteLine("Press ESC to exit.");
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now;

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Escape)
                    break;

                if (key.Key == ConsoleKey.Enter)
                {
                    if (!_running)
                    {
                        startTime = DateTime.Now;

                        _runCount++;

                        if (_runCount > MaxRuns)
                            _runCount = 1; // wrap back to 1

                        // Log run number + line break to file
                        Log.StringLog("", $"# {_runCount}\nCONTEXT:\nANSWER:\n", "");

                        Console.WriteLine($"[START] Run #{_runCount} tracking started...");
                        RamTracker.Start();
                        _running = true;
                    }
                    else
                    {
                        Console.WriteLine("[STOP] RAM tracking stopped and logged.");
                        endTime = DateTime.Now;
                        Log.TimeLog(startTime, endTime, $"Run #{_runCount} Duration");
                        RamTracker.StopAndLog();
                        _running = false;
                    }
                }
            }

            Console.WriteLine("Exiting...");
        }
    }
}
