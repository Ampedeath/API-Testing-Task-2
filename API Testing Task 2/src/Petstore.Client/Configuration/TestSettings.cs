using Microsoft.Extensions.Configuration;

namespace Petstore.Client.Configuration;

public sealed class TestSettings
{
    public string BaseUrl { get; init; } = "https://petstore.swagger.io/v2";
    public string ApiKeyHeader { get; init; } = "api_key";
    public string ApiKeyValue { get; init; } = "special-key";
    public int TimeoutSeconds { get; init; } = 30;
    public string LogFile { get; init; } = "logs/api-tests-.log";
    public string LogLevel { get; init; } = "Information";

    public static TestSettings Load()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: false).AddEnvironmentVariables().Build();

        var apiSection = config.GetSection("Api");
        var loggingSection = config.GetSection("Logging");

        return new TestSettings
        {
            BaseUrl = apiSection["BaseUrl"]!,
            ApiKeyHeader = apiSection["ApiKeyHeader"]!,
            ApiKeyValue = apiSection["ApiKeyValue"]!,
            TimeoutSeconds = int.TryParse(apiSection["TimeoutSeconds"] ?? config["TimeoutSeconds"], out var t) ? t : 30,
            LogFile = loggingSection["LogFile"]!,
            LogLevel = loggingSection["LogLevel"]!
        };
    }
}

