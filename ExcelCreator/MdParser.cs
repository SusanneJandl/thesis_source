using System.Globalization;
using System.Text.RegularExpressions;

namespace ExcelCreator
{
    public static class MdParser
    {
        public static int ExtractTokens(string text)
        {
            Match m = Regex.Match(text, @"\((\d+)\s*T\)");
            if (m.Success)
            {
                return int.Parse(m.Groups[1].Value);
            }

            return 0;
        }

        public static double ExtractTiming(string text, string label)
        {
            string pattern =
                @"\(?\s*"        // optional '('
              + label
              + @"\s*\)?\s*"    // optional ')'
              + @":?\s*"        // optional ':'
              + @"([\d,\.]+)"; // the number

            Match m = Regex.Match(text, pattern);

            if (m.Success)
            {
                string value = m.Groups[1].Value.Replace(',', '.');
                return double.Parse(value, CultureInfo.InvariantCulture);
            }

            return 0.0;
        }

        public static RamUsage ExtractRamUsage(string text, string section)
        {
            RamUsage r = new RamUsage();

            string pattern =
                @"RAM USAGE\s+"
                + section
                + @":\s*MIN=(\d+)\s*MB\s*\|\s*MAX=(\d+)\s*MB\s*\|\s*AVG=(\d+)\s*MB";

            Match m = Regex.Match(text, pattern);

            if (m.Success)
            {
                r.min = int.Parse(m.Groups[1].Value);
                r.max = int.Parse(m.Groups[2].Value);
                r.avg = int.Parse(m.Groups[3].Value);
            }

            return r;
        }

        public static void ResolveQuestion(string questionText, out int questionNo, out string language)
        {
            questionNo = 0;
            language = "UNKNOWN";

            for (int i = 1; i < Questions.EN.Length; i++)
            {
                if (Questions.EN[i] == questionText)
                {
                    questionNo = i;
                    language = "EN";
                    return;
                }
            }

            for (int i = 1; i < Questions.DE.Length; i++)
            {
                if (Questions.DE[i] == questionText)
                {
                    questionNo = i;
                    language = "DE";
                    return;
                }
            }
        }

        public static string ExtractQuestion(string text)
        {
            Match m = Regex.Match(
                text,
                @"QUESTION:\s*(.*)",
                RegexOptions.IgnoreCase
            );

            if (m.Success)
                return m.Groups[1].Value.Trim();

            return "";
        }

        public static string[] SplitIntoBlocks(string mdText)
        {
            return mdText.Split(
                new string[] { "================================================================" },
                StringSplitOptions.RemoveEmptyEntries
            );
        }

        public static int? ExtractAccuracy(string block)
        {
            Match m = Regex.Match(block, @"ACCURACY:\s*(\d+)");
            if (m.Success)
                return int.Parse(m.Groups[1].Value);

            return null;
        }

        public static bool HasQuestion(string block, MdFormat format)
        {
            switch (format)
            {
                case MdFormat.OllamaSharp:
                    return block.Contains("QUESTION:");

                case MdFormat.Flask:
                case MdFormat.NoGeneration:
                    return block.Contains("GERMAN QUESTION:")
                        || block.Contains("ENGLISH QUESTION:");

                default:
                    return false;
            }
        }

        public static string ExtractQuestionForResolution(string block, MdFormat format)
        {
            switch (format)
            {
                case MdFormat.OllamaSharp:
                    return ExtractQuestionOllamaSharp(block);

                case MdFormat.Flask:
                case MdFormat.NoGeneration:
                    // Prefer German
                    string de = ExtractQuestionFlask(block, "GERMAN QUESTION:");
                    if (!string.IsNullOrEmpty(de))
                        return de;

                    return ExtractQuestionFlask(block, "ENGLISH QUESTION:");

                default:
                    return "";
            }
        }

        public static string ExtractQuestionForTokens(string block, MdFormat format)
        {
            switch (format)
            {
                case MdFormat.OllamaSharp:
                    // old format: tokens are in ANSWER anyway
                    return "";

                case MdFormat.Flask:
                case MdFormat.NoGeneration:
                    return ExtractQuestionFlask(block, "ENGLISH QUESTION:");

                default:
                    return "";
            }
        }

        private static string ExtractQuestionOllamaSharp(string block)
        {
            Match m = Regex.Match(
                block,
                @"QUESTION:\s*(.+)",
                RegexOptions.IgnoreCase
            );

            return m.Success ? m.Groups[1].Value.Trim() : "";
        }

        private static string ExtractQuestionFlask(string block, string label)
        {
            Match m = Regex.Match(
                block,
                label + @"\s*(.+?)(\r?\n\r?\n)",
                RegexOptions.Singleline
            );

            return m.Success ? m.Groups[1].Value.Trim() : "";
        }

    }
}