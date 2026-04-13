using System;

namespace ExcelCreator
{
    public static class Parser
    {
        public static Values Parse(MdFormat format, string block, string file)
        {
            return format switch
            {
                MdFormat.Flask => ParseFlask(block, file),
                MdFormat.OllamaSharp => ParseOllama(block, file),
                MdFormat.NoGeneration => ParseNoGeneration(block, file),
                MdFormat.Widget => ParseWidget(block, file),
                MdFormat.Quantization => ParseQuantization(block, file),
                _ => throw new Exception("Unknown format")
            };
        }

        private static Values ParseFlask(string block, string file)
        {
            Values v = new Values();
            v.filePath = file;

            // --- model ---
            v.model = MdParser.ExtractModel(block);

            // --- question + language ---
            string question = MdParser.ExtractQuestionForResolution(block, MdFormat.Flask);
            MdParser.ResolveQuestion(question, out string qNo, out string lang);

            v.questionId = qNo;
            v.language = lang;

            // --- tokens ---
            v.tokens = MdParser.ExtractTokens(block);

            // --- timings ---
            v.context.value = MdParser.ExtractTiming(block, "CONTEXT");

            v.toEn = new Timings
            {
                value = MdParser.ExtractTiming(block, "TO EN")
            };

            v.toDe = new Timings
            {
                value = MdParser.ExtractTiming(block, "TO DE")
            };

            v.generation = new Timings
            {
                value = MdParser.ExtractTiming(block, "ANSWER EN")
            };

            v.total.value = MdParser.ExtractTiming(block, "TOTAL");

            // --- RAM ---
            v.ramTotal = MdParser.ExtractRamUsage(block, "RAM USAGE TOTAL");
            v.ramCSharp = MdParser.ExtractRamUsage(block, "C#");
            v.ramPython = MdParser.ExtractRamUsage(block, "FLASK");
            v.ramOllama = MdParser.ExtractRamUsage(block, "OLLAMA");

            return v;
        }
        private static Values ParseOllama(string block, string file)
        {
            Values v = new Values();
            v.filePath = file;

            // --- question ---
            string question = MdParser.ExtractQuestion(block);
            MdParser.ResolveQuestion(question, out string qNo, out string lang);

            v.questionId = qNo;
            v.language = lang;

            // --- tokens ---
            v.tokens = MdParser.ExtractTokens(block);

            // --- timings ---
            v.context.value = MdParser.ExtractTiming(block, "CONTEXT");

            v.stream = new Timings
            {
                value = MdParser.ExtractTiming(block, "STREAM START")
            };

            v.generation = new Timings
            {
                value = MdParser.ExtractTiming(block, "GENERATION")
            };

            v.total.value = MdParser.ExtractTiming(block, "TOTAL");

            // --- RAM ---
            v.ramTotal = MdParser.ExtractRamUsage(block, "RAM USAGE TOTAL");
            v.ramCSharp = MdParser.ExtractRamUsage(block, "RAM USAGE C#");
            v.ramPython = MdParser.ExtractRamUsage(block, "RAM USAGE LANGFLOW");
            v.ramOllama = MdParser.ExtractRamUsage(block, "RAM USAGE OLLAMA");

            return v;
        }
        private static Values ParseNoGeneration(string block, string file)
        {
            Values v = new Values();
            v.filePath = file;

            // --- language (directly from file) ---
            var langMatch = System.Text.RegularExpressions.Regex.Match(block, @"LANGUAGE:\s*(.+)");
            if (langMatch.Success)
                v.language = langMatch.Groups[1].Value.Trim();

            // --- question ---
            string question = MdParser.ExtractQuestionForResolution(block, MdFormat.NoGeneration);
            MdParser.ResolveQuestion(question, out string qNo, out string langResolved);

            v.questionId = qNo;

            // fallback if language missing
            if (string.IsNullOrEmpty(v.language))
                v.language = langResolved;

            // --- accuracy ---
            v.accuracy = MdParser.ExtractAccuracy(block);

            // --- tokens ---
            v.tokens = 0;

            // --- timings ---
            v.toEn = new Timings
            {
                value = MdParser.ExtractTiming(block, "TO EN")
            };

            v.generation = new Timings
            {
                value = MdParser.ExtractTiming(block, "ANSWER")
            };

            v.total.value = MdParser.ExtractTiming(block, "TOTAL");

            // --- RAM ---
            v.ramTotal = MdParser.ExtractRamUsage(block, "RAM USAGE TOTAL");
            v.ramCSharp = MdParser.ExtractRamUsage(block, "RAM USAGE C#");
            v.ramPython = MdParser.ExtractRamUsage(block, "RAM USAGE FLASK");

            return v;
        }
        private static Values ParseWidget(string block, string file)
        {
            Values v = new Values();
            v.filePath = file;

            // --- question number ---
            v.questionId = MdParser.ExtractQuestionNumber(block);

            // --- tokens ---
            v.tokens = MdParser.ExtractTokens(block);

            // --- timing (Duration instead of TOTAL) ---
            v.total.value = MdParser.ExtractTiming(block, "Duration");

            // --- RAM (different labels!) ---
            v.ramTotal = MdParser.ExtractRamUsage(block, "TOTAL");
            v.ramPython = MdParser.ExtractRamUsage(block, "LANGFLOW");
            v.ramOllama = MdParser.ExtractRamUsage(block, "OLLAMA");

            return v;
        }
        private static Values ParseQuantization(string block, string file)
        {
            Values v = new Values();
            v.filePath = file;

            // --- model ---
            var modelMatch = System.Text.RegularExpressions.Regex.Match(block, @"MODEL:\s*(.+)");
            if (modelMatch.Success)
                v.model = modelMatch.Groups[1].Value.Trim();

            // --- language ---
            var langMatch = System.Text.RegularExpressions.Regex.Match(block, @"LANGUAGE:\s*(.+)");
            if (langMatch.Success)
                v.language = langMatch.Groups[1].Value.Trim();

            // --- question ---
            string question = MdParser.ExtractQuestion(block);
            MdParser.ResolveQuestion(question, out string qNo, out string langResolved);
            v.questionId = qNo;

            // fallback if language missing
            if (string.IsNullOrEmpty(v.language))
                v.language = langResolved;

            // --- tokens ---
            v.tokens = MdParser.ExtractTokens(block);

            // --- timings ---
            v.context.value = MdParser.ExtractTiming(block, "CONTEXT");
            v.generation = new Timings
            {
                value = MdParser.ExtractTiming(block, "ANSWER")
            };
            v.total.value = MdParser.ExtractTiming(block, "TOTAL");

            // --- RAM ---
            v.ramTotal = MdParser.ExtractRamUsage(block, "TOTAL RAM");
            v.ramPython = MdParser.ExtractRamUsage(block, "PYTHON RAM");
            v.ramOllama = MdParser.ExtractRamUsage(block, "OLLAMA RAM");

            return v;
        }
    }
}