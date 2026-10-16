using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ReportItNG.Services.External.Gemma
{
    public sealed class GemmaService : IGemmaService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private readonly GemmaOptions _options;
        private readonly ILogger<GemmaService> _logger;

        public GemmaService(
            HttpClient http,
            IOptions<GemmaOptions> options,
            ILogger<GemmaService> logger)
        {
            _http = http;
            _options = options.Value;
            _logger = logger;
        }

        public Task<string> GenerateAsync(
            string prompt,
            GemmaRequestOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var history = new List<GemmaMessage>
            {
                new() { Role = "user", Text = prompt }
            };
            return ChatAsync(history, options, cancellationToken);
        }

        public async Task<string> ChatAsync(
            IReadOnlyList<GemmaMessage> history,
            GemmaRequestOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (history.Count == 0)
                throw new ArgumentException("History must contain at least one message.", nameof(history));

            var model = options?.Model ?? _options.Model;

            var body = new GenerateContentRequest
            {
                Contents = history.Select(m => new Content
                {
                    Role = m.Role,
                    Parts = new List<Part> { new() { Text = m.Text } }
                }).ToList()
            };

            if (!string.IsNullOrWhiteSpace(options?.SystemInstruction))
            {
                body.SystemInstruction = new Content
                {
                    Parts = new List<Part> { new() { Text = options!.SystemInstruction } }
                };
            }

            if (options is not null &&
                (options.Temperature.HasValue ||
                 options.MaxOutputTokens.HasValue ||
                 !string.IsNullOrWhiteSpace(options.ThinkingLevel)))
            {
                body.GenerationConfig = new GenerationConfig
                {
                    Temperature = options.Temperature,
                    MaxOutputTokens = options.MaxOutputTokens,
                    ThinkingConfig = string.IsNullOrWhiteSpace(options.ThinkingLevel)
                        ? null
                        : new ThinkingConfig { ThinkingLevel = options.ThinkingLevel! }
                };
            }

            var url = $"models/{model}:generateContent";

            HttpResponseMessage response;
            try
            {
                response = await _http.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new GemmaApiException("The Gemma request timed out.", null, ex);
            }
            catch (HttpRequestException ex)
            {
                throw new GemmaApiException("Could not reach the Gemma API.", null, ex);
            }

            using (response)
            {
                var raw = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemma API error {Status}: {Body}", (int)response.StatusCode, raw);
                    throw new GemmaApiException(
                        $"Gemma API returned {(int)response.StatusCode}: {raw}",
                        (int)response.StatusCode);
                }

                var parsed = JsonSerializer.Deserialize<GenerateContentResponse>(raw, JsonOptions);

                if (parsed?.PromptFeedback?.BlockReason is { } blocked)
                    throw new GemmaApiException($"The prompt was blocked: {blocked}");

                var parts = parsed?.Candidates?.FirstOrDefault()?.Content?.Parts;
                if (parts is null || parts.Count == 0)
                    throw new GemmaApiException("The Gemma API returned no content.");

                var sb = new StringBuilder();
                foreach (var part in parts)
                {
                    // Skip reasoning parts, keep only the final answer text
                    if (part.Thought == true) continue;
                    if (!string.IsNullOrEmpty(part.Text)) sb.Append(part.Text);
                }

                return sb.ToString();
            }
        }
    }
}