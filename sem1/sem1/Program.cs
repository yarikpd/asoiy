Console.WriteLine("Введите количество участников: ");
var strN = Console.ReadLine();
if (!int.TryParse(strN, out var n))
{
    Console.WriteLine("Неверный ввод. Пожалуйста, введите целое число для количества участников.");
    return;
}

var teams = new string[n];
var avgSpeeds = new double[n];

for (var i = 0; i < n; i++)
{
    Console.WriteLine($"\nУчастник #{i + 1}:");
    Console.WriteLine("Введите название команды: ");
    var team = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(team))
    {
        Console.WriteLine("Неверный ввод. Пожалуйста, введите название команды.");
        while (string.IsNullOrWhiteSpace(team))
        {
            Console.WriteLine("Введите название команды: ");
            team = Console.ReadLine();
        }
    }
    
    Console.WriteLine("Введите среднюю скорость: ");
    var strSpeed = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(strSpeed))
    {
        Console.WriteLine("Неверный ввод. Пожалуйста, введите число для средней скорости.");
        while (string.IsNullOrWhiteSpace(strSpeed))
        {
            Console.WriteLine("Введите среднюю скорость: ");
            strSpeed = Console.ReadLine();
        }
    }
    
    strSpeed = strSpeed.Replace(".", ",");
    if (!double.TryParse(strSpeed, out var speed))
    {
        Console.WriteLine("Неверный ввод. Пожалуйста, введите число для средней скорости.");
        while (!double.TryParse(strSpeed, out speed))
        {
            Console.WriteLine("Введите среднюю скорость: ");
            strSpeed = Console.ReadLine();
        }
    }

    teams[i] = team;
    avgSpeeds[i] = speed;
}

Console.WriteLine("\n\n--- СТАТИСТИКА КВАЛИФИКАЦИИ ---");
var avgSpeedSum = 0.0;
var maxSpeedIndex = 0;
var maxSpeed = avgSpeeds[0];
var minSpeedIndex = 0;
var minSpeed = avgSpeeds[0];

for (var i = 0; i < n; i++)
{
    avgSpeedSum += avgSpeeds[i];
    
    if (avgSpeeds[i] > maxSpeed)
    {
        maxSpeed = avgSpeeds[i];
        maxSpeedIndex = i;
    }
    if (avgSpeeds[i] < minSpeed)
    {
        minSpeed = avgSpeeds[i];
        minSpeedIndex = i;
    }
}
var avgSpeedOverall = avgSpeedSum / n;

Console.WriteLine($"Средняя скорость всех участников: {avgSpeedOverall:F2}");
Console.WriteLine($"Лидер: {teams[maxSpeedIndex]} ({maxSpeed:F2} км/ч)");
Console.WriteLine($"Самый медленный: {teams[minSpeedIndex]} ({minSpeed:F2} км/ч)");
Console.WriteLine($"Разница темпа: {maxSpeed - minSpeed:F2} км/ч");