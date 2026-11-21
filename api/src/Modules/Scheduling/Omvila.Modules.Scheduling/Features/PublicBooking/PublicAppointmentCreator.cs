using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Online randevu (sözleşme publicCreateAppointment; dilim-02 § Alan kuralları, kabul kriterleri 1, 2, 4, 6). Sıra: biçim
/// → personel seçimi ayarları → müşteri (telefonla bul/oluştur: kart Source=Online, kişi IsVerified) → engelli müşteri 403
/// → <see cref="AppointmentWriter"/> (public kip; sunulmayan her saat slot_taken + meta.alternatives) → rıza kayıtları
/// (aydınlatma metni okundu, isteğe bağlı kampanya izni) → outbox olayı. Hata olursa transaction (müşteri kartı dahil) geri
/// alınır. Onay gerekiyorsa (hizmet ?? marka <c>RequiresApproval</c>, kalemlerden biri yeter) randevu Onay bekliyor başlar,
/// tutma şimdi + <c>ApprovalTimeoutMinutes</c>, olay <see cref="AppointmentRequested"/>; değilse Onaylandı ve
/// <see cref="AppointmentConfirmed"/> (dilim-03 kabul kriteri 1). Markanın aktif kapora hesabı varsa kalem kaporaları yazma
/// yolunda çözülür (müşterinin gelmeme sayısıyla; dilim-04 kabul kriteri 2): kapora varsa randevu Kapora bekleniyor başlar,
/// tutma şimdi + havale süresi, olay yine <see cref="AppointmentRequested"/> (müşteriye "Talebiniz alındı" yerine
/// <c>deposit.awaiting</c> gider; T-81).
/// </summary>
internal sealed class PublicAppointmentCreator(
    AppointmentWriter writer,
    ICustomerDirectory customers,
    IPublicBookingQueries booking,
    ITenantQueries tenants,
    IBrandBankAccounts bankAccounts,
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

        var settings = await tenants.GetAppointmentSettingsAsync(brandId, ct) ?? throw new DomainException("Markanın randevu ayarları yok.");
        var slot = new SlotRequest(context.Scope.BranchId, body.StartUtc!.Value, items.Value, sameStaff, AvailabilityMode.Public);
        var deposit = await DepositTermsAsync(bankAccounts, brandId, customer.NoShowCount, ct);
        var written = await writer.CreateAsync(
            new CreateAppointmentCommand(
                brandId, slot, customer.CustomerId, customer.BrandCustomerId, AppointmentChannel.Online, context.ClientApp, body.CustomerNote,
                StaffNote: null, SeriesId: null, CreatedByUserId: null, settings.Policy(), deposit),
            ct);
        if (!written.IsSuccess)
        {
            return await WithAlternativesAsync(written.Error, brandId, slot, customer.CustomerId, ct);
        }

        await RecordConsentsAsync(context, body, customer.CustomerId, ct);
        await PublishCreatedAsync(outbox, written.Value.Appointment, ct);
        return written.Value;
    }

    /// <summary>
    /// Online oluşan randevunun olayı (rezervasyon sayfası ve bekleme teklifi kabulü; W4). Onay veya kapora gerekiyorsa
    /// <see cref="AppointmentRequested"/> ("Talebiniz alındı", kaporada deposit.awaiting, staff.approval_needed; onay mesajı ve
    /// hatırlatmalar onayda veya kapora gelince), değilse <see cref="AppointmentConfirmed"/>.
    /// </summary>
    internal static async Task PublishCreatedAsync(IOutbox outbox, Appointment appointment, CancellationToken ct)
    {
        if (appointment.Status == AppointmentStatus.PendingApproval)
        {
            await outbox.EnqueueAsync(ApprovalEvents.Requested(appointment, isReschedule: false), ct);
            return;
        }

        await outbox.EnqueueAsync(
            new AppointmentConfirmed(
                appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc,
                appointment.Channel.ToString(), NotifyCustomer: true, SeriesId: null, IsSeriesStart: false),
            ct);
    }

    /// <summary>Kural: sureler-ve-ayarlar.md § Kapora — aktif hesap yoksa kapora yok; varsa kalem kaporaları yazma yolunda çözülür.</summary>
    internal static async Task<DepositTerms?> DepositTermsAsync(IBrandBankAccounts bankAccounts, Guid brandId, int customerNoShowCount, CancellationToken ct)
    {
        var account = await bankAccounts.GetActiveAsync(brandId, ct);
        return account is null
            ? null
            : new DepositTerms(customerNoShowCount, account.TransferCodePrefix, Duration.FromMinutes(account.TransferWindowMinutes));
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
