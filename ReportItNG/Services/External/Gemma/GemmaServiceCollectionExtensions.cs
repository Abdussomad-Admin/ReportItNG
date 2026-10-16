using Microsoft.Extensions.Options;
using ReportItNG.Services.External.Gemma;

namespace YourApp.Services.Gemma;

public static class GemmaServiceCollectionExtensions
{
    public static IServiceCollection AddGemma(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GemmaOptions>(configuration.GetSection(GemmaOptions.SectionName));

        services.AddHttpClient<IGemmaService, GemmaService>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<GemmaOptions>>().Value;

            if (string.IsNullOrWhiteSpace(opts.ApiKey))
                throw new InvalidOperationException(
                    "Gemma:ApiKey is not configured. Set it with user-secrets or an environment variable.");

            client.BaseAddress = new Uri(opts.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("x-goog-api-key", opts.ApiKey);
        });

        return services;
    }
}