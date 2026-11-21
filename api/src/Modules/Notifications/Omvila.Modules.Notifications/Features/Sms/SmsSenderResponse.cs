using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Features.Sms;

/// <summary>Sözleşme SmsSender; hiç başvuru yoksa durum None, ad null, SMS'ler varsayılan adla.</summary>
internal sealed record SmsSenderResponse(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Name,
    string Status,
    string EffectiveName,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? SubmittedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? DecidedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? RejectionReason)
{
    public static SmsSenderResponse From(BrandSmsSender? sender, string defaultSender) =>
        sender is null
            ? new(null, nameof(SmsSenderStatus.None), defaultSender, null, null, null)
            : new(sender.Name, sender.Status.ToString(), BrandSmsSender.EffectiveName(sender, defaultSender), sender.SubmittedUtc, sender.DecidedUtc,
                sender.RejectionReason);
}
