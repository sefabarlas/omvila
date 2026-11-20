namespace Omvila.Modules.Tenancy.Domain;

/// <summary>durum-makinesi.md: Onboarding (kurulum adımları bitmedi) → Active → Suspended / Archived.</summary>
internal enum BrandStatus
{
    Onboarding,
    Active,
    Suspended,
    Archived,
}
