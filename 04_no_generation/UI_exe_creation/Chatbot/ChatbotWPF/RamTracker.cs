using System.Diagnostics;
using System.Timers;

namespace ChatbotWPF
{
    internal static class RamTracker
    {
        private static readonly List<double> _samplesTotalMb = [];
        private static readonly List<double> _samplesCSharpMb = [];
        private static readonly List<double> _samplesFlaskMb = [];

        private static readonly object _lock = new();
        private static System.Timers.Timer? _timer;

        public static void Start()
        {
            lock (_lock)
            {
                if (_timer != null)
                    return;

                _samplesTotalMb.Clear();
                _samplesCSharpMb.Clear();
                _samplesFlaskMb.Clear();
                
                _timer = new System.Timers.Timer(10);
                _timer.AutoReset = true;
                _timer.Elapsed += Timer_Elapsed;
                _timer.Start();
            }
        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                double csharpMb = 0;
                double flaskMb = 0;
                
                using (Process current = Process.GetCurrentProcess())
                {
                    csharpMb = current.WorkingSet64 / (1024.0 * 1024.0);
                }

                flaskMb = GetProcessGroupMemoryMb("python");

                double totalMb = csharpMb + flaskMb;

                lock (_lock)
                {
                    _samplesCSharpMb.Add(csharpMb);
                    _samplesFlaskMb.Add(flaskMb);
                    _samplesTotalMb.Add(totalMb);
                }
            }
            catch{}
        }

        private static double GetProcessGroupMemoryMb(string nameSubstring)
        {
            try
            {
                double totalMb = 0;

                foreach (var p in Process.GetProcesses())
                {
                    try
                    {
                        if (p.ProcessName.Contains(nameSubstring, StringComparison.OrdinalIgnoreCase))
                        {
                            totalMb += p.WorkingSet64 / (1024.0 * 1024.0);
                        }
                    }
                    catch{}
                    finally
                    {
                        p.Dispose();
                    }
                }

                return totalMb;
            }
            catch
            {
                return 0;
            }
        }


        public static void StopAndLog(string purpose)
        {
            List<double> totalSnapshot;
            List<double> csharpSnapshot;
            List<double> flaskSnapshot;
            
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }

                totalSnapshot = [.. _samplesTotalMb];
                csharpSnapshot = [.. _samplesCSharpMb];
                flaskSnapshot = [.. _samplesFlaskMb];
                
                _samplesTotalMb.Clear();
                _samplesCSharpMb.Clear();
                _samplesFlaskMb.Clear();
            }

            if (totalSnapshot.Count == 0 &&
                csharpSnapshot.Count == 0 &&
                flaskSnapshot.Count == 0)
            {
                return;
            }

            static (int Min, int Max, int Avg) Summarize(List<double> list)
            {
                if (list == null || list.Count == 0)
                    return (0, 0, 0);

                double min = list.Min();
                double max = list.Max();
                double avg = list.Average();

                return ((int)Math.Round(min),
                        (int)Math.Round(max),
                        (int)Math.Round(avg));
            }

            var total = Summarize(totalSnapshot);
            var csharp = Summarize(csharpSnapshot);
            var flask = Summarize(flaskSnapshot);
            
            Log.RamUsage($"{purpose} TOTAL", total.Min, total.Max, total.Avg);
            Log.RamUsage($"{purpose} C#", csharp.Min, csharp.Max, csharp.Avg);
            Log.RamUsage($"{purpose} FLASK", flask.Min, flask.Max, flask.Avg);
            Log.End();
        }
    }
}
