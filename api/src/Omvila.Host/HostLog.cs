namespace Omvila.Host;

internal static partial class HostLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "OMVILA host started with role {Role}")]
    public static partial void Started(ILogger logger, HostRole role);
}
