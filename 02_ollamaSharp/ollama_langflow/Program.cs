using OllamaSharp;
using ollama_langflow;

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
string newMemory;
ollama.SelectedModel = "llama3.2:3b";
Log.History();

var runner = new Conversation();

while (true)
{
    Console.WriteLine("\nGib eine Frage ein:");
    question = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(question))
        continue;

    await runner.RunQuestion(question);
}

