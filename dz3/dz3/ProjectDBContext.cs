using dz3.Models;
using Microsoft.EntityFrameworkCore;

namespace dz3;

public class ProjectDbContext : DbContext
{
    public DbSet<Stores> Stores { get; set; } = null!;
    public DbSet<Orders> Orders { get; set; } = null!;

    public ProjectDbContext() { }

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (options.IsConfigured) return;
        options.UseSqlite("Data Source=project.db");
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }

    public void Initialize()
    {
        if (Stores.Any())
        {
            return;
        }

        var storesCsvPath = Path.Combine(Directory.GetCurrentDirectory(), "dz3", "stores.csv");
        if (!File.Exists(storesCsvPath))
        {
            storesCsvPath = Path.Combine(Directory.GetCurrentDirectory(), "stores.csv");
        }

        var storeList = new List<Stores>();

        if (File.Exists(storesCsvPath))
        {
            var lines = File.ReadAllLines(storesCsvPath);
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                var parts = line.Split(';');
                if (parts.Length >= 2)
                {
                    storeList.Add(new Stores { Name = parts[1] });
                }
            }
        }
        else
        {
            storeList.Add(new Stores { Name = "Магнит" });
            storeList.Add(new Stores { Name = "Пятёрочка" });
            storeList.Add(new Stores { Name = "Дикси" });
            storeList.Add(new Stores { Name = "Лента" });
        }

        Stores.AddRange(storeList);
        SaveChanges();
    }
}