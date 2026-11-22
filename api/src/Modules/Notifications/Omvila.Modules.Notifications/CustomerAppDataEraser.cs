using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;

namespace Omvila.Modules.Notifications;

/// <summary><see cref="ICustomerAppDataEraser"/>: müşteri bağlamında yalnız müşterinin kendi cihazları ve bildirimleri görünür (RLS).</summary>
internal sealed class CustomerAppDataEraser(NotificationsDbContext db) : ICustomerAppDataEraser
{
    // Kural: sözleşme meRequestAccountDeletion (cihazlar pasifleşir, oturumlar kapanır); T-104
    public async Task EraseAsync(Guid customerId, CancellationToken ct)
    {
        await db.Devices.Where(d => d.BrandId == null && d.OwnerType == Device.CustomerOwner && d.OwnerId == customerId).ExecuteDeleteAsync(ct);
        await db.CustomerNotifications.Where(n => n.CustomerId == customerId).ExecuteDeleteAsync(ct);
    }
}
