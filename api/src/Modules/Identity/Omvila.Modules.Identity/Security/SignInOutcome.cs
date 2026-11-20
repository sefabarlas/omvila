namespace Omvila.Modules.Identity.Security;

/// <summary>Doğrulanmış telefonun sonucu: oturum (SignedIn) veya kayıt jetonu (SignupRequired).</summary>
internal sealed record SignInOutcome(ManageSession? Session, string? SignupToken);
