namespace Petstore.Tests.Utils;

public static class JsonExtensions
{
    public static string Truncate(this string value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLen ? value : value.Substring(0, maxLen) + "…(truncated)";
    }
}
