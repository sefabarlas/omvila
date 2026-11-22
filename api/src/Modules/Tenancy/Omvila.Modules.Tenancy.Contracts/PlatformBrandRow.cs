using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Platform paneli marka satırı (Tenancy alanları).</summary>
/// <param name="Id">Marka.</param>
/// <param name="Name">Ad.</param>
/// <param name="Slug">Bağlantı adı.</param>
/// <param name="IndustryCode">Sektör.</param>
/// <param name="Status">Onboarding, Active, Suspended, Archived.</param>
/// <param name="City">Ana (ilk açılan etkin) şubenin ili; şube yoksa null.</param>
/// <param name="CreatedUtc">Kayıt anı.</param>
/// <param name="BranchCount">Etkin şube sayısı.</param>
/// <param name="ActiveStaffCount">Etkin personel sayısı.</param>
/// <param name="DepositEnabled">"Kapora açık": etkin kapora IBAN hesabı var.</param>
/// <param name="HasWorkingHours">Çalışma saatlerini girdi (aktivasyon hunisi).</param>
/// <param name="Version">Marka satırının sürümü (xmin).</param>
public sealed record PlatformBrandRow(
    Guid Id,
    string Name,
    string Slug,
    string IndustryCode,
    string Status,
    string? City,
    Instant CreatedUtc,
    int BranchCount,
    int ActiveStaffCount,
    bool DepositEnabled,
    bool HasWorkingHours,
    uint Version);
