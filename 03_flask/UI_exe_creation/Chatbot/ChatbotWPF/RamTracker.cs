using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace ChatbotWPF
{
    internal static class RamTracker
    {
        private static readonly List<double> _samplesMb = new();
        private static readonly object _lock = new();
        private static System.Timers.Timer? _timer;

        public static void Start()
        {
            lock (_lock)
            {
                // Already running? Don’t start again.
                if (_timer != null)
                    return;

                _samplesMb.Clear();

                _timer = new System.Timers.Timer(1000); // 1000 ms = 1 second
                _timer.AutoReset = true;
                _timer.Elapsed += Timer_Elapsed;
                _timer.Start();
            }
        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                using Process process = Process.GetCurrentProcess();
                double mb = process.WorkingSet64 / (1024.0 * 1024.0); // bytes → MB

                lock (_lock)
                {
                    _samplesMb.Add(mb);
                }
            }
            catch
            {
                // Swallow any sampling error – logging must not crash the app
            }
        }

        public static void StopAndLog(string purpose)
        {
            List<double> snapshot;

            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }

                snapshot = new List<double>(_samplesMb);
                _samplesMb.Clear();
            }

            if (snapshot.Count == 0)
            {
                // No samples, nothing to log
                return;
            }

            double min = snapshot.Min();
            double max = snapshot.Max();
            double avg = snapshot.Average();

            // Round to int MB for your existing Log.RamUsage(string, int)
            Log.RamUsage("C# RAM USAGE", (int)Math.Round(min), (int)Math.Round(max), (int)Math.Round(avg));
        }
    }
}
