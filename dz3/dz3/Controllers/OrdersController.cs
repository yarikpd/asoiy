using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dz3.Models;

namespace dz3.Controllers;

/// <summary>
/// Контроллер для управления заказами (Detail-таблица).
/// </summary>
public class OrdersController : Controller
{
    /// <summary>
    /// Отображает список всех заказов с поддержкой сортировки.
    /// </summary>
    /// <param name="sort">Параметр сортировки.</param>
    /// <returns>Представление со списком заказов.</returns>
    public IActionResult Index(string sort = "")
    {
        using var context = new ProjectDbContext();
        
        var query = from o in context.Orders.Include(o => o.Store)
                    select o;

        query = sort.ToLower() switch
        {
            "id_desc" => query.OrderByDescending(o => o.Id),
            "name" => query.OrderBy(o => o.Name),
            "name_desc" => query.OrderByDescending(o => o.Name),
            "amount" => query.OrderBy(o => o.Amount),
            "amount_desc" => query.OrderByDescending(o => o.Amount),
            "store" or "storename" => query.OrderBy(o => o.Store.Name),
            "store_desc" or "storename_desc" => query.OrderByDescending(o => o.Store.Name),
            _ => query.OrderBy(o => o.Id)
        };

        ViewBag.CurrentSort = sort;
        ViewBag.IdSort = string.IsNullOrEmpty(sort) ? "id_desc" : "";
        ViewBag.NameSort = sort == "name" ? "name_desc" : "name";
        ViewBag.AmountSort = sort == "amount" ? "amount_desc" : "amount";
        ViewBag.StoreSort = sort == "store" ? "store_desc" : "store";

        return View(query.ToList());
    }

    /// <summary>
    /// Отображает форму создания нового заказа.
    /// </summary>
    /// <returns>Представление формы создания.</returns>
    public IActionResult Create()
    {
        using var context = new ProjectDbContext();
        ViewBag.StoreId = new SelectList(context.Stores.ToList(), "Id", "Name");
        return View();
    }

    /// <summary>
    /// Обрабатывает отправку формы создания нового заказа.
    /// </summary>
    /// <param name="order">Данные заказа.</param>
    /// <returns>Перенаправление на список или возврат к форме при ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Orders order)
    {
        if (string.IsNullOrWhiteSpace(order.Name))
        {
            ModelState.AddModelError("Name", "Название заказа не может быть пустым.");
        }

        if (order.Amount < 0)
        {
            ModelState.AddModelError("Amount", "Сумма заказа не может быть отрицательной.");
        }

        ModelState.Remove("Store");

        if (ModelState.IsValid)
        {
            using var context = new ProjectDbContext();
            context.Orders.Add(order);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        using var db = new ProjectDbContext();
        ViewBag.StoreId = new SelectList(db.Stores.ToList(), "Id", "Name", order.StoreId);
        return View(order);
    }

    /// <summary>
    /// Отображает форму редактирования существующего заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Представление формы редактирования.</returns>
    public IActionResult Edit(int id)
    {
        using var context = new ProjectDbContext();
        var order = context.Orders.Find(id);
        if (order == null)
        {
            return NotFound();
        }

        ViewBag.StoreId = new SelectList(context.Stores.ToList(), "Id", "Name", order.StoreId);
        return View(order);
    }

    /// <summary>
    /// Обрабатывает отправку формы редактирования заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="order">Данные заказа.</param>
    /// <returns>Перенаправление на список или возврат к форме при ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Orders order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(order.Name))
        {
            ModelState.AddModelError("Name", "Название заказа не может быть пустым.");
        }

        if (order.Amount < 0)
        {
            ModelState.AddModelError("Amount", "Сумма заказа не может быть отрицательной.");
        }

        ModelState.Remove("Store");

        if (ModelState.IsValid)
        {
            using var context = new ProjectDbContext();
            context.Entry(order).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        using var db = new ProjectDbContext();
        ViewBag.StoreId = new SelectList(db.Stores.ToList(), "Id", "Name", order.StoreId);
        return View(order);
    }

    /// <summary>
    /// Отображает страницу подтверждения удаления заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Представление подтверждения удаления.</returns>
    public IActionResult Delete(int id)
    {
        using var context = new ProjectDbContext();
        var order = context.Orders.Include(o => o.Store).FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    /// <summary>
    /// Обрабатывает подтвержденное удаление заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Перенаправление на список.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = new ProjectDbContext();
        var order = context.Orders.Find(id);
        if (order == null) return RedirectToAction(nameof(Index));
        context.Orders.Remove(order);
        context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
