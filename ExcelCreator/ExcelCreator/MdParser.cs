using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
              + @"=?\s*"        // optional '='
              + @"([\d,\.]+)"; // the number

            Match m = Regex.Match(text, pattern);

            if (m.Success)
            {
                string value = m.Groups[1].Value.Replace(',', '.');
                return double.Parse(value, CultureInfo.InvariantCulture);
            }

            return 0.0;
        }

        public static double ExtractDuration(string block)
        {
            Match m = Regex.Match(
                block,
                @"Duration:\s*([\d,\.]+)\s*s"
            );

            if (m.Success)
            {
                string value = m.Groups[1].Value.Replace(',', '.');
                return double.Parse(value, CultureInfo.InvariantCulture);
            }

            return 0.0;
        }

        //public static RamUsage ExtractRamUsage(string text, string section)
        //{
        //    RamUsage r = new RamUsage();

        //    string pattern =
        //        @"RAM USAGE\s+"
        //        + section
        //        + @":\s*MIN=(\d+)\s*MB\s*\|\s*MAX=(\d+)\s*MB\s*\|\s*AVG=(\d+)\s*MB";

        //    Match m = Regex.Match(text, pattern);

        //    if (m.Success)
        //    {
        //        r.min = int.Parse(m.Groups[1].Value);
        //        r.max = int.Parse(m.Groups[2].Value);
        //        r.avg = int.Parse(m.Groups[3].Value);
        //    }

        //    return r;
        //}
        public static RamUsage ExtractRamUsage(string text, string section)
        {
            RamUsage r = new RamUsage();

            string pattern =
                section +
                @":\s*MIN=(\d+)\s*MB\s*\|\s*MAX=(\d+)\s*MB\s*\|\s*AVG=(\d+)\s*MB";

            Match m = Regex.Match(text, pattern);

            if (m.Success)
            {
                r.min = int.Parse(m.Groups[1].Value);
                r.max = int.Parse(m.Groups[2].Value);
                r.avg = int.Parse(m.Groups[3].Value);
            }

            return r;
        }
        public static string ExtractQuestionNumber(string block)
        {
            Match m = Regex.Match(block, @"#\s*(\d+)");

            if (m.Success)
                return m.Groups[1].Value;

            return "";
        }

        public static void ResolveQuestion(string question, out string questionNo, out string language)
        {
            questionNo = "";
            language = "UNKNOWN";
            string questionText = Normalize(question);

            foreach (var kv in Questions.EN)
            {
                if (questionText.Equals(Normalize(kv.Value)))
                {
                    questionNo = kv.Key;
                    language = "EN";
                    return;
                }
            }

            foreach (var kv in Questions.DE)
            {
                if (Normalize(kv.Value) == questionText)
                {
                    questionNo = kv.Key;
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

        public static string ExtractModel(string mdText)
        {
            Match m = Regex.Match(mdText, @"MODEL\s*=\s*(.+)");

            if (m.Success)
                return m.Groups[1].Value.Trim();

            return "";
        }

        private static string Normalize(string text)
        {
            return text
                .Normalize(NormalizationForm.FormC)
                .Replace("„", "\"")
                .Replace("“", "\"")
                .Replace("’", "'")
                .Replace("‘", "'")
                .Replace("–", "-")
                .Replace("—", "-")
                .Replace("  ", " ")
                .ToLowerInvariant()
                .Trim();
        }


        private static async Task<string> GetTokensAsync(string answer)
        {
            string token = "0";

            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(9000);

                var request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://localhost:5000/token"));

                var jsonBody = JsonConvert.SerializeObject(new
                {
                    answer = answer
                });
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the request
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Read the response
                string text = await response.Content.ReadAsStringAsync();
                JsonResponse wholeAnswer = JsonConvert.DeserializeObject<JsonResponse>(text)!;
                token = wholeAnswer.token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                answer = "Something went wrong. ";
            }

            return token;
        }
        private static async Task<string> AddTokensToBlock(string block)
        {
            var match = Regex.Match(
                block,
                @"(ANSWER:\s*)(.*?)(\n\s*\|)",
                RegexOptions.Singleline);

            if (!match.Success)
                return block;

            var prefix = match.Groups[1].Value;
            var answer = match.Groups[2].Value.Trim();
            var suffix = match.Groups[3].Value;

            if (string.IsNullOrWhiteSpace(answer))
                return block;

            string tokens = await GetTokensAsync(answer);

            var newAnswer = $"{answer} {tokens}\n";

            return block.Replace(match.Value, prefix + newAnswer + suffix);
        }

        public static async Task AddTokensToFile(string path)
        {
            var content = File.ReadAllText(path, Encoding.UTF8);

            var blocks = content.Split("================================================================");

            var updatedBlocks = new List<string>();

            foreach (var block in blocks)
            {
                var updated = await AddTokensToBlock(block);
                updatedBlocks.Add(updated);
            }

            var result = string.Join("================================================================", updatedBlocks);

            File.WriteAllText(path, result, Encoding.UTF8);
        }
    }
}