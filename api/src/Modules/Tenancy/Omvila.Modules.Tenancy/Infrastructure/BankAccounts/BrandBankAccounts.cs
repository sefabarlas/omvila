using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Tenancy.Infrastructure.BankAccounts;

/// <summary><see cref="IBrandBankAccounts"/>: kapora hesabını okur, IBAN'ı yalnız istendiğinde çözer.</summary>
internal sealed class BrandBankAccounts(TenancyDbContext db, IbanProtector protector) : IBrandBankAccounts
{
    public Task<bool> HasActiveAsync(Guid brandId, CancellationToken ct) =>
        db.BankAccounts.AsNoTracking().AnyAsync(a => a.BrandId == brandId && a.IsActive, ct);

    public async Task<ActiveBankAccount?> GetActiveAsync(Guid brandId, CancellationToken ct)
    {
        var account = await db.BankAccounts.AsNoTracking().SingleOrDefaultAsync(a => a.BrandId == brandId && a.IsActive, ct);
        return account is null
            ? null
            : new ActiveBankAccount(
                account.AccountHolderName,
                account.BankName,
                protector.Unprotect(brandId, account.IbanEncrypted),
                account.TransferCodePrefix,
                account.TransferWindowMinutes);
    }
}
