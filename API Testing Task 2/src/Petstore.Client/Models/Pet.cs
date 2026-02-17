namespace Petstore.Client.Models;

public class Pet
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public List<string> PhotoUrls { get; set; } = new();
    public string Status { get; set; } = "available";
}
