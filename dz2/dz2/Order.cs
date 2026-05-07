namespace dz2;

internal class Order
{
    public int Id { get; }

    public int StoreId { get; set; }

    public string Name { get; set; }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value < 0)
                throw new ArgumentException(
                    "Сумма заказа не может быть отрицательной");
            _amount = value;
        }
    }

    public Order(int id, int storeId, string name, decimal amount)
    {
        Id = id;
        StoreId = storeId;
        Name = name;
        Amount = amount;
    }

    public override string ToString()
        => $"[{Id}] {Name}, магазин #{StoreId}, сумма: {Amount} руб.";
}