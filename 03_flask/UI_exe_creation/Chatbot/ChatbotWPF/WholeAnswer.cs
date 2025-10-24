using Newtonsoft.Json;

namespace ChatbotWPF
{
    internal class WholeAnswer
    {
        
        [JsonProperty("answer")]
        public string? Answer { get; set; }
    
}
}
