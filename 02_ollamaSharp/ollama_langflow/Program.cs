using ollama_langflow;
using OllamaSharp;

async Task<string> ContextAsync(string question)
{
    string? context = await LangflowClient._langflowClient.QueryLangflowAsync(question);

    return string.IsNullOrWhiteSpace(context)
        ? "Keine zusätzlichen Informationen verfügbar."
        : context;
}

string Context(string question)
{
    try
    {
        return ContextAsync(question).GetAwaiter().GetResult();
    }
    catch
    {
        return "Keine zusätzlichen Informationen verfügbar.";
    }
}

// Chat variables
List<string> memory = [];
string question = "";
var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);
string newMemory = "";
ollama.SelectedModel = "llama3.2:3b";
Log.History();

while (true)
{
    if (Consts.HISTORY)
    { 
        while (memory.Count > 2)
        {
            memory.RemoveAt(0);
        }
    }
    newMemory = "\n";

    
    // Chat with Ollama
    var chat = new Chat(ollama);
    Console.WriteLine("\nGib eine Frage ein: ");
    var input = Console.ReadLine();
    Log.StringLog("QUESTION", input ?? "none" + "\n\n");
    question = input ?? "none";

    Log.startTotal = DateTime.Now;
    RamTracker.Start();

    var context = $"{Context(question)}";
    Log.StringLog("CONTEXT", context + "\n\n");
    Log.doneContext = DateTime.Now;
    string prompt = "";
    if (Consts.HISTORY)
    {
        Log.StringLog("HISTORY", string.Join(" ", memory));
        prompt = $"Answer the following question based on the provided information:\n" +
                 $"Question: {question}\n" +
                 $"Chat History: {memory}\n" +
                 $"Information: {context}" +
                 $"Answer German or English depending on the question.\n";

    }
    else
    {
        prompt = $"Answer the following question based on the provided information:\n" +
                  $"Question: {question}\n" +
                  $"Information: {context}" +
                  $"Answer German or English depending on the question.\n";
    }
    newMemory = $"User: {question} \n Assistant: ";
    string answer = "";
    bool isFirst = true;
    try
    {
        await foreach (var answerToken in chat.SendAsync(prompt))
        {
            if (isFirst)
            {
                Log.startStream = DateTime.Now;
                isFirst = false;
            }
            Console.Write(answerToken);
            newMemory += answerToken;
            answer += answerToken;
        }
        if (Consts.HISTORY)
        {
            memory.Add(newMemory);
        }
        Log.StringLog("ANSWER", answer + "\n\n");
        Log.doneTotal = DateTime.Now;
        Log.StringLog("TIMINGS", "");
        Log.TimeLog(Log.startTotal, Log.doneContext, "CONTEXT");
        Log.TimeLog(Log.doneContext, Log.startStream, "STREAM START");
        Log.TimeLog(Log.doneContext, Log.doneTotal, "(GENERATION)");
        Log.TimeLog(Log.startTotal, Log.doneTotal, "TOTAL");
        
        RamTracker.StopAndLog("RAM USAGE");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in chat response: {ex.Message}");
        newMemory += "Error: Unable to retrieve response.\n";
    }
}
