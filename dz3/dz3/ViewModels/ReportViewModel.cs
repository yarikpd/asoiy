using System.Collections.Generic;

namespace dz3.ViewModels;

/// <summary>
/// Элемент первого раздела отчета: Полный список заказов с названиями магазинов.
/// </summary>
public class Report1Item
{
    /// <summary>
    /// Название заказа.
    /// </summary>
    public string OrderName { get; set; } = "";

    /// <summary>
    /// Название магазина.
    /// </summary>
    public string StoreName { get; set; } = "";

    /// <summary>
    /// Сумма заказа.
    /// </summary>
    public decimal Amount { get; set; }
}

/// <summary>
/// Элемент второго раздела отчета: Количество заказов по магазинам.
/// </summary>
public class Report2Item
{
    /// <summary>
    /// Название магазина.
    /// </summary>
    public string StoreName { get; set; } = "";

    /// <summary>
    /// Количество заказов.
    /// </summary>
    public int Count { get; set; }
}

/// <summary>
/// Элемент третьего раздела отчета: Средняя сумма заказа по магазинам.
/// </summary>
public class Report3Item
{
    /// <summary>
    /// Название магазина.
    /// </summary>
    public string StoreName { get; set; } = "";

    /// <summary>
    /// Средняя сумма заказа.
    /// </summary>
    public decimal AverageAmount { get; set; }
}

/// <summary>
/// Модель представления для страницы отчетов, объединяющая все три раздела.
/// </summary>
public class ReportViewModel
{
    /// <summary>
    /// Первый раздел отчета.
    /// </summary>
    public List<Report1Item> Report1 { get; set; } = new();

    /// <summary>
    /// Второй раздел отчета.
    /// </summary>
    public List<Report2Item> Report2 { get; set; } = new();

    /// <summary>
    /// Третий раздел отчета.
    /// </summary>
    public List<Report3Item> Report3 { get; set; } = new();
}
