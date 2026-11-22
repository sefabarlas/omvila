namespace Omvila.Modules.Platform.Domain;

/// <summary>Eşleşme kararı (sözleşme MatchVerdict): "Kod tutuyor" veya "Kontrol edin".</summary>
internal enum MatchVerdict
{
    CodeMatches,
    CheckRequired,
}
