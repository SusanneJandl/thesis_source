using System.Text;

public class LangflowResponse
{
    public List<OutputWrapper>? Outputs { get; set; }
}

public class OutputWrapper
{
    public List<OutputItem>? Outputs { get; set; }
}

public class OutputItem
{
    public OutputResults? Results { get; set; }
}

public class OutputResults
{
    public OutputMessage? Message { get; set; }
}

public class OutputMessage
{
    public OutputData? Data { get; set; }
}

public class OutputData
{
    public string? text { get; set; }
}

public class LangflowClient
{
    private static readonly HttpClient _httpClient = new HttpClient();
    //private const string LangflowUrl = "http://127.0.0.1:7888/api/v1/run/c1877972-c54e-4e59-a7b3-8a725124011d"; // PC
    private const string LangflowUrl = "http://127.0.0.1:7888/api/v1/run/228f99f9-fbc1-469d-bbd9-efd9bef15c29"; // laptop

    public static readonly LangflowClient _langflowClient = new();

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

            return parsed?
                .Outputs?.Count > 0 ? parsed.Outputs[0]?
                .Outputs?.Count > 0 ? parsed.Outputs?[0].Outputs?[0]?
                .Results?
                .Message?
                .Data?
                .text : null : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
            return null;
        }
    }

}
