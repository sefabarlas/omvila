namespace Omvila.Modules.Scheduling.Domain.Waitlist;

/// <summary>"Hangi saatler size uyar?" (sözleşme WaitlistTimeWindow); sınırlar <see cref="WaitlistPreferences.WindowOf"/>.</summary>
internal enum WaitlistTimeWindow
{
    Morning,
    Afternoon,
    Evening,
}
