namespace Omvila.Host;

internal static class HostRoleParser
{
    public static HostRole Parse(string? value) => value?.Trim().ToLowerInvariant() switch
    {
        null or "" or "all" => HostRole.All,
        "api" => HostRole.Api,
        "worker" => HostRole.Worker,
        "migrate" => HostRole.Migrate,
        "seed" => HostRole.Seed,
        "platform-user" => HostRole.PlatformUser,
        _ => throw new ArgumentException($"Geçersiz rol: '{value}'. Kullanım: --role=api|worker|all|migrate|seed|platform-user", nameof(value)),
    };
}
