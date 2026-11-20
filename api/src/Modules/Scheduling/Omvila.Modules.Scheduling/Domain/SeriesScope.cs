namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Tekrarlayan randevuda değişikliğin kapsamı (TekrarIptal; sözleşme scope: ThisOnly, ThisAndFollowing).</summary>
internal enum SeriesScope
{
    ThisOnly,
    ThisAndFollowing,
}
