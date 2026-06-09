namespace dz3.Models;

public class Stores
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    
    public List<Orders> Orders { get; set; } = [];
}