using NodaTime;

namespace Omvila.Modules.Identity.Domain;

/// <summary>Kullanım koşulları ve aydınlatma metni onayı (alan-modeli.md § TermsAcceptance). Kayıtta zorunludur; satırlar değişmez.</summary>
internal sealed class TermsAcceptance
{
    private TermsAcceptance()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public LegalDocumentType DocumentType { get; private set; }

    public string DocumentVersion { get; private set; } = string.Empty;

    public Instant AcceptedUtc { get; private set; }

    public string? IpAddress { get; private set; }

    public static TermsAcceptance Record(Guid id, Guid userId, LegalDocumentType type, string version, string? ipAddress, Instant now) => new()
    {
        Id = id,
        UserId = userId,
        DocumentType = type,
        DocumentVersion = version,
        AcceptedUtc = now,
        IpAddress = ipAddress,
    };
}
