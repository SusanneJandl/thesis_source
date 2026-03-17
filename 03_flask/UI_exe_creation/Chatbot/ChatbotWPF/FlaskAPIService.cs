using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ChatbotWPF
{
    internal class FlaskAPIService
    {
        public static async Task<string> RetrieveAnswerAsync(string query, List<string> history)
        {
            string answer = "";

            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(9000);

                var request = new HttpRequestMessage(HttpMethod.Post, Consts.ANSWER);

                var jsonBody = JsonConvert.SerializeObject(new
                {
                    question = query,
                    topic = "Fantasy",
                    language = Consts.LANGUAGE,
                    history
                });
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Send the request
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Read the response
                string text = await response.Content.ReadAsStringAsync();
                WholeAnswer wholeAnswer = JsonConvert.DeserializeObject<WholeAnswer>(text)!;
                Console.WriteLine(wholeAnswer.Answer);
                answer = wholeAnswer.Answer!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                answer = "Something went wrong. ";
            }

            return answer;
        }
    }
}
