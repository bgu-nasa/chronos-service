using System.Text.RegularExpressions;

namespace Chronos.Data.Utils;

public static partial class ConfigUtils
{
    /// <summary>
    /// Pluralizes and transforms an entity name from pascal case to snake case.
    /// </summary>
    /// <param name="entityClassName">The name to transform.</param>
    /// <returns>Pluralized name in snake case.</returns>
    public static string ToTableName(string entityClassName)
    {
        var words = CapitalsRegex().Split(entityClassName).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        if (words.Length == 1)
        {
            return words[0].ToLower() + "s";
        }
        
        return string.Join("_", words.Select(s => s.ToLower())) + "s";
    }

    [GeneratedRegex("([A-Z][^A-Z]*)")]
    private static partial Regex CapitalsRegex();
}