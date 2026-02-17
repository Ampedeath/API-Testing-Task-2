using Petstore.Client.Models;

namespace Petstore.Tests.TestData;
public static class PetBuilder
{
    public static Pet Default() => new()
    {
        Name = $"pet-{Guid.NewGuid():N}".Substring(0, 10),
        PhotoUrls = new() { "http://test/img.png" },
        Status = "available"
    };
}
