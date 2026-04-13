using OllamaSharp;

namespace ollama_langflow
{
    public class Conversation
    {
        private readonly OllamaApiClient _ollama;
        private readonly List<string> _memory = [];

        public Conversation()
        {
            var uri = new Uri("http://localhost:11434");
            _ollama = new OllamaApiClient(uri)
            {
                SelectedModel = "llama3.2:3b"
            };
        }

        public async Task RunQuestion(string question)
        {
            Log.StringLog("QUESTION", question + "\n\n");

            Log.startTotal = DateTime.Now;
            RamTracker.Start();

            // CONTEXT
            var context = await LangflowClient._langflowClient.QueryLangflowAsync(question);
            Log.StringLog("CONTEXT", context + "\n\n");
            Log.doneContext = DateTime.Now;

            // PROMPT
            string prompt;

            if (Consts.HISTORY)
            {
                while (_memory.Count > 2)
                    _memory.RemoveAt(0);

                Log.StringLog("HISTORY", string.Join(" ", _memory));

                prompt =
                    $"Answer the following question based on the provided information:\n" +
                    $"Question: {question}\n" +
                    $"Chat History: {_memory}\n" +
                    $"Information: {context}\n";
            }
            else
            {
                prompt =
                    $"Answer the following question based on the provided information:\n" +
                    $"Question: {question}\n" +
                    $"Information: {context}\n";
            }

            var chat = new Chat(_ollama);

            string answer = "";
            string newMemory = $"User: {question}\nAssistant: ";
            bool isFirst = true;

            await foreach (var token in chat.SendAsync(prompt))
            {
                if (isFirst)
                {
                    Log.startStream = DateTime.Now;
                    isFirst = false;
                }

                answer += token;
                newMemory += token;
            }

            if (Consts.HISTORY)
            {
                _memory.Add(newMemory);
            }

            Log.StringLog("ANSWER", answer + "\n\n");

            Log.doneTotal = DateTime.Now;

            Log.StringLog("| TIMINGS", "");
            Log.TimeLog(Log.startTotal, Log.doneContext, "CONTEXT");
            Log.TimeLog(Log.doneContext, Log.startStream, "STREAM START");
            Log.TimeLog(Log.startStream, Log.doneTotal, "(GENERATION)");
            Log.TimeLog(Log.startTotal, Log.doneTotal, "TOTAL");

            RamTracker.StopAndLog("RAM USAGE");
        }
    }
}