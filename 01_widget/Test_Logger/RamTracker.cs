using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Test_Logger
{
    internal static class RamTracker
    {
        // Samples (MB)
        private static readonly List<double> _samplesTotalMb = new();
        private static readonly List<double> _samplesLangflowMb = new();
        private static readonly List<double> _samplesOllamaMb = new();
        
        private static readonly object _lock = new();
        private static System.Timers.Timer? _timer;

        // Adjust these if needed
        private const string LangflowNamePattern = "python";   // or "langflow" if you run it as that
        private const string OllamaNamePattern = "ollama";

        // Sampling interval (ms)
        private const double IntervalMs = 500;

        public static void Start()
        {
            lock (_lock)
            {
                if (_timer != null)
                    return; // already running

                _samplesTotalMb.Clear();
                _samplesLangflowMb.Clear();
                _samplesOllamaMb.Clear();
                
                _timer = new System.Timers.Timer(IntervalMs)
                {
                    AutoReset = true
                };
                _timer.Elapsed += Timer_Elapsed;
                _timer.Start();
            }
        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                double langflowMb = GetProcessGroupMemoryMbByName(LangflowNamePattern);
                double ollamaMb = GetProcessGroupMemoryMbByName(OllamaNamePattern);

                                // TOTAL = port-based Langflow + port-based Ollama
                double totalMb = langflowMb + ollamaMb;

                lock (_lock)
                {
                    _samplesLangflowMb.Add(langflowMb);
                    _samplesOllamaMb.Add(ollamaMb);

                    _samplesTotalMb.Add(totalMb);
                }
            }
            catch
            {
                // Never let RAM tracking crash the app
            }
        }

        // --- NAME-BASED MEMORY (substring match) -----------------------------------

        private static double GetProcessGroupMemoryMbByName(string nameSubstring)
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

        
        

        // --- STOP + LOG --------------------------------------------------------------

        public static void StopAndLog(string purpose)
        {
            List<double> totalSnapshot;
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
                langflowSnapshot = new List<double>(_samplesLangflowMb);
                ollamaSnapshot = new List<double>(_samplesOllamaMb);

                _samplesTotalMb.Clear();
                _samplesLangflowMb.Clear();
                _samplesOllamaMb.Clear();


                if (totalSnapshot.Count == 0 &&
                    langflowSnapshot.Count == 0 &&
                    ollamaSnapshot.Count == 0)
                {
                    return; // nothing to log
                }

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

                var totalPort = Summarize(totalSnapshot);
                var langflowName = Summarize(langflowSnapshot);
                var ollamaName = Summarize(ollamaSnapshot);

                // TOTAL is port-based by design
                Log.RamUsage($"{purpose} TOTAL_PORT", totalPort.Min, totalPort.Max, totalPort.Avg);

                Log.RamUsage($"{purpose} LANGFLOW_NAME", langflowName.Min, langflowName.Max, langflowName.Avg);
                
                Log.RamUsage($"{purpose} OLLAMA", ollamaName.Min, ollamaName.Max, ollamaName.Avg);

                Log.End();
            }
        }
    }
}
