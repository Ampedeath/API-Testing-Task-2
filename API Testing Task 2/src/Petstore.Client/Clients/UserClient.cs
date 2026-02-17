using System.Net.Http.Json;
using Petstore.Client.Http;
using Petstore.Client.Models;

namespace Petstore.Client.Clients;

public sealed class UserClient : ApiClient
{
    public UserClient(HttpClient httpClient) : base(httpClient) { }

    public async Task CreateAsync(User user)
    {
        var response = await Http.PostAsJsonAsync("user", user);
        await EnsureSuccessAsync(response);
    }

    public async Task<User> GetAsync(string username)
    {
        var response = await Http.GetAsync($"user/{username}");
        return await ReadRequiredAsync<User>(response);
    }

    public async Task UpdateAsync(string username, User user)
    {
        var response = await Http.PutAsJsonAsync($"user/{username}", user);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteAsync(string username)
    {
        var response = await Http.DeleteAsync($"user/{username}");
        await EnsureSuccessAsync(response);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var response = await Http.GetAsync(
            $"user/login?username={username}&password={password}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task LogoutAsync()
    {
        var response = await Http.GetAsync("user/logout");
        await EnsureSuccessAsync(response);
    }

    public async Task<HttpResponseMessage> LoginRawAsync(string username, string password)
    {
        return await Http.GetAsync($"user/login?username={username}&password={password}");
    }

}
