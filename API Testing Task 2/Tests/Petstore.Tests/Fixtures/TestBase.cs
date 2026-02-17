using Allure.NUnit;
using Microsoft.Extensions.Logging;
using Petstore.Client.Clients;
using Petstore.Client.Configuration;
using Petstore.Client.Http;
using Petstore.Client.Models;
using Serilog;
using Serilog.Extensions.Logging;

namespace Petstore.Tests.Fixtures;

[AllureNUnit]
public abstract class TestBase
{
    protected TestSettings Settings = null!;
    protected Guid CorrelationId;

    protected PetClient PetClient = null!;
    protected StoreClient StoreClient = null!;
    protected UserClient UserClient = null!;

    private SerilogLoggerFactory? _loggerFactory;
    private readonly List<Func<Task>> _cleanupActions = new();

    [SetUp]
    public void SetUp()
    {
        Settings = TestSettings.Load();

        CorrelationId = Guid.NewGuid();

        InitLogger();

        var httpClient = CreateHttpClient();

        PetClient = new PetClient(httpClient);
        StoreClient = new StoreClient(httpClient);
        UserClient = new UserClient(httpClient);

        _cleanupActions.Clear();
    }

    private void InitLogger()
    {
        Directory.CreateDirectory("logs");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(ParseSerilogLevel(Settings.LogLevel))
            .WriteTo.Console()
            .WriteTo.File(Settings.LogFile, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _loggerFactory = new SerilogLoggerFactory(Log.Logger, dispose: false);
    }

    private HttpClient CreateHttpClient()
    {
        var handler = new LoggingHandler(_loggerFactory!.CreateLogger<LoggingHandler>())
        {
            InnerHandler = new HttpClientHandler()
        };

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(Settings.BaseUrl),
            Timeout = TimeSpan.FromSeconds(Settings.TimeoutSeconds)
        };

        client.DefaultRequestHeaders.Add(Settings.ApiKeyHeader, Settings.ApiKeyValue);
        client.DefaultRequestHeaders.Add("X-Correlation-Id", CorrelationId.ToString());

        return client;
    }

    protected async Task<Pet> CreatePetWithCleanupAsync(Pet pet)
    {
        var created = await PetClient.CreateAsync(pet);

        RegisterCleanup(() => PetClient.DeleteAsync(created.Id));

        return created;
    }

    protected void RegisterCleanup(Func<Task> cleanup)
    {
        _cleanupActions.Add(cleanup);
    }

    [TearDown]
    public async Task TearDown()
    {
        foreach (var cleanup in _cleanupActions)
        {
            try
            {
                await cleanup();
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Cleanup failed: {ex}");
            }
        }

        Log.CloseAndFlush();
        _loggerFactory?.Dispose();
    }

    private static Serilog.Events.LogEventLevel ParseSerilogLevel(string level)
    {
        return Enum.TryParse(level, true, out Serilog.Events.LogEventLevel parsed)
            ? parsed
            : Serilog.Events.LogEventLevel.Information;
    }
}
