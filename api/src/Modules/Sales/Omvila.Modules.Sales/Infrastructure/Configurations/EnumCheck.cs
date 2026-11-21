namespace Omvila.Modules.Sales.Infrastructure.Configurations;

/// <summary>Enum sütunları metin + CHECK olarak saklanır (alan-modeli.md § Ortak kurallar: Enum).</summary>
internal static class EnumCheck
{
    public static string In<TEnum>(string column)
        where TEnum : struct, Enum =>
        $"{column} IN ({string.Join(", ", Enum.GetNames<TEnum>().Select(n => $"'{n}'"))})";
}
