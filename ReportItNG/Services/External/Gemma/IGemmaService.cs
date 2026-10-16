namespace ReportItNG.Services.External.Gemma
{
    public interface IGemmaService
    {
        Task<string> GenerateAsync(
            string prompt,
            GemmaRequestOptions? options = null,
            CancellationToken cancellationToken = default);

        Task<string> ChatAsync(
            IReadOnlyList<GemmaMessage> history,
            GemmaRequestOptions? options = null,
            CancellationToken cancellationToken = default);
    }

    public sealed class GemmaApiException : Exception
    {
        public int? StatusCode { get; }

        public GemmaApiException(string message, int? statusCode = null, Exception? inner = null)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}