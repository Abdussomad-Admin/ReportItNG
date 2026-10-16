namespace ReportItNG.Services.External.Gemma
{
    public sealed class GemmaOptions
    {
        public const string SectionName = "Gemma";

        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemma-4-31b-it";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta/";
        public int TimeoutSeconds { get; set; } = 60;
    }
}