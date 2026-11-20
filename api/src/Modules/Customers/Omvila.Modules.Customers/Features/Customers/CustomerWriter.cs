using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>
/// Panelden müşteri ekleme (MusteriEkle; dilim-02 kabul kriteri 15). Numara markada kayıtlıysa 409 phone_already_exists
/// ve meta.brandCustomerId ("Mevcut kartı aç"). Numara başka markada kayıtlıysa global kişi yeniden kullanılır; o
/// markanın verisi (ad, doğum yılı, not) ne okunur ne değişir.
/// </summary>
internal sealed class CustomerWriter(CustomersDbContext db, CustomerRegistry registry, IIdGenerator ids, IClock clock)
{
    public const string BrandCustomerIdMeta = "brandCustomerId";

    public async Task<Result<Guid>> CreateAsync(Guid brandId, CustomerCreateRequest body, CustomerViewContext view, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        var validated = CustomerRules.ValidateNew(body.FullName, body.Phone, body.BirthYear, body.Notes, view.Today.Year);
        if (!validated.IsSuccess)
        {
            return validated.Error;
        }

        var input = validated.Value;
        var existing = await registry.FindByPhoneAsync(input.Phone, ct);
        if (existing is not null && await registry.FindCardAsync(brandId, existing.Id, ct) is { } card)
        {
            return Error.Conflict(ErrorCodes.PhoneAlreadyExists, new Dictionary<string, object?>(StringComparer.Ordinal) { [BrandCustomerIdMeta] = card.Id });
        }

        // Panelden eklenen numara doğrulanmamıştır (IsVerified yalnız OTP ile).
        var customer = existing ?? await registry.FindOrCreateAsync(input.Phone, input.FullName, verified: false, ct);
        var created = BrandCustomer.Create(
            ids.New(), brandId, customer.Id, input.FullName, input.BirthYear, input.Notes, BrandCustomerSource.Panel,
            body.SendAppointmentSms ?? true, clock.GetCurrentInstant());
        db.BrandCustomers.Add(created);
        await db.SaveChangesAsync(ct);
        return created.Id;
    }
}
