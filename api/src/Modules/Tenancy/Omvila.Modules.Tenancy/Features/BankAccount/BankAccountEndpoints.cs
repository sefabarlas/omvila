using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.Modules.Tenancy.Infrastructure.BankAccounts;
using Omvila.SharedKernel;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.BankAccount;

/// <summary>
/// Kapora hesabı uçları (sözleşme manageGetBankAccount / manageUpdateBankAccount; AyarlarOdeme). Yalnız settings.payment (Owner).
/// Markada tek hesap; hesap yokken GET 200 configured=false ve ETag W/"0", ilk PUT If-Match W/"0" ile oluşturur.
/// Tam IBAN bu uçlardan hiç dönmez (maske + son 4); IBAN ve alıcı adı günlüğe yazılmaz.
/// </summary>
internal static class BankAccountEndpoints
{
    // Hesap yokken sürüm (sözleşme: ETag W/"0"). PostgreSQL xmin 0 olamaz; var olan satırla karışmaz.
    private const uint MissingVersion = 0;

    public static void Map(IEndpointRouteBuilder app)
    {
        var brand = app.MapApiGroup("/api/v1/manage/brand", Audiences.Manage);
        brand.MapGet("/bank-account", GetAsync).WithName("manageGetBankAccount").RequirePermission(Permissions.Settings.Payment);
        brand.MapPut("/bank-account", UpdateAsync).WithName("manageUpdateBankAccount")
            .RequirePermission(Permissions.Settings.Payment).RequireIfMatch();
    }

    private static async Task<IResult> GetAsync(
        HttpContext http, ITenantContext tenant, TenancyDbContext db, IbanProtector protector, CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var account = await db.BankAccounts.AsNoTracking().SingleOrDefaultAsync(a => a.BrandId == brandId, ct);
        return account is null
            ? Versioned(http, MissingVersion, BankAccountResponse.Missing(MissingVersion))
            : Versioned(http, account.Version, BankAccountResponse.From(account, protector.Unprotect(brandId, account.IbanEncrypted)));
    }

    private static async Task<IResult> UpdateAsync(
        BankAccountWriteRequest body, HttpContext http, ITenantContext tenant, TenancyDbContext db, IbanProtector protector,
        IClock clock, CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var expected = http.GetIfMatchVersion();
        var account = await db.BankAccounts.SingleOrDefaultAsync(a => a.BrandId == brandId, ct);
        if (account is null ? expected != MissingVersion : !Matches(db, account, account.Version, expected))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var errors = BankAccountRules.Validate(
            body.AccountHolderName, body.BankName, body.TransferCodePrefix, body.TransferWindowMinutes, body.Iban,
            isFirstSave: account is null, out var iban);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var values = new BankAccountValues(body.AccountHolderName!, body.BankName!, body.TransferCodePrefix!, body.TransferWindowMinutes!.Value);
        var encrypted = iban is null ? null : protector.Protect(brandId, iban);
        var now = clock.GetCurrentInstant();
        if (account is null)
        {
            account = BrandBankAccount.Create(brandId, values, encrypted!, now);
            db.BankAccounts.Add(account);
        }
        else
        {
            account.Update(values, encrypted, now);
            db.Entry(account).State = EntityState.Modified;
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Aynı anda iki ilk kayıt (ikisi de W/"0"): ikincisi eşzamanlı düzenleme çakışmasıdır.
            return Error.PreconditionFailed().ToProblem();
        }

        var saved = iban ?? protector.Unprotect(brandId, account.IbanEncrypted);
        return Versioned(http, account.Version, BankAccountResponse.From(account, saved));
    }
}
