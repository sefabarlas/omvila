using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>Havale bildiriminin amacı (sözleşme PaymentNoticePurpose).</summary>
internal enum PaymentNoticePurpose
{
    Subscription,
    Credits,
}
