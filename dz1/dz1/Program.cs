static int distance(string s1, string s2)
{
    if (s1.Length == 0 || s2.Length == 0) return Math.Max(s1.Length, s2.Length);
    
    s1 = s1.ToUpper();
    s2 = s2.ToUpper();
    
    var dp = new int[s1.Length + 1, s2.Length + 1];
    
    for (var i = 0; i <= s1.Length; i++)
        dp[i, 0] = i;
    for (var j = 0; j <= s2.Length; j++)
        dp[0, j] = j;

    for (var i = 1; i <= s1.Length; i++)
    {
        for (var j = 1; j <= s2.Length; j++)
        {
            var eq = s1[i - 1] == s2[j - 1] ? 0 : 1;
            
            dp[i, j] = Math.Min(
                Math.Min(
                    dp[i - 1, j] + 1,
                    dp[i, j - 1] + 1
                ),
                dp[i - 1, j - 1] + eq
            );

            if (i > 1 && j > 1 && s1[i - 1] == s2[j - 2] && s1[i - 2] == s2[j - 1])
            {
                dp[i, j] = Math.Min(
                    dp[i, j],
                    dp[i - 2, j - 2] + eq
                );
            }
        }
    }
    
    return dp[s1.Length, s2.Length];
}

Console.WriteLine("Введите перое слово для сравнения:");
var s1 = Console.ReadLine();

while (String.IsNullOrEmpty(s1))
{
    Console.WriteLine("Введите не пустое перое слово для сравнения:");
    s1 = Console.ReadLine();
}

Console.WriteLine("Введите второе слово для сравнения:");
var s2 = Console.ReadLine();

while (String.IsNullOrEmpty(s2))
{
    Console.WriteLine("Введите не пустое второе слово для сравнения:");
    s2 = Console.ReadLine();
}

Console.WriteLine($"'{s1}', '{s2}' -> {distance(s1, s2)}");