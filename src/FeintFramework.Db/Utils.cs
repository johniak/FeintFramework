
using System.Text.RegularExpressions;

public static class Utils
{
    public static string ToUnderscoreCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        string result = Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2");
        return result.ToLowerInvariant();
    }
}