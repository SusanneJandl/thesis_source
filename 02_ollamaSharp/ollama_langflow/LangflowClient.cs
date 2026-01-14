using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class LangflowResponse
{
    public List<OutputWrapper> outputs { get; set; }
}

public class OutputWrapper
{
    public List<OutputItem> outputs { get; set; }
}

public class OutputItem
{
    public OutputResults results { get; set; }
}

public class OutputResults
{
    public OutputMessage message { get; set; }
}

public class OutputMessage
{
    public OutputData data { get; set; }
}

public class OutputData
{
    public string text { get; set; }
}

public class LangflowClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string LangflowUrl =
        "http://127.0.0.1:7888/api/v1/run/228f99f9-fbc1-469d-bbd9-efd9bef15c29";
    
    public static readonly LangflowClient _langflowClient = new LangflowClient();

    public async Task<string?> QueryLangflowAsync(string message)
    {
        var payload = new
        {
            input_value = message
        };

        var json = JsonConvert.SerializeObject(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(LangflowUrl, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            return ExtractText(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
    }

    private string? ExtractText(string json)
    {
        try
        {
            var parsed = JsonConvert.DeserializeObject<LangflowResponse>(json);

            // Vollständig null-sicher
            return parsed?
                .outputs?.Count > 0 ? parsed.outputs[0]?
                .outputs?.Count > 0 ? parsed.outputs[0].outputs[0]?
                .results?
                .message?
                .data?
                .text : null : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return null;
        }
    }

}
