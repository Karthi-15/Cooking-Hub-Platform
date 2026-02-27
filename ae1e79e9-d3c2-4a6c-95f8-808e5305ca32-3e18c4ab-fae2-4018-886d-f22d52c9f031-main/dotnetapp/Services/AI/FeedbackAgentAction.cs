using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using dotnetapp.Data;

public class FeedbackAgentAction
{
    private readonly FeedbackTool _feedbackTool;
    private readonly ClassTool _classTool;
    private readonly HttpClient _http;
    private readonly ApplicationDbContext _context;
    private readonly string _apiKey;

    public FeedbackAgentAction(
        FeedbackTool feedbackTool,
        ClassTool classTool,
        IConfiguration configuration)
    {
        _feedbackTool = feedbackTool;
        _classTool = classTool;
        _http = new HttpClient();

        _apiKey = configuration["Gemini:ApiKey"];
    }

    public async Task<string> GenerateActionPlan()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "AI API key missing";
        var feedbacks = _feedbackTool.GetAllFeedbackTexts() ?? new List<string>();
        var classes = _classTool.GetAllClassNames() ?? new List<string>();

        var feedbackText = string.Join("\n", feedbacks);
        var classText = string.Join(", ", classes);

        var prompt = $@"
You are an AI admin assistant for a cooking class platform.

Analyze the provided feedback and existing classes.

Return SHORT and structured output in this exact format:

Sentiment:
(one line summary)

Top Issues:
- bullet 1
- bullet 2
- bullet 3

Recommended Actions:
- bullet 1
- bullet 2
- bullet 3

New Class Ideas:
- bullet 1
- bullet 2
- bullet 3

Keep it concise and business-focused.

Existing Classes:
{classText}

Feedback:
{feedbackText}
";

        var body = new
        {
            contents = new[]
            {
                new {
                    parts = new[] {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonConvert.SerializeObject(body);

        var url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("x-goog-api-key", _apiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(request);
        var response = await res.Content.ReadAsStringAsync();

        if (!res.IsSuccessStatusCode)
            return $" API Error:\n{response}";

        try
        {
            dynamic data = JsonConvert.DeserializeObject(response);

            var text = data?.candidates?[0]?.content?.parts?[0]?.text;

            if (string.IsNullOrWhiteSpace((string?)text))
                return "Returned empty response.";
            return (string)text;
        }
        catch (Exception ex)
        {
            return $" AI parsing failed:\n{ex.Message}\nRaw:\n{response}";
        }
    }
}
