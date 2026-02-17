using System.Net.Http.Json;
using Petstore.Client.Http;
using Petstore.Client.Models;

namespace Petstore.Client.Clients;

public sealed class PetClient : ApiClient
{
    public PetClient(HttpClient httpClient) : base(httpClient) { }

    public async Task<Pet> CreateAsync(Pet pet)
    {
        var response = await Http.PostAsJsonAsync("pet", pet);
        return await ReadRequiredAsync<Pet>(response);
    }

    public async Task<Pet> GetAsync(long petId)
    {
        var response = await Http.GetAsync($"pet/{petId}");
        return await ReadRequiredAsync<Pet>(response);
    }

    public async Task DeleteAsync(long petId)
    {
        var response = await Http.DeleteAsync($"pet/{petId}");
        await EnsureSuccessAsync(response);
    }

    public async Task UpdateAsync(Pet pet)
    {
        var response = await Http.PutAsJsonAsync("pet", pet);
        await EnsureSuccessAsync(response);
    }

    public async Task<List<Pet>> FindByStatusAsync(string status)
    {
        var response = await Http.GetAsync($"pet/findByStatus?status={status}");
        return await ReadRequiredAsync<List<Pet>>(response);
    }

    public async Task<HttpResponseMessage> CreateRawAsync(Pet pet)
    {
        return await Http.PostAsJsonAsync("pet", pet);
    }

    public async Task<HttpResponseMessage> GetRawAsync(long id)
    {
        return await Http.GetAsync($"pet/{id}");
    }
}
