using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace ollama_langflow
{
    internal static class RamTracker
    {
        private static readonly List<double> _samplesTotalMb = new();
        private static readonly List<double> _samplesCSharpMb = new();
        private static readonly List<double> _samplesLangflowMb = new();
        private static readonly List<double> _samplesOllamaMb = new();

        private static readonly object _lock = new();
        private static System.Timers.Timer? _timer;

        public static void Start()
        {
            lock (_lock)
            {
                if (_timer != null)
                    return; // already running

                _samplesTotalMb.Clear();
                _samplesCSharpMb.Clear();
                _samplesLangflowMb.Clear();
                _samplesOllamaMb.Clear();

                _timer = new System.Timers.Timer(500);
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
                double langflowMb = 0;
                double ollamaMb = 0;

                // C# (current) process
                using (Process current = Process.GetCurrentProcess())
                {
                    csharpMb = current.WorkingSet64 / (1024.0 * 1024.0);
                }

                // Flask – usually "python" on Windows
                langflowMb = GetProcessGroupMemoryMb("python");

                // Ollama – assume process name "ollama"
                // (adjust if your exe name differs)
                ollamaMb = GetProcessGroupMemoryMb("ollama");

                double totalMb = csharpMb + langflowMb + ollamaMb;

                lock (_lock)
                {
                    _samplesCSharpMb.Add(csharpMb);
                    _samplesLangflowMb.Add(langflowMb);
                    _samplesOllamaMb.Add(ollamaMb);
                    _samplesTotalMb.Add(totalMb);
                }
            }
            catch
            {
                // never let RAM tracking crash the app
            }
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
                        if (p.ProcessName.IndexOf(nameSubstring,
                                StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            totalMb += p.WorkingSet64 / (1024.0 * 1024.0);
                        }
                    }
                    catch
                    {
                        // process might have exited or be inaccessible
                    }
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
            List<double> langflowSnapshot;
            List<double> ollamaSnapshot;

            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                    _timer = null;
                }

                totalSnapshot = new List<double>(_samplesTotalMb);
                csharpSnapshot = new List<double>(_samplesCSharpMb);
                langflowSnapshot = new List<double>(_samplesLangflowMb);
                ollamaSnapshot = new List<double>(_samplesOllamaMb);

                _samplesTotalMb.Clear();
                _samplesCSharpMb.Clear();
                _samplesLangflowMb.Clear();
                _samplesOllamaMb.Clear();
            }

            if (totalSnapshot.Count == 0 &&
                csharpSnapshot.Count == 0 &&
                langflowSnapshot.Count == 0 &&
                ollamaSnapshot.Count == 0)
            {
                return; // nothing to log
            }

            // helper
            (int Min, int Max, int Avg) Summarize(List<double> list)
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

            // compute summaries
            var total = Summarize(totalSnapshot);
            var csharp = Summarize(csharpSnapshot);
            var langflow = Summarize(langflowSnapshot);
            var ollama = Summarize(ollamaSnapshot);

            // Log all four; adjust text as you like
            Log.StringLog("", "");
            Log.RamUsage($"{purpose} TOTAL", total.Min, total.Max, total.Avg);
            Log.RamUsage($"{purpose} C#", csharp.Min, csharp.Max, csharp.Avg);
            Log.RamUsage($"{purpose} LANGFLOW", langflow.Min, langflow.Max, langflow.Avg);
            Log.RamUsage($"{purpose} OLLAMA", ollama.Min, ollama.Max, ollama.Avg);
            Log.End();
        }
    }
}
