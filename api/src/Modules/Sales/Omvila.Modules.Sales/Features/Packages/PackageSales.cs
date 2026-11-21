using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>
/// Paket satışı ve iadesi (sözleşme manageSellPackage, manageRefundCustomerPackage; dilim-05 kabul kriteri 6). Satış tek
/// transaction'da CustomerPackage(Active) + Payment (yöntem, paket fiyatı, CustomerPackage tahsisi) + satan personele
/// CommissionEntry(Sale, Accrued) yazar. İade paketi Refunded yapar, satış tahsilatını iade işaretler ve satış primine iade
/// oranında ters kayıt açar.
/// </summary>
internal sealed class PackageSales(
    SalesDbContext db, ICustomerDirectory customers, ITenantQueries tenants, IIdGenerator ids, IClock clock)
{
    // Kural: sözleşme manageSellPackage; alan-modeli.md § CustomerPackage, § CommissionEntry (satış primi)
    public async Task<Result<CustomerPackage>> SellAsync(
        Guid brandId, Guid brandCustomerId, PackageSaleRequest body, PackageScope scope, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(scope);
        if ((await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct)).Count == 0)
        {
            return Error.NotFound();
        }

        var errors = Validate(body);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var branchId = body.BranchId!.Value;
        if (!scope.Branches.Includes(branchId) || !await tenants.BranchExistsAsync(brandId, branchId, ct))
        {
            return Error.NotFound();
        }

        var package = await db.Packages.Include(p => p.Items).SingleOrDefaultAsync(p => p.Id == body.PackageId && p.BrandId == brandId, ct);
        if (package is null)
        {
            return Error.NotFound();
        }

        var allowed = package.CanSell();
        if (!allowed.IsSuccess)
        {
            return allowed.Error;
        }

        var staffId = body.SoldByStaffId!.Value;
        if (await tenants.GetBranchSellerCommissionPercentAsync(brandId, staffId, branchId, ct) is not { } staffPercent)
        {
            return Error.Validation("soldByStaffId", "out_of_range");
        }

        var now = clock.GetCurrentInstant();
        var today = now.InZone(await BranchCalendar.ZoneAsync(tenants, brandId, branchId, ct)).Date;
        var sold = CustomerPackage.Sell(ids, package, branchId, brandCustomerId, staffId, today, userId, now);
        if (!sold.IsSuccess)
        {
            return sold.Error;
        }

        var customerPackage = sold.Value;
        db.CustomerPackages.Add(customerPackage);
        db.Payments.Add(Payment.RecordPackageSale(
            ids, brandId, branchId, brandCustomerId, customerPackage.Id, AppointmentPayments.ToDomain(Enum.Parse<CollectedPaymentMethod>(body.PaymentMethod!)),
            package.Price, userId, now));
        var percent = CommissionEntry.SalePercent(package.SaleCommissionPercentOverride, staffPercent);
        if (percent > 0)
        {
            db.CommissionEntries.Add(CommissionEntry.AccrueSale(ids, brandId, branchId, staffId, customerPackage.Id, package.Price, percent, today, now));
        }

        await db.SaveChangesAsync(ct);
        return customerPackage;
    }

    // Kural: sözleşme manageRefundCustomerPackage; durum-makinesi.md § 5, § 6, § 7; S-10 (tutar elle, W9)
    public async Task<Result<CustomerPackage>> RefundAsync(
        Guid brandId, Guid customerPackageId, PackageRefundRequest body, PackageScope scope, uint expectedVersion, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(scope);
        var package = await db.CustomerPackages.Include(p => p.Items).SingleOrDefaultAsync(p => p.Id == customerPackageId && p.BrandId == brandId, ct);
        if (package is null || !scope.Covers(package))
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(package, package.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var amount = MoneyInput.Read(body.Amount, "amount", package.PricePaid.Currency, errors);
        if (amount is null)
        {
            return Error.Validation(errors);
        }

        var now = clock.GetCurrentInstant();
        var refunded = package.Refund(amount.Value, body.Note, userId, now);
        if (!refunded.IsSuccess)
        {
            return refunded.Error;
        }

        var payment = await (from a in db.PaymentAllocations
                             join p in db.Payments on a.PaymentId equals p.Id
                             where a.BrandId == brandId && a.TargetType == AllocationTargetType.CustomerPackage && a.TargetId == package.Id
                             select p).SingleAsync(ct);
        payment.MarkRefunded(amount.Value);

        var sale = await db.CommissionEntries.SingleOrDefaultAsync(
            c => c.BrandId == brandId && c.CustomerPackageId == package.Id && c.Type == CommissionType.Sale && c.ReversesEntryId == null, ct);
        if (sale is not null)
        {
            var today = now.InZone(await BranchCalendar.ZoneAsync(tenants, brandId, package.BranchId, ct)).Date;
            db.CommissionEntries.Add(sale.Reverse(ids, Math.Min(amount.Value.AmountMinor, sale.BasisMinor), today, now));
        }

        await db.SaveChangesAsync(ct);
        return package;
    }

    private static Dictionary<string, string[]> Validate(PackageSaleRequest body)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.PackageId is null)
        {
            errors["packageId"] = ["required"];
        }

        if (body.BranchId is null)
        {
            errors["branchId"] = ["required"];
        }

        if (body.SoldByStaffId is null)
        {
            errors["soldByStaffId"] = ["required"];
        }

        if (body.PaymentMethod is null)
        {
            errors["paymentMethod"] = ["required"];
        }
        else if (!Enum.GetNames<CollectedPaymentMethod>().Contains(body.PaymentMethod, StringComparer.Ordinal))
        {
            errors["paymentMethod"] = ["invalid_format"];
        }

        return errors;
    }
}
