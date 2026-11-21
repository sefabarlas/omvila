using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Infrastructure;

/// <summary>
/// <see cref="IServiceDeposits"/>: kalem başına <see cref="DepositResolver.Resolve"/>. Kanal Online değilse veya kalem yoksa
/// veritabanına gitmez; aksi hâlde hizmet kuralları tek sorguda, marka politikası ve eşik Tenancy'den okunur.
/// </summary>
internal sealed class ServiceDeposits(CatalogDbContext db, ITenantQueries tenants) : IServiceDeposits
{
    // Kural: sureler-ve-ayarlar.md § Kapora ("Bir kalem için kapora çözümü"), durum-makinesi.md § Oluşturma kuralı
    public async Task<IReadOnlyList<Money?>> ResolveAsync(Guid brandId, ServiceDepositRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!request.IsOnlineChannel || request.Items.Count == 0)
        {
            return [.. request.Items.Select(_ => (Money?)null)];
        }

        var settings = await tenants.GetDepositSettingsAsync(brandId, ct) ?? throw new DomainException("Markanın kapora ayarları yok.");
        var policy = Enum.TryParse<BrandDepositPolicy>(settings.DepositPolicy, ignoreCase: false, out var parsed) ? parsed : BrandDepositPolicy.None;
        var serviceIds = request.Items.Select(i => i.ServiceId).Distinct().ToList();
        var services = await db.Services.AsNoTracking()
            .Where(s => s.BrandId == brandId && serviceIds.Contains(s.Id))
            .ToListAsync(ct);
        var rules = services.ToDictionary(s => s.Id, s => s.Deposit);

        return [.. request.Items.Select(item => DepositResolver.Resolve(
            rules.GetValueOrDefault(item.ServiceId) ?? DepositRule.Inherit,
            item.Price,
            new DepositContext(
                settings.HasActiveBankAccount, IsOnlineChannel: true, item.IsPackageRedemption, policy, request.CustomerNoShowCount,
                settings.RiskyCustomerNoShowThreshold)))];
    }
}
