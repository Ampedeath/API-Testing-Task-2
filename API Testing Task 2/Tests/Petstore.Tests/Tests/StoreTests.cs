using FluentAssertions;
using Petstore.Client.Models;
using Petstore.Tests.Fixtures;
using static Petstore.Tests.Utils.TestSteps;

namespace Petstore.Tests.Tests;

[TestFixture]
public class StoreTests : TestBase
{
    [Test]
    public async Task GetInventory_ShouldReturnDictionary()
    {
        var inventory = await StoreClient.GetInventoryAsync();

        TestCaseStep("Verify inventory structure", () =>
        {
            inventory.Should().NotBeNull();
            inventory.Should().NotBeEmpty();
        });
    }

    [Test]
    public async Task PlaceOrder_ShouldCreateOrder()
    {
        var order = new Order
        {
            PetId = 1,
            Quantity = 1,
            Status = "placed",
            Complete = false
        };

        var created = await StoreClient.PlaceOrderAsync(order);

        RegisterCleanup(() => StoreClient.DeleteOrderAsync(created.Id));

        TestCaseStep("Verify order created", () =>
        {
            created.Id.Should().BeGreaterThan(0);
            created.Quantity.Should().Be(1);
        });
    }

    [Test]
    public async Task GetOrder_ShouldReturnOrder()
    {
        var order = new Order
        {
            PetId = 1,
            Quantity = 1,
            Status = "placed"
        };

        var created = await StoreClient.PlaceOrderAsync(order);

        RegisterCleanup(() => StoreClient.DeleteOrderAsync(created.Id));

        var fetched = await StoreClient.GetOrderAsync(created.Id);

        fetched.Id.Should().Be(created.Id);
    }

    [Test]
    public async Task DeleteOrder_ShouldRemoveOrder()
    {
        var order = new Order
        {
            PetId = 1,
            Quantity = 1
        };

        var created = await StoreClient.PlaceOrderAsync(order);

        await StoreClient.DeleteOrderAsync(created.Id);

        Func<Task> act = async () =>
            await StoreClient.GetOrderAsync(created.Id);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    // NEGATIVE

    [Test]
    public async Task PlaceOrder_InvalidQuantity_ShouldFail()
    {
        var order = new Order
        {
            Id = Random.Shared.NextInt64(),
            PetId = 1,
            Quantity = -1
        };

        var response = await StoreClient.PlaceOrderRawAsync(order);

        response.IsSuccessStatusCode.Should().BeFalse();
    }


    [Test]
    public async Task GetOrder_NonExistent_ShouldFail()
    {
        Func<Task> act = async () =>
            await StoreClient.GetOrderAsync(999999);

        await act.Should().ThrowAsync<HttpRequestException>();
    }
}
