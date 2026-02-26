namespace Test_Logger
{
    internal class Program
    {
        private static bool running = false;
        private static int runCount = 0;
        private const int maxRuns = 8;

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
                    if (!running)
                    {
                        startTime = DateTime.Now;

                        runCount++;

                        if (runCount > maxRuns)
                            runCount = 1;

                        Log.StringLog("", $"# {runCount}\nCONTEXT:\nANSWER:\n", "");

                        Console.WriteLine($"[START] Run #{runCount} tracking started...");
                        RamTracker.Start();
                        running = true;
                    }
                    else
                    {
                        Console.WriteLine("[STOP] RAM tracking stopped and logged.");
                        endTime = DateTime.Now;
                        Log.TimeLog(startTime, endTime, $"Run #{runCount} Duration");
                        RamTracker.StopAndLog();
                        running = false;
                    }
                }
            }

            Console.WriteLine("Exiting...");
        }
    }
}
