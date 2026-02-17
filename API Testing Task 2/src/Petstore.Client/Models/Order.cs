namespace Petstore.Client.Models;

public class Order
{
    public long Id { get; set; }
    public long PetId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = "placed";
    public bool Complete { get; set; }
}
