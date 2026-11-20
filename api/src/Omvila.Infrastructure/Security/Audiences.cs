namespace Omvila.Infrastructure.Security;

/// <summary>API yüzeyleri ve JWT aud değerleri (docs/mimari/mimari.md § API yüzeyleri). Bir yüzeyin jetonu diğerinde geçmez.</summary>
public static class Audiences
{
    public const string Manage = "manage";
    public const string Admin = "admin";
    public const string Me = "me";
    public const string PublicBooking = "public-booking";
}
