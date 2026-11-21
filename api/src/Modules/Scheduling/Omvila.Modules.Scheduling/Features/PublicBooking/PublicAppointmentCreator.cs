using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Online randevu (sözleşme publicCreateAppointment; dilim-02 § Alan kuralları, kabul kriterleri 1, 2, 4, 6). Sıra: biçim
/// → personel seçimi ayarları → müşteri (telefonla bul/oluştur: kart Source=Online, kişi IsVerified) → engelli müşteri 403
/// → <see cref="AppointmentWriter"/> (public kip; sunulmayan her saat slot_taken + meta.alternatives) → rıza kayıtları
/// (aydınlatma metni okundu, isteğe bağlı kampanya izni) → <see cref="AppointmentConfirmed"/> outbox olayı. Hata olursa
/// transaction (müşteri kartı dahil) geri alınır. Onay gerektiren salon Dilim 3, kapora Dilim 4: D2'de randevu Onaylandı başlar.
/// </summary>
internal sealed class PublicAppointmentCreator(
    AppointmentWriter writer,
    ICustomerDirectory customers,
    IPublicBookingQueries booking,
    IOutbox outbox,
    IClock clock)
{
    private const int NameMinLength = 2;
    private const int NameMaxLength = 120;
    private const int CustomerNoteMaxLength = 500;
    private const int DocumentVersionMaxLength = 20;
    private const int AlternativeCount = 3;

    // Kural: durum-makinesi.md A1, dilim-02 § Alan kuralları (online oluşturma), guvenlik-kvkk.md § Rıza
    public async Task<Result<AppointmentWriteResult>> CreateAsync(PublicBookingRequest context, PublicAppointmentCreate body, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(body);
        var brandId = context.Scope.BrandId;
        var sameStaff = body.SameStaffForAll ?? true;
        var items = PublicStaffSelection.Read(body.Items, sameStaff, await booking.GetBookingOptionsAsync(brandId, ct));
        var errors = Validate(body);
        if (!items.IsSuccess)
        {
            foreach (var (field, codes) in items.Error.FieldErrors)
            {
                errors.TryAdd(field, codes);
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var customer = await customers.EnsureBrandCustomerAsync(
            new EnsureBrandCustomerRequest(brandId, context.Phone, body.FullName!.Trim(), CustomerSource.Online, PhoneVerified: true), ct);
        if (customer.IsBlocked)
        {
            return Error.Forbidden(ErrorCodes.CustomerBlocked);
        }

        var slot = new SlotRequest(context.Scope.BranchId, body.StartUtc!.Value, items.Value, sameStaff, AvailabilityMode.Public);
        var written = await writer.CreateAsync(
            new CreateAppointmentCommand(
                brandId, slot, customer.CustomerId, customer.BrandCustomerId, AppointmentChannel.Online, context.ClientApp, body.CustomerNote,
                StaffNote: null, SeriesId: null, CreatedByUserId: null),
            ct);
        if (!written.IsSuccess)
        {
            return await WithAlternativesAsync(written.Error, brandId, slot, customer.CustomerId, ct);
        }

        await RecordConsentsAsync(context, body, customer.CustomerId, ct);
        var appointment = written.Value.Appointment;
        await outbox.EnqueueAsync(
            new AppointmentConfirmed(
                brandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc,
                appointment.Channel.ToString(), NotifyCustomer: true, SeriesId: null, IsSeriesStart: false),
            ct);
        return written.Value;
    }

    /// <summary>
    /// Aydınlatma metni okundu (zorunlu) ve kampanya izni (verildiyse SMS ve e-posta; "Salonun kampanyalarından SMS ve
    /// e-posta"). Rıza markaya özeldir; işaretlenmeyen izin geri çekme sayılmaz, satır yazılmaz.
    /// </summary>
    private async Task RecordConsentsAsync(PublicBookingRequest context, PublicAppointmentCreate body, Guid customerId, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var version = body.PrivacyNoticeVersion!.Trim();
        var types = new List<ConsentType> { ConsentType.PrivacyNoticeAcknowledged };
        if (body.MarketingConsent == true)
        {
            types.AddRange([ConsentType.MarketingSms, ConsentType.MarketingEmail]);
        }

        foreach (var type in types)
        {
            await customers.RecordConsentAsync(
                new ConsentEntry(customerId, context.Scope.BrandId, type, IsGranted: true, version, context.ConsentChannel, context.IpAddress, now), ct);
        }
    }

    private async Task<Error> WithAlternativesAsync(Error error, Guid brandId, SlotRequest slot, Guid customerId, CancellationToken ct)
    {
        if (!string.Equals(error.Code, ErrorCodes.SlotTaken, StringComparison.Ordinal))
        {
            return error;
        }

        var alternatives = await writer.SuggestAlternativesAsync(brandId, slot, AlternativeCount, customerId, ignoreAppointmentId: null, ct);
        return Error.Conflict(ErrorCodes.SlotTaken, AppointmentWriter.AlternativesMeta(alternatives));
    }

    private static Dictionary<string, string[]> Validate(PublicAppointmentCreate body)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.StartUtc is null)
        {
            errors["startUtc"] = ["required"];
        }

        var name = body.FullName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["fullName"] = ["required"];
        }
        else if (name.Length is < NameMinLength or > NameMaxLength)
        {
            errors["fullName"] = ["out_of_range"];
        }

        if (body.CustomerNote is { Length: > CustomerNoteMaxLength })
        {
            errors["customerNote"] = ["too_long"];
        }

        // Paket kullanımı Dilim 5: kullanılabilir paket kalemi yok (publicListMyPackages boş liste).
        if (body.UsePackageItems is { Count: > 0 })
        {
            errors["usePackageItems"] = ["out_of_range"];
        }

        if (body.PrivacyNoticeAcknowledged != true)
        {
            errors["privacyNoticeAcknowledged"] = ["required"];
        }

        var version = body.PrivacyNoticeVersion?.Trim();
        if (string.IsNullOrEmpty(version))
        {
            errors["privacyNoticeVersion"] = ["required"];
        }
        else if (version.Length > DocumentVersionMaxLength)
        {
            errors["privacyNoticeVersion"] = ["too_long"];
        }

        return errors;
    }
}
