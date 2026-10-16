using System.Text.Json.Serialization;

namespace ReportItNG.Services.External.Gemma
{
    public sealed class GemmaMessage
    {
        // "user" or "model"
        public string Role { get; set; } = "user";
        public string Text { get; set; } = string.Empty;
    }

    public sealed class GemmaRequestOptions
    {
        public string? SystemInstruction { get; set; }
        public double? Temperature { get; set; }
        public int? MaxOutputTokens { get; set; }

        // "high" turns thinking on, "minimal" turns it off
        public string? ThinkingLevel { get; set; }

        // Override the default model per call (gemma-4-31b-it or gemma-4-26b-a4b-it)
        public string? Model { get; set; }
    }

    // ---- Wire DTOs (match the Gemini REST JSON) ----

    internal sealed class GenerateContentRequest
    {
        [JsonPropertyName("contents")]
        public List<Content> Contents { get; set; } = new();

        [JsonPropertyName("systemInstruction")]
        public Content? SystemInstruction { get; set; }

        [JsonPropertyName("generationConfig")]
        public GenerationConfig? GenerationConfig { get; set; }
    }

    internal sealed class Content
    {
        [JsonPropertyName("role")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; }

        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();
    }

    internal sealed class Part
    {
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        // True on parts that contain the model's reasoning, not the final answer
        [JsonPropertyName("thought")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Thought { get; set; }
    }

    internal sealed class GenerationConfig
    {
        [JsonPropertyName("temperature")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Temperature { get; set; }

        [JsonPropertyName("maxOutputTokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxOutputTokens { get; set; }

        [JsonPropertyName("thinkingConfig")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ThinkingConfig? ThinkingConfig { get; set; }
    }

    internal sealed class ThinkingConfig
    {
        [JsonPropertyName("thinkingLevel")]
        public string ThinkingLevel { get; set; } = "minimal";
    }

    internal sealed class GenerateContentResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; set; }

        [JsonPropertyName("promptFeedback")]
        public PromptFeedback? PromptFeedback { get; set; }
    }

    internal sealed class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }
    }

    internal sealed class PromptFeedback
    {
        [JsonPropertyName("blockReason")]
        public string? BlockReason { get; set; }
    }
}