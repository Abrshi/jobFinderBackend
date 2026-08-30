using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace jobFinderBackend.Infrastructure.AI.Gemini;

public sealed class GeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateJsonAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "Gemini:ApiKey is not configured.");
        }

        /*
         * Gemini model.
         *
         * Your previous model:
         * gemini-2.5-flash
         *
         * was returning HTTP 404 for your account.
         *
         * The API error specifically instructed you
         * to use gemini-3.6-flash.
         */
        var model = "gemini-3.6-flash";

        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine("                    GEMINI API REQUEST");
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine($"Model: {model}");
        Console.WriteLine("Sending request to Gemini...");
        Console.WriteLine("------------------------------------------------------------");

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"v1beta/models/{model}:generateContent");

        request.Headers.Add(
            "x-goog-api-key",
            apiKey);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        var responseBody =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        /*
         * Handle HTTP errors.
         */
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine();
            Console.WriteLine("!!!!!!!!!!!!!!!! GEMINI API ERROR !!!!!!!!!!!!!!!!");
            Console.WriteLine(
                $"HTTP Status: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"Response: {responseBody}");
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            throw new HttpRequestException(
                $"Gemini API failed: " +
                $"{(int)response.StatusCode} " +
                $"{response.StatusCode}. " +
                $"Response: {responseBody}");
        }

        /*
         * Parse Gemini response.
         */
        try
        {
            using var document =
                JsonDocument.Parse(responseBody);

            var root = document.RootElement;

            /*
             * Make sure candidates exist.
             */
            if (!root.TryGetProperty(
                    "candidates",
                    out var candidates))
            {
                throw new InvalidOperationException(
                    $"Gemini response does not contain 'candidates'. " +
                    $"Response: {responseBody}");
            }

            if (candidates.GetArrayLength() == 0)
            {
                throw new InvalidOperationException(
                    $"Gemini returned zero candidates. " +
                    $"Response: {responseBody}");
            }

            var candidate = candidates[0];

            /*
             * Check content.
             */
            if (!candidate.TryGetProperty(
                    "content",
                    out var content))
            {
                throw new InvalidOperationException(
                    $"Gemini candidate does not contain 'content'. " +
                    $"Response: {responseBody}");
            }

            /*
             * Check parts.
             */
            if (!content.TryGetProperty(
                    "parts",
                    out var parts))
            {
                throw new InvalidOperationException(
                    $"Gemini content does not contain 'parts'. " +
                    $"Response: {responseBody}");
            }

            if (parts.GetArrayLength() == 0)
            {
                throw new InvalidOperationException(
                    $"Gemini returned zero parts. " +
                    $"Response: {responseBody}");
            }

            /*
             * Extract generated text.
             */
            var textElement = parts[0];

            if (!textElement.TryGetProperty(
                    "text",
                    out var textProperty))
            {
                throw new InvalidOperationException(
                    $"Gemini part does not contain 'text'. " +
                    $"Response: {responseBody}");
            }

            var text = textProperty.GetString();

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException(
                    "Gemini returned an empty response.");
            }

            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine("                    GEMINI API SUCCESS");
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"Model: {model}");
            Console.WriteLine("Gemini returned a response successfully.");
            Console.WriteLine("------------------------------------------------------------");

            return text.Trim();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Could not parse Gemini response as JSON. " +
                $"Response: {responseBody}",
                ex);
        }
    }
}