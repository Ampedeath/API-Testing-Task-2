using System.Net.Http.Json;
using Petstore.Client.Http;
using Petstore.Client.Models;

namespace Petstore.Client.Clients;

public sealed class StoreClient : ApiClient
{
    public StoreClient(HttpClient httpClient) : base(httpClient) { }

    public async Task<Dictionary<string, int>> GetInventoryAsync()
    {
        var response = await Http.GetAsync("store/inventory");
        return await ReadRequiredAsync<Dictionary<string, int>>(response);
    }

    public async Task<Order> PlaceOrderAsync(Order order)
    {
        var response = await Http.PostAsJsonAsync("store/order", order);
        return await ReadRequiredAsync<Order>(response);
    }

    public async Task<Order> GetOrderAsync(long orderId)
    {
        var response = await Http.GetAsync($"store/order/{orderId}");
        return await ReadRequiredAsync<Order>(response);
    }

    public async Task DeleteOrderAsync(long orderId)
    {
        var response = await Http.DeleteAsync($"store/order/{orderId}");
        await EnsureSuccessAsync(response);
    }

    public async Task<HttpResponseMessage> PlaceOrderRawAsync(Order order)
    {
        return await Http.PostAsJsonAsync("store/order", order);
    }

}
