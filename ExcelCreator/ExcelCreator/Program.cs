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

        static MdFormat GetFormatFromFile()
        {
            if (Consts.MdPath.Contains("03_flask")) return MdFormat.Flask;
            if (Consts.MdPath.Contains("02_ollamaSharp")) return MdFormat.OllamaSharp;
            if (Consts.MdPath.Contains("04_no_generation")) return MdFormat.NoGeneration;
            if (Consts.MdPath.Contains("01_widget")) return MdFormat.Widget;
            if (Consts.MdPath.Contains("05_quantization")) return MdFormat.Quantization;

            throw new Exception("Unknown format");
        }

        static void Main(string[] args)
        {            
            string[] files = Directory.GetFiles(Consts.MdPath, "*.md");
            List<Values> results = new List<Values>();

            foreach (string file in files)
            {
                MdFormat format = GetFormatFromFile();
                if (format == MdFormat.Widget || format == MdFormat.OllamaSharp)
                {
                    MdParser.AddTokensToFile(file).Wait();
                }

                string mdText = File.ReadAllText(file);
                string[] blocks = MdParser.SplitIntoBlocks(mdText);

                foreach (string block in blocks)
                {
                    if (string.IsNullOrWhiteSpace(block))
                        continue;
                    
                    Values v = Parser.Parse(format, block, GetShortPath(file));
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
