using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog;
using Omvila.Modules.Sales;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Scheduling.IntegrationTests;

/// <summary>
/// Müsaitlik bütünleşik testlerinin verisi (tasarım verisi, Kuaför Elif): Merkez (Kadıköy rolünde) ve Nişantaşı; Merkez'de
/// pazartesi–cumartesi 10:00–20:00 çalışan 12 personel (ilk ikisi Elif ve Mert; Mert Nişantaşı'nda da çalışır), Saç kesimi
/// (45 dk, 12 personel) ve Boya (45 + 40 bekleme + 35, yalnız Elif, özel süre 130). Saat TestClock: Çar 25 Kas 2026 12:00.
/// </summary>
internal sealed class AvailabilityData
{
    public const int StaffCount = 12;

    /// <summary>Perşembe 26 Kasım 2026.</summary>
    public static readonly LocalDate Thursday = new(2026, 11, 26);

    public static readonly LocalDate Sunday = new(2026, 11, 29);

    public static readonly DateTimeZone Istanbul = DateTimeZoneProviders.Tzdb["Europe/Istanbul"];

    private readonly PostgresFixture _db;
    private readonly ServiceProvider _services;

    private AvailabilityData(PostgresFixture db, ServiceProvider services)
    {
        _db = db;
        _services = services;
    }

    public Guid BrandId { get; private set; }

    public Guid Merkez { get; private set; }

    public Guid Nisantasi { get; private set; }

    public Guid InactiveStaff { get; private set; }

    public IReadOnlyList<Guid> Staff { get; private set; } = [];

    public Guid Elif => Staff[0];

    public Guid Mert => Staff[1];

    public Guid Haircut { get; private set; }

    public Guid Color { get; private set; }

    public static ServiceProvider CreateServices(PostgresFixture db, IInterceptor? interceptor = null, Action<IServiceCollection>? configure = null) =>
        db.CreateServices(configure: s =>
        {
            s.AddTenancyModule(db.Configuration()).AddCatalogModule().AddSalesModule().AddSchedulingModule(db.Configuration());
            if (interceptor is not null)
            {
                s.AddSingleton(interceptor);
            }

            configure?.Invoke(s);
        });

    public static async Task<AvailabilityData> SeedAsync(PostgresFixture db, ServiceProvider services, CancellationToken ct)
    {
        var data = new AvailabilityData(db, services);
        await data.SeedAsync(ct);
        return data;
    }

    public static Instant At(LocalDate date, int hour, int minute) =>
        Istanbul.AtStrictly(date.At(new LocalTime(hour, minute))).ToInstant();

