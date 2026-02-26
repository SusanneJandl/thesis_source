using System;
using System.IO;
using System.Collections.Generic;

namespace ExcelCreator
{
    public static class Program
    {
        static string GetShortPath(string fullPath)
        {
            FileInfo file = new FileInfo(fullPath);
            return file.Directory.Parent.Name + "\\" +
                   file.Directory.Name + "\\" +
                   file.Name;
        }

        static void Main(string[] args)
        {
            // 👇 choose once per run
            MdFormat format = MdFormat.OllamaSharp; // or New / FlaskNoGeneration

            string[] files = Directory.GetFiles(Consts.MdPath, "*.md");
            List<Values> results = new List<Values>();

            foreach (string file in files)
            {
                string mdText = File.ReadAllText(file);
                string[] blocks = MdParser.SplitIntoBlocks(mdText);

                foreach (string block in blocks)
                {
                    // skip empty / non-question blocks
                    if (!MdParser.HasQuestion(block, format))
                        continue;

                    Values v = new Values();
                    v.filePath = GetShortPath(file);

                    // --- question & language ---
                    string questionForResolution =
    MdParser.ExtractQuestionForResolution(block, format);

                    int questionNo;
                    string language;

                    MdParser.ResolveQuestion(
                        questionForResolution,
                        out questionNo,
                        out language
                    );

                    v.questionNo = questionNo;
                    v.language = language;


                    // tokens → from English answer (recommended)
                    v.tokens = MdParser.ExtractTokens(block);

                    // --- accuracy (only in some formats) ---
                    v.accuracy = MdParser.ExtractAccuracy(block);

                    // --- tokens (only if English answer exists) ---
                    v.tokens = MdParser.ExtractTokens(block);

                    // --- timings ---
                    v.context.value = MdParser.ExtractTiming(block, "CONTEXT");
                    v.total.value = MdParser.ExtractTiming(block, "TOTAL");

                    if (format == MdFormat.OllamaSharp)
                    {
                        v.stream = new Timings();
                        v.stream.value = MdParser.ExtractTiming(block, "STREAM START");

                        v.generation = new Timings();
                        v.generation.value = MdParser.ExtractTiming(block, "GENERATION");
                    }
                    else
                    {
                        v.toEn = new Timings();
                        v.toEn.value = MdParser.ExtractTiming(block, "TO EN");

                        v.toDe = new Timings();
                        v.toDe.value = MdParser.ExtractTiming(block, "TO DE");

                        v.generation = new Timings();
                        v.generation.value = MdParser.ExtractTiming(block, "ANSWER EN");
                    }

                    // --- RAM ---
                    v.ramTotal = MdParser.ExtractRamUsage(block, "TOTAL");
                    v.ramCSharp = MdParser.ExtractRamUsage(block, "C#");

                    if (format != MdFormat.OllamaSharp)
                        v.ramPython = MdParser.ExtractRamUsage(block, "FLASK");
                    else
                        v.ramPython = MdParser.ExtractRamUsage(block, "LANGFLOW");

                    if (format == MdFormat.OllamaSharp)
                        v.ramOllama = MdParser.ExtractRamUsage(block, "OLLAMA");

                    results.Add(v);
                }
            }

            string outputFile =
                @"C:\Users\susan\Documents\bachelor-thesis_data\results.xlsx";

            ExcelWriter.Write(outputFile, results);

            Console.WriteLine("Excel written to:");
            Console.WriteLine(outputFile);
        }
    }
}
