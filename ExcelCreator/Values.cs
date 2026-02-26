namespace ExcelCreator
{
    public class Values
    {
        // --- identification ---
        public string filePath { get; set; }
        public int questionNo { get; set; }
        public string language { get; set; }

        // --- quality ---
        public int? accuracy { get; set; }   // only in some Flask runs

        // --- tokens ---
        public int tokens { get; set; }       // only when answer exists

        // --- timings ---
        public Timings context { get; set; }      // always when present
        public Timings? stream { get; set; }      // OllamaSharp only
        public Timings? toEn { get; set; }         // Flask / Langflow
        public Timings? toDe { get; set; }         // Flask / Langflow

        // unified semantic meaning:
        // GENERATION (ollamaSharp)
        // ANSWER EN (Flask / Langflow)
        public Timings? generation { get; set; }

        public Timings total { get; set; }

        // --- RAM usage ---
        public RamUsage ramTotal { get; set; }
        public RamUsage ramCSharp { get; set; }

        // unified semantic meaning:
        // RAM USAGE FLASK
        // RAM USAGE LANGFLOW
        // (Python process)
        public RamUsage ramPython { get; set; }

        public RamUsage? ramOllama { get; set; }   // only OllamaSharp

        public Values()
        {
            filePath = "";
            language = "";

            tokens = 0;
            accuracy = null;

            context = new Timings();
            stream = null;
            toEn = null;
            toDe = null;
            generation = null;
            total = new Timings();

            ramTotal = new RamUsage();
            ramCSharp = new RamUsage();
            ramPython = new RamUsage();
            ramOllama = null;
        }
    }
}
