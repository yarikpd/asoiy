using Microsoft.AspNetCore.Mvc;

namespace dz3.Controllers;

/// <summary>
/// Контроллер главной страницы.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Отображает главную страницу с панелью управления.
    /// </summary>
    /// <returns>Представление главной страницы.</returns>
    public IActionResult Index()
    {
        return View();
    }
}
