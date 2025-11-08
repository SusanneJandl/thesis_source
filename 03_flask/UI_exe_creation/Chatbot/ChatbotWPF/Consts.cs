namespace ChatbotWPF
{
    internal class Consts
    {
        public static readonly Uri URI = new("http://127.0.0.1:11434");
        public static readonly Uri CONTENT = new("http://127.0.0.1:5000/content");
        public static readonly Uri ANSWER = new("http://127.0.0.1:5000/answer");
        public static readonly Uri TRANS_DE_EN = new("http://127.0.0.1:5000/de-en");
        public static readonly Uri TRANS_EN_DE = new("http://127.0.0.1:5000/en-de");
        public static string LANGUAGE = "DE"; // "DE" or "EN"
        public static bool HISTORY = true;
        public static string DEVICE = "laptop"; // "Latop" or "PC"
    }
}
