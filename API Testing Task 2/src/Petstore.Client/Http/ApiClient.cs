using System.Net.Http.Json;

namespace Petstore.Client.Http;

public abstract class ApiClient
{
    protected readonly HttpClient Http;

    protected ApiClient(HttpClient httpClient)
    {
        Http = httpClient;
    }

    protected static async Task<T> ReadRequiredAsync<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>();

        if (result == null)
            throw new InvalidOperationException(
                $"Failed to deserialize response to {typeof(T).Name}");

        return result;
    }

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }

    protected async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> action)
    {
        return await action();
    }

}
