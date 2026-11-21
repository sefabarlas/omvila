namespace Omvila.Infrastructure.Push;

/// <summary>
/// appsettings "Push". <see cref="Provider"/> boşsa push kapalıdır (müşteri mesajı SMS'e düşer, işletme push'u atlanır).
/// "Fake" yalnız Development ve Testing'de; "Fcm" kimlik bilgisi verilmezse kapalı kalır. Hizmet hesabı JSON'u kasadan
/// veya ortamdan gelir (<c>Push__Fcm__CredentialsJson</c>), depoya girmez.
/// </summary>
public sealed class PushOptions
{
    public const string Section = "Push";

    public const string FakeProvider = "Fake";

    public const string FcmProvider = "Fcm";

    public string Provider { get; set; } = string.Empty;

    public FcmOptions Fcm { get; set; } = new();
}