    /// <summary>Markanın kiracı bağlamında, açık transaction'da çalıştırır (geri alınır).</summary>
    public async Task<T> InBrandAsync<T>(Func<IServiceProvider, Task<T>> action, CancellationToken ct, bool commit = false)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(BrandId), ct);
        await session.BeginAsync(ct);
        var result = await action(scope.ServiceProvider);
        if (commit)
        {
            await session.CommitAsync(ct);
        }
        else
        {
            await session.RollbackAsync(ct);
        }

        return result;
    }

    /// <summary>Saç kesimi (45 dk) randevusu yazar.</summary>
    public Task AddAppointmentsAsync(IEnumerable<(Guid Staff, Guid Branch, Instant Start)> appointments, CancellationToken ct) =>
        InBrandAsync(
            async sp =>
            {
                var context = sp.GetRequiredService<SchedulingDbContext>();
                var ids = sp.GetRequiredService<IIdGenerator>();
                foreach (var (staff, branch, start) in appointments)
                {
                    var plan = AppointmentPlanner.Plan(start, [HaircutOffer(staff)]).Value;
                    context.Appointments.Add(Appointment.Create(
                        new NewAppointment(BrandId, branch, ids.New(), ids.New(), AppointmentChannel.Phone, null, null, null, null, null),
                        plan,
                        ids,
                        start - Duration.FromDays(3)));
                }

                await context.SaveChangesAsync(ct);
                return true;
            },
            ct,
            commit: true);

    private ItemOffer HaircutOffer(Guid staff) =>
        new(Haircut, staff, "Saç kesimi", Money.Try(45_000), Money.Zero(Currency.TRY), 0, 0, [new PlannedStage("Saç kesimi", 45, true)]);

    private async Task SeedAsync(CancellationToken ct)
    {
        var ids = _services.GetRequiredService<IIdGenerator>();
        await using (var scope = _services.CreateAsyncScope())
        {
            var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
            await session.BeginAsync(ct);
            var tenant = await scope.ServiceProvider.GetRequiredService<ITenantProvisioning>()
                .ProvisionBrandAsync("Kuaför Elif " + ids.New().ToString("N")[..8], "hair_salon", ids.New(), null, ct);
            await session.CommitAsync(ct);
            BrandId = tenant.BrandId;
            Merkez = tenant.BranchId;
        }

        Nisantasi = ids.New();
        Staff = [.. Enumerable.Range(0, StaffCount).Select(_ => ids.New())];
        InactiveStaff = ids.New();
        Haircut = ids.New();
        Color = ids.New();
        var category = ids.New();

        var sql = new StringBuilder(BaseSql(category));
        foreach (var (staff, index) in Staff.Append(InactiveStaff).Select((s, i) => (s, i)))
        {
            var staffBranch = ids.New();
            var isActive = staff != InactiveStaff;
            sql.Append(CultureInfo.InvariantCulture, $"""
                INSERT INTO tenancy.staff (id, brand_id, full_name, calendar_color, service_commission_percent, sale_commission_percent, is_bookable_online, is_active, created_utc)
                  VALUES ('{staff}', '{BrandId}', 'Personel {index}', '#2F5D8A', 0, 0, true, true, now());
                INSERT INTO tenancy.staff_branches (id, brand_id, staff_id, branch_id, is_primary, is_active)
                  VALUES ('{staffBranch}', '{BrandId}', '{staff}', '{Merkez}', true, {(isActive ? "true" : "false")});
                INSERT INTO tenancy.staff_working_hours (id, brand_id, staff_branch_id, day_of_week, start_time, end_time)
                  SELECT gen_random_uuid(), '{BrandId}', '{staffBranch}', d, '10:00', '20:00' FROM generate_series(1, 6) d;
                INSERT INTO catalog.service_staff (id, brand_id, service_id, staff_id, duration_override_minutes, surcharge_minor, is_active)
                  VALUES (gen_random_uuid(), '{BrandId}', '{Haircut}', '{staff}', NULL, 0, true);

                """);
        }

        // Mert Nişantaşı'nda da çalışır (perşembe); Elif cuma tüm gün izinli.
        sql.Append(CultureInfo.InvariantCulture, $"""
            INSERT INTO tenancy.staff_branches (id, brand_id, staff_id, branch_id, is_primary, is_active)
              VALUES ('{ids.New()}', '{BrandId}', '{Mert}', '{Nisantasi}', false, true);
            INSERT INTO tenancy.time_offs (id, brand_id, staff_id, type, start_utc, end_utc, is_all_day, note, created_utc)
              VALUES (gen_random_uuid(), '{BrandId}', '{Elif}', 'Leave', '{At(Thursday.PlusDays(1), 0, 0)}', '{At(Thursday.PlusDays(2), 0, 0)}', true, NULL, now());
            """);

        await using var connection = await _db.OpenMigratorConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql.ToString(), connection);
        await command.ExecuteNonQueryAsync(ct);
    }

    private string BaseSql(Guid category) => string.Create(CultureInfo.InvariantCulture, $"""
            INSERT INTO tenancy.branches (id, brand_id, name, slug, address_line, district, city, country_code, time_zone_id, is_active, created_utc)
              VALUES ('{Nisantasi}', '{BrandId}', 'Nişantaşı', 'nisantasi', '', '', '', 'TR', 'Europe/Istanbul', true, now());
            INSERT INTO tenancy.branch_working_hours (id, brand_id, branch_id, day_of_week, start_time, end_time)
              SELECT gen_random_uuid(), '{BrandId}', b, d, '10:00', '20:00' FROM generate_series(1, 6) d, unnest(ARRAY['{Merkez}', '{Nisantasi}']::uuid[]) b;
            INSERT INTO tenancy.working_hours_overrides (id, brand_id, branch_id, date, kind, is_closed, intervals, note, created_utc)
              VALUES (gen_random_uuid(), '{BrandId}', '{Merkez}', '{Sunday:yyyy-MM-dd}', 'ExtraOpening', false, ARRAY['11:00-16:00'], NULL, now()),
                     (gen_random_uuid(), '{BrandId}', '{Merkez}', '2027-01-01', 'Holiday', true, ARRAY[]::text[], NULL, now());
            INSERT INTO catalog.service_categories (id, brand_id, name, sort_order, is_active, created_utc) VALUES ('{category}', '{BrandId}', 'Saç', 1, true, now());
            INSERT INTO catalog.services (id, brand_id, category_id, name, duration_minutes, base_price_minor, currency_code, price_display,
                buffer_before_minutes, buffer_after_minutes, deposit_mode, is_online_bookable, min_notice_minutes, sort_order, is_active, created_utc)
              VALUES ('{Haircut}', '{BrandId}', '{category}', 'Saç kesimi', 45, 45000, 'TRY', 'Fixed', 0, 0, 'Inherit', true, NULL, 1, true, now()),
                     ('{Color}', '{BrandId}', '{category}', 'Boya', 120, 120000, 'TRY', 'StartingFrom', 0, 0, 'Inherit', true, 30, 2, true, now());
            INSERT INTO catalog.service_stages (id, brand_id, service_id, seq_no, name, duration_minutes, staff_busy) VALUES
              (gen_random_uuid(), '{BrandId}', '{Color}', 1, 'Uygulama', 45, true),
              (gen_random_uuid(), '{BrandId}', '{Color}', 2, 'Bekleme', 40, false),
              (gen_random_uuid(), '{BrandId}', '{Color}', 3, 'Yıkama ve fön', 35, true);
            INSERT INTO catalog.service_staff (id, brand_id, service_id, staff_id, duration_override_minutes, surcharge_minor, is_active)
              VALUES (gen_random_uuid(), '{BrandId}', '{Color}', '{Elif}', 130, 30000, true);

            """);
}
