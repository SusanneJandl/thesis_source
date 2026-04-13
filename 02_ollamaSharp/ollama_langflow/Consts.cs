namespace ollama_langflow
{
    public class Consts
    {
        public static readonly Uri UriOllama = new("http://127.0.0.1:11434");
        public static readonly Uri uriToken = new("http://127.0.0.1:5000/token");
        public static bool HISTORY = false;
        public static string DEVICE = "laptop"; // "Latop" or "PC"
    }
}
