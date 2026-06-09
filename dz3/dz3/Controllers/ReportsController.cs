using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dz3.ViewModels;

namespace dz3.Controllers;

/// <summary>
/// Контроллер для формирования аналитических отчетов.
/// </summary>
public class ReportsController : Controller
{
    /// <summary>
    /// Формирует и отображает трехкомпонентный аналитический отчет.
    /// </summary>
    /// <returns>Представление с моделью отчета.</returns>
    public IActionResult Index()
    {
        using var context = new ProjectDbContext();

        // Раздел 1: Полный список заказов с названиями магазинов
        var report1 = context.Orders
            .Include(o => o.Store)
            .OrderBy(o => o.Name)
            .Select(o => new Report1Item
            {
                OrderName = o.Name,
                StoreName = o.Store != null ? o.Store.Name : "Не указан",
                Amount = (decimal)o.Amount
            })
            .ToList();

        // Раздел 2: Количество заказов по магазинам
        var report2 = context.Orders
            .GroupBy(o => o.Store.Name)
            .Select(g => new Report2Item
            {
                StoreName = g.Key ?? "Не указан",
                Count = g.Count()
            })
            .OrderBy(r => r.StoreName)
            .ToList();

        // Раздел 3: Средняя сумма заказа по магазинам (сортировка по убыванию)
        var report3 = context.Orders
            .GroupBy(o => o.Store.Name)
            .Select(g => new Report3Item
            {
                StoreName = g.Key ?? "Не указан",
                AverageAmount = (decimal)g.Average(o => o.Amount)
            })
            .AsEnumerable()
            .OrderByDescending(r => r.AverageAmount)
            .ToList();

        var viewModel = new ReportViewModel
        {
            Report1 = report1,
            Report2 = report2,
            Report3 = report3
        };

        return View(viewModel);
    }
}
