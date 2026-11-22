using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;

namespace Omvila.Modules.Notifications;

/// <summary><see cref="ICustomerDevices"/>: müşteri bağlamında yalnız müşterinin kendi cihazları görünür (RLS).</summary>
internal sealed class CustomerDevices(NotificationsDbContext db) : ICustomerDevices
{
    public Task DeactivateAsync(Guid customerId, string pushToken, CancellationToken ct) =>
        db.Devices
            .Where(d => d.BrandId == null && d.OwnerType == Device.CustomerOwner && d.OwnerId == customerId && d.PushToken == pushToken && d.IsActive)
            .ExecuteUpdateAsync(set => set.SetProperty(d => d.IsActive, false), ct);
}
