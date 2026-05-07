using Microsoft.Data.Sqlite;

namespace dz2;

internal class DatabaseManager(string dbPath)
{
    private readonly string _connectionString = $"Data Source={dbPath}";

    public void InitializeDatabase(string storeCsvPath, string orderCsvPath)
    {
        CreateTables();

        if (GetAllStores().Count == 0 && File.Exists(storeCsvPath))
        {
            ImportStoresFromCsv(storeCsvPath);
            Console.WriteLine($"[OK] Загружены магазины из {storeCsvPath}");
        }

        if (GetAllOrders().Count != 0 || !File.Exists(orderCsvPath)) return;
        ImportOrdersFromCsv(orderCsvPath);
        Console.WriteLine($"[OK] Загружены заказы из {orderCsvPath}");
    }

    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """

                                  CREATE TABLE IF NOT EXISTS store (
                                      store_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                                      store_name TEXT NOT NULL
                                  );
                                  CREATE TABLE IF NOT EXISTS ord (
                                      order_id      INTEGER PRIMARY KEY AUTOINCREMENT,
                                      store_id      INTEGER NOT NULL,
                                      order_name    TEXT    NOT NULL,
                                      amount        REAL    NOT NULL,
                                      FOREIGN KEY (store_id) REFERENCES store(store_id)
                                  );
                          """;
        cmd.ExecuteNonQuery();
    }

    private void ImportStoresFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var lines = File.ReadAllLines(path);
        for (var i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(';');
            if (parts.Length < 2) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO store (store_id, store_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }

    private void ImportOrdersFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var lines = File.ReadAllLines(path);
        for (var i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(';');
            if (parts.Length < 4) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = """

                                          INSERT INTO ord (order_id, store_id, order_name, amount)
                                          VALUES (@id, @storeId, @name, @amount)
                              """;
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@storeId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@amount", decimal.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }

    public List<Store> GetAllStores()
    {
        var result = new List<Store>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT store_id, store_name FROM store ORDER BY store_id";
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new Store(
                reader.GetInt32(0),
                reader.GetString(1)));
        }
        return result;
    }

    public List<Order> GetAllOrders()
    {
        var result = new List<Order>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT order_id, store_id, order_name, amount FROM ord ORDER BY order_id";
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new Order(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDecimal(3)));
        }
        return result;
    }

    public Order? GetOrderById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT order_id, store_id, order_name, amount FROM ord WHERE order_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Order(
                reader.GetInt32(0), reader.GetInt32(1),
                reader.GetString(2), reader.GetDecimal(3));
        }
        return null;
    }

    public void AddOrder(Order order)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """

                                  INSERT INTO ord (store_id, order_name, amount)
                                  VALUES (@storeId, @name, @amount)
                          """;
        cmd.Parameters.AddWithValue("@storeId", order.StoreId);
        cmd.Parameters.AddWithValue("@name", order.Name);
        cmd.Parameters.AddWithValue("@amount", order.Amount);
        cmd.ExecuteNonQuery();
    }

    public void UpdateOrder(Order order)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """

                                  UPDATE ord
                                  SET store_id = @storeId, order_name = @name, amount = @amount
                                  WHERE order_id = @id
                          """;
        cmd.Parameters.AddWithValue("@id", order.Id);
        cmd.Parameters.AddWithValue("@storeId", order.StoreId);
        cmd.Parameters.AddWithValue("@name", order.Name);
        cmd.Parameters.AddWithValue("@amount", order.Amount);
        cmd.ExecuteNonQuery();
    }

    public void DeleteOrder(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM ord WHERE order_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        var columns = new string[reader.FieldCount];
        for (var i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            var row = new string[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i).ToString() ?? "";
            rows.Add(row);
        }

        return (columns, rows);
    }
}