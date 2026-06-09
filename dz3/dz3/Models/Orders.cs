namespace dz3.Models;

public class Orders
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int StoreId { get; set; }
    public double Amount { get; set; }
    
    public Stores Store { get; set; } = null!;
}