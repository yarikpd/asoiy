using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dz3.Models;

namespace dz3.Controllers;

/// <summary>
/// Контроллер для управления справочником магазинов (Master-таблица).
/// </summary>
public class StoresController : Controller
{
    /// <summary>
    /// Отображает список всех магазинов с поддержкой сортировки.
    /// </summary>
    /// <param name="sort">Параметр сортировки.</param>
    /// <returns>Представление со списком магазинов.</returns>
    public IActionResult Index(string sort = "")
    {
        using var context = new ProjectDbContext();
        
        var query = from s in context.Stores.Include(s => s.Orders)
                    select s;

        query = sort.ToLower() switch
        {
            "id_desc" => query.OrderByDescending(s => s.Id),
            "name" => query.OrderBy(s => s.Name),
            "name_desc" => query.OrderByDescending(s => s.Name),
            "orderscount" => query.OrderBy(s => s.Orders.Count),
            "orderscount_desc" => query.OrderByDescending(s => s.Orders.Count),
            _ => query.OrderBy(s => s.Id)
        };

        ViewBag.CurrentSort = sort;
        ViewBag.IdSort = string.IsNullOrEmpty(sort) ? "id_desc" : "";
        ViewBag.NameSort = sort == "name" ? "name_desc" : "name";
        ViewBag.OrdersCountSort = sort == "orderscount" ? "orderscount_desc" : "orderscount";

        return View(query.ToList());
    }

    /// <summary>
    /// Отображает форму создания нового магазина.
    /// </summary>
    /// <returns>Представление формы создания.</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Обрабатывает отправку формы создания нового магазина.
    /// </summary>
    /// <param name="store">Данные магазина.</param>
    /// <returns>Перенаправление на список или возврат к форме при ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Stores store)
    {
        if (string.IsNullOrWhiteSpace(store.Name))
        {
            ModelState.AddModelError("Name", "Название магазина не может быть пустым.");
        }

        if (ModelState.IsValid)
        {
            using var context = new ProjectDbContext();
            context.Stores.Add(store);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        return View(store);
    }

    /// <summary>
    /// Отображает форму редактирования существующего магазина.
    /// </summary>
    /// <param name="id">Идентификатор магазина.</param>
    /// <returns>Представление формы редактирования.</returns>
    public IActionResult Edit(int id)
    {
        using var context = new ProjectDbContext();
        var store = context.Stores.Find(id);
        if (store == null)
        {
            return NotFound();
        }
        return View(store);
    }

    /// <summary>
    /// Обрабатывает отправку формы редактирования магазина.
    /// </summary>
    /// <param name="id">Идентификатор магазина.</param>
    /// <param name="store">Данные магазина.</param>
    /// <returns>Перенаправление на список или возврат к форме при ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Stores store)
    {
        if (id != store.Id)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(store.Name))
        {
            ModelState.AddModelError("Name", "Название магазина не может быть пустым.");
        }

        if (ModelState.IsValid)
        {
            using var context = new ProjectDbContext();
            context.Entry(store).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        return View(store);
    }

    /// <summary>
    /// Отображает страницу подтверждения удаления магазина.
    /// </summary>
    /// <param name="id">Идентификатор магазина.</param>
    /// <returns>Представление подтверждения удаления.</returns>
    public IActionResult Delete(int id)
    {
        using var context = new ProjectDbContext();
        var store = context.Stores.Include(s => s.Orders).FirstOrDefault(s => s.Id == id);
        if (store == null)
        {
            return NotFound();
        }
        return View(store);
    }

    /// <summary>
    /// Обрабатывает подтвержденное удаление магазина.
    /// </summary>
    /// <param name="id">Идентификатор магазина.</param>
    /// <returns>Перенаправление на список или сообщение об ошибке.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = new ProjectDbContext();
        var store = context.Stores.Include(s => s.Orders).FirstOrDefault(s => s.Id == id);
        if (store == null)
        {
            return NotFound();
        }

        if (store.Orders.Any())
        {
            ModelState.AddModelError(string.Empty, "Нельзя удалить магазин, так как в основной таблице есть связанные с ним заказы.");
            return View(store);
        }

        context.Stores.Remove(store);
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
