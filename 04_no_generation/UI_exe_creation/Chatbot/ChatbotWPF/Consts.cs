namespace ChatbotWPF
{
    internal class Consts
    {
        public static readonly Uri ANSWER = new("http://127.0.0.1:5000/answer_qa");
        public static readonly Uri TRANS_DE_EN = new("http://127.0.0.1:5000/de-en");
        public static string LANGUAGE = "DE"; // "DE" or "EN"
        public static string DEVICE = "laptop"; // "Latop" or "PC"
        public static int ACCURACY = 3;
    }
}
