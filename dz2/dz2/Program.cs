using System.Text;
using dz2;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string dbPath = "orders.db";
var storeCsv = Path.Combine(AppContext.BaseDirectory, "stores.csv");
var orderCsv = Path.Combine(AppContext.BaseDirectory, "orders.csv");

var db = new DatabaseManager(dbPath);
db.InitializeDatabase(storeCsv, orderCsv);

Console.WriteLine();

string choice;
do
{
    Console.WriteLine("╔══════════════════════════════════════╗");
    Console.WriteLine("║         УПРАВЛЕНИЕ ЗАКАЗАМИ          ║");
    Console.WriteLine("╠══════════════════════════════════════╣");
    Console.WriteLine("║  1 — Показать все магазины           ║");
    Console.WriteLine("║  2 — Показать все заказы             ║");
    Console.WriteLine("║  3 — Добавить заказ                  ║");
    Console.WriteLine("║  4 — Редактировать заказ             ║");
    Console.WriteLine("║  5 — Удалить заказ                   ║");
    Console.WriteLine("║  6 — Отчёты                          ║");
    Console.WriteLine("║  0 — Выход                           ║");
    Console.WriteLine("╚══════════════════════════════════════╝");
    Console.Write("Ваш выбор: ");

    choice = Console.ReadLine()?.Trim() ?? "";
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1": ShowStores(db); break;
            case "2": ShowOrders(db); break;
            case "3": AddOrder(db); break;
            case "4": EditOrder(db); break;
            case "5": DeleteOrder(db); break;
            case "6": ReportsMenu(db); break;
            case "0": Console.WriteLine("До свидания!"); break;
            default: Console.WriteLine("Неверный пункт меню."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }

    Console.WriteLine();
}
while (choice != "0");

static void ShowStores(DatabaseManager db)
{
    Console.WriteLine("--- Все магазины ---");
    var stores = db.GetAllStores();
    foreach (var store in stores)
        Console.WriteLine("  " + store);
    Console.WriteLine($"Итого: {stores.Count}");
}

static void ShowOrders(DatabaseManager db)
{
    Console.WriteLine("--- Все заказы ---");
    var orders = db.GetAllOrders();
    foreach (var order in orders)
        Console.WriteLine("  " + order);
    Console.WriteLine($"Итого: {orders.Count}");
}

static void AddOrder(DatabaseManager db)
{
    Console.WriteLine("--- Добавление заказа ---");

    Console.WriteLine("Доступные магазины:");
    var stores = db.GetAllStores();
    foreach (var store in stores)
        Console.WriteLine("  " + store);

    Console.Write("ID магазина: ");
    if (!int.TryParse(Console.ReadLine(), out int storeId))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    Console.Write("Название заказа: ");
    var name = Console.ReadLine()?.Trim() ?? "";
    if (name.Length == 0)
    {
        Console.WriteLine("Ошибка: название не может быть пустым.");
        return;
    }

    Console.Write("Сумма заказа (руб.): ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
    {
        Console.WriteLine("Ошибка: введите число.");
        return;
    }

    try
    {
        var order = new Order(0, storeId, name, amount);
        db.AddOrder(order);
        Console.WriteLine("Заказ добавлен.");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}

static void EditOrder(DatabaseManager db)
{
    Console.WriteLine("--- Редактирование заказа ---");
    Console.Write("Введите ID заказа: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var order = db.GetOrderById(id);
    if (order == null)
    {
        Console.WriteLine($"Заказ с ID={id} не найден.");
        return;
    }

    Console.WriteLine($"Текущие данные: {order}");
    Console.WriteLine("(нажмите Enter, чтобы оставить значение без изменений)");

    Console.Write($"Название [{order.Name}]: ");
    var input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0)
        order.Name = input;

    Console.Write($"ID магазина [{order.StoreId}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && int.TryParse(input, out int newStoreId))
        order.StoreId = newStoreId;

    Console.Write($"Сумма [{order.Amount}]: ");
    input = Console.ReadLine()?.Trim() ?? "";
    if (input.Length > 0 && decimal.TryParse(input, out decimal newAmount))
    {
        try
        {
            order.Amount = newAmount;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return;
        }
    }

    db.UpdateOrder(order);
    Console.WriteLine("Данные обновлены.");
}

static void DeleteOrder(DatabaseManager db)
{
    Console.WriteLine("--- Удаление заказа ---");
    Console.Write("Введите ID заказа: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("Ошибка: введите целое число.");
        return;
    }

    var order = db.GetOrderById(id);
    if (order == null)
    {
        Console.WriteLine($"Заказ с ID={id} не найден.");
        return;
    }

    Console.Write($"Удалить «{order.Name}»? (да/нет): ");
    var confirm = Console.ReadLine()?.Trim().ToLower() ?? "";
    if (confirm == "да")
    {
        db.DeleteOrder(id);
        Console.WriteLine("Заказ удалён.");
    }
    else
    {
        Console.WriteLine("Удаление отменено.");
    }
}

static void ReportsMenu(DatabaseManager db)
{
    string choice;
    do
    {
        Console.WriteLine("--- Отчёты ---");
        Console.WriteLine("  1 — Заказы по магазинам");
        Console.WriteLine("  2 — Количество заказов в магазинах");
        Console.WriteLine("  3 — Средняя сумма заказа по магазинам");
        Console.WriteLine("  0 — Назад");
        Console.Write("Ваш выбор: ");

        choice = Console.ReadLine()?.Trim() ?? "";

        switch (choice)
        {
            case "1": Report1OrdersWithStores(db); break;
            case "2": Report2CountByStore(db); break;
            case "3": Report3_AvgAmountByStore(db); break;
            case "0": break;
            default: Console.WriteLine("Неверный пункт."); break;
        }

        Console.WriteLine();
    }
    while (choice != "0");
}

static void Report1OrdersWithStores(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query("""
               SELECT o.order_name, s.store_name, o.amount
                                FROM ord o
                                JOIN store s ON o.store_id = s.store_id
                                ORDER BY o.order_name
               """)
        .Title("Заказы по магазинам")
        .Header("Заказ", "Магазин", "Сумма (руб.)")
        .ColumnWidths(28, 15, 15)
        .Footer("Всего записей")
        .Print();
}

static void Report2CountByStore(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query("""
               SELECT s.store_name, COUNT(*) AS cnt
                                FROM ord
                                JOIN store s ON ord.store_id = s.store_id
                                GROUP BY s.store_name
                                ORDER BY s.store_name
               """)
        .Title("Количество заказов по магазинам")
        .Header("Магазин", "Кол-во")
        .ColumnWidths(20, 10)
        .Print();
}

static void Report3_AvgAmountByStore(DatabaseManager db)
{
    new ReportBuilder(db)
        .Query("""
               SELECT s.store_name,
                                       ROUND(AVG(o.amount), 2) AS avg_amount
                                FROM ord o
                                JOIN store s ON o.store_id = s.store_id
                                GROUP BY s.store_name
                                ORDER BY avg_amount DESC
               """)
        .Title("Средняя сумма заказа по магазинам")
        .Header("Магазин", "Средняя сумма (руб.)")
        .ColumnWidths(20, 22)
        .Print();
}
