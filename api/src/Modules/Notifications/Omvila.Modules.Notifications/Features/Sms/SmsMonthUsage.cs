using NodaTime;

namespace Omvila.Modules.Notifications.Features.Sms;

/// <summary>Sözleşme SmsOverview.sentThisMonth ("Bu ay gönderilen 862 SMS · 1.904 bildirim").</summary>
internal sealed record SmsMonthUsage(LocalDate Month, int SmsCount, int CreditsUsed, int NotificationCount);
