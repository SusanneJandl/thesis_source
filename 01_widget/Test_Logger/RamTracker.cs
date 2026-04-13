using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Test_Logger
{
    internal static class RamTracker
    {
        private static readonly List<double> samplesTotalMb = new();
        private static readonly List<double> samplesLangflowMb = new();
        private static readonly List<double> samplesOllamaMb = new();
        
        private static readonly object _lock = new();
        private static System.Timers.Timer? timer;

        private const string LangflowNamePattern = "python";
        private const string OllamaNamePattern = "ollama";

        private const double IntervalMs = 100;

        public static void Start()
        {
            lock (_lock)
            {
                if (timer != null)
                    return;

                samplesTotalMb.Clear();
                samplesLangflowMb.Clear();
                samplesOllamaMb.Clear();
                
                timer = new System.Timers.Timer(IntervalMs)
                {
                    AutoReset = true
                };
                timer.Elapsed += Timer_Elapsed;
                timer.Start();
            }
        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                double langflowMb = GetProcessGroupMemoryMbByName(LangflowNamePattern);
                double ollamaMb = GetProcessGroupMemoryMbByName(OllamaNamePattern);
                double totalMb = langflowMb + ollamaMb;

                lock (_lock)
                {
                    samplesLangflowMb.Add(langflowMb);
                    samplesOllamaMb.Add(ollamaMb);

                    samplesTotalMb.Add(totalMb);
                }
            }
            catch{}
        }

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

        
        public static void StopAndLog()
        {
            List<double> totalSnapshot;
            List<double> langflowSnapshot;
            List<double> ollamaSnapshot;

            lock (_lock)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }

                totalSnapshot = new List<double>(samplesTotalMb);
                langflowSnapshot = new List<double>(samplesLangflowMb);
                ollamaSnapshot = new List<double>(samplesOllamaMb);

                samplesTotalMb.Clear();
                samplesLangflowMb.Clear();
                samplesOllamaMb.Clear();


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

                var total = Summarize(totalSnapshot);
                var langflow = Summarize(langflowSnapshot);
                var ollama = Summarize(ollamaSnapshot);

                Log.RamUsage($"TOTAL", total.Min, total.Max, total.Avg);

                Log.RamUsage($"LANGFLOW", langflow.Min, langflow.Max, langflow.Avg);
                
                Log.RamUsage($"OLLAMA", ollama.Min, ollama.Max, ollama.Avg);

                Log.End();
            }
        }
    }
}
