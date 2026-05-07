namespace dz2;

internal class Store(int id, string name)
{
    private int Id { get; set; } = id;

    private string Name { get; set; } = name;

    public override string ToString() => $"[{Id}] {Name}";
}