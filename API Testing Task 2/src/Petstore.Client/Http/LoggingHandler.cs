using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace Petstore.Client.Http;

public sealed class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var correlationId = request.Headers.TryGetValues("X-Correlation-Id", out var values) ? values.FirstOrDefault() ?? "n/a" : "n/a";

        var method = request.Method.Method;
        var url = request.RequestUri?.ToString() ?? "n/a";

        var requestBody = "<empty>";

        if (request.Content != null)
        {
            requestBody = await request.Content.ReadAsStringAsync();

            var mediaType = request.Content.Headers.ContentType?.MediaType ?? "application/json";

            request.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
        }

        _logger.LogInformation(
            "[{CorrelationId}] REQUEST {Method} {Url}\nPayload: {Payload}",
            correlationId,
            method,
            url,
            requestBody);

        var sw = Stopwatch.StartNew();

        var response = await base.SendAsync(request, cancellationToken);

        var responseBody = response.Content != null ? await response.Content.ReadAsStringAsync() : "<empty>";

        _logger.LogInformation(
            "[{CorrelationId}] RESPONSE {Method} {Url} -> {StatusCode} ({DurationMs}ms)\nBody: {Body}",
            correlationId,
            method,
            url,
            (int)response.StatusCode,
            sw.ElapsedMilliseconds,
            responseBody);

        return response;
    }
}
