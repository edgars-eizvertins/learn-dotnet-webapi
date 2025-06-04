using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuoteApi.Services;

public class GeminiService
{
    private readonly string apiKey;
    private readonly string modelUrl;

    public GeminiService(string apiKey)
    {
        this.apiKey = apiKey;
		this.modelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={this.apiKey}";
    }

    public async Task<GeminiResult> GenerateQuoteAsync()
    {
        var prompt = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = "Generate a short inspirational quote. Think widely so answer does not repeat" }
                    }
                }
            }
        };

        using var http = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json");
        var response = await http.PostAsync(modelUrl, content);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Try to extract error message from Gemini response
            try
            {
                using var doc = JsonDocument.Parse(json);
                var error = doc.RootElement.GetProperty("error").GetProperty("message").GetString();
                return new GeminiResult { Error = error ?? "Unknown Gemini API error." };
            }
            catch
            {
                return new GeminiResult { Error = $"HTTP {response.StatusCode}: {json}" };
            }
        }

        using (var doc = JsonDocument.Parse(json))
        {
            var quote = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return new GeminiResult { Quote = quote };
        }
    }
}