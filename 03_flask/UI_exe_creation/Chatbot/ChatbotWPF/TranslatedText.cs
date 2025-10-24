using Newtonsoft.Json;

namespace ChatbotWPF
{
    public class TranslatedText
    {
        [JsonProperty("context")]
        public string? Context { get; set; }
    }
}
