namespace Omvila.Host;

internal static class HostRoleParser
{
    public static HostRole Parse(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        null or "" or "all" => HostRole.All,
        "api" => HostRole.Api,
        "worker" => HostRole.Worker,
        "seed" => HostRole.Seed,
        _ => throw new ArgumentException($"Geçersiz rol: '{value}'. Kullanım: --role=api|worker|all|seed", nameof(value)),
    };
}
