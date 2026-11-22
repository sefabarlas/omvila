namespace Omvila.Modules.Identity.Domain;

/// <summary>Kodun amacı (alan-modeli.md § OtpChallenge). Amaç koda bağlıdır: giriş kodu rezervasyonda geçmez (ADR-0007).</summary>
internal enum OtpPurpose
{
    ManageLogin,
    ManageSignup,
    CustomerLogin,
    PublicBooking,
    AccountDeletion,
    AdminLogin,
}
