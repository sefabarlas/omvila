using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Müsaitlik motoru (musaitlik.md § Algoritma, § Çok kalemli randevu, § "Fark etmez", § Gün listesi). Saf fonksiyonlar:
/// girdi <see cref="AvailabilitySnapshot"/> ve <see cref="AvailabilityRequest"/>, çıktı aday saatler. Sonuç yalnız
/// öneridir; yazma yolu aynı başlangıcı kilit altında <see cref="Evaluate"/> ile yeniden doğrular.
/// </summary>
internal static class AvailabilityEngine
{
    /// <summary>Bir günün saat ızgarası: şube açıkken her ızgara saati aday veya dolu.</summary>
    public static DaySlots GetSlots(AvailabilitySnapshot snapshot, AvailabilityRequest request, LocalDate date)
    {
        var run = new Run(snapshot, request);
        var open = run.BranchOpen(date);
        var slots = new List<SlotCandidate>();
        var unavailable = new List<LocalTime>();
        if (run.HasEligibleStaff)
        {
            foreach (var point in run.Grid(date))
            {
                if (!open.Contains(point.Instant))
                {
                    continue;
                }

                var candidate = run.InWindow(point.Instant, date) ? run.FindStrict(point.Instant, point.Local, date) : null;
                if (candidate is null)
                {
                    unavailable.Add(point.Local);
                }
                else
                {
                    slots.Add(candidate);
                }
            }
        }
        else
        {
            unavailable.AddRange(run.Grid(date).Where(p => open.Contains(p.Instant)).Select(p => p.Local));
        }

        return new DaySlots(date, open.IsEmpty, slots, unavailable);
    }

    /// <summary>Gün şeridi: her gün için ilk aday aranır, bulununca o günün hesabı durur (musaitlik.md § Gün listesi).</summary>
    public static IReadOnlyList<DayAvailability> GetDays(AvailabilitySnapshot snapshot, AvailabilityRequest request, LocalDate from, LocalDate to)
    {
        if (to < from)
        {
            throw new ArgumentOutOfRangeException(nameof(to), "Bitiş başlangıçtan önce olamaz.");
        }

        var run = new Run(snapshot, request);
        var days = new List<DayAvailability>();
        for (var date = from; date <= to; date = date.PlusDays(1))
        {
            var open = run.BranchOpen(date);
            SlotCandidate? first = null;
            if (!open.IsEmpty && run.HasEligibleStaff && run.DateInWindow(date))
            {
                foreach (var point in run.Grid(date))
                {
                    if (open.Contains(point.Instant) && run.InWindow(point.Instant, date) && run.FindStrict(point.Instant, point.Local, date) is { } candidate)
                    {
                        first = candidate;
                        break;
                    }
                }
            }

            days.Add(new DayAvailability(date, open.IsEmpty, first));
        }

        return days;
    }

    /// <summary>
    /// Belirli bir başlangıcı değerlendirir (yazma yolunun kilit altındaki yeniden doğrulaması, panel uyarıları).
    /// Personel sabitse aday, yalnız uyarı sınıfında ihlal olsa da döner; fark etmez kalemde boş personel yoksa aday yoktur.
    /// </summary>
    public static SlotEvaluation Evaluate(AvailabilitySnapshot snapshot, AvailabilityRequest request, Instant start)
    {
        var run = new Run(snapshot, request);
        var local = start.InZone(snapshot.Zone);
        var date = local.Date;
        var violations = run.WindowViolations(start, local.LocalDateTime);
        violations |= run.EligibilityViolations();
        if ((violations & (SlotViolation.NoEligibleStaff | SlotViolation.NotBookableOnline)) != SlotViolation.None)
        {
            return new SlotEvaluation(start, violations, null);
        }

        if (run.FindStrict(start, local.TimeOfDay, date) is { } strict)
        {
            return new SlotEvaluation(start, violations, strict);
        }

        if (run.FixedChain(start, local.TimeOfDay, date) is { } fixedChain)
        {
            return new SlotEvaluation(start, violations | fixedChain.Violations, fixedChain.Candidate);
        }

        return new SlotEvaluation(start, violations | SlotViolation.NoStaffAvailable, null);
    }

    /// <summary>Başlangıç her kısıta uyuyor mu (PR-5 yazma yolu kilit altında çağırır).</summary>
    public static bool IsSlotAvailable(AvailabilitySnapshot snapshot, AvailabilityRequest request, Instant start) =>
        Evaluate(snapshot, request, start).IsAvailable;

    /// <summary>
    /// Personelin o günkü yerel saatleri (takvimdeki çalışma saati gösterimi de bunu kullanır). Kural: musaitlik.md M7,
    /// dilim-02 kabul kriteri 16 ("Yalnızca bu gün için aç"): şube haftalık düzende kapalı olduğu bir günü özel günle
    /// açmışsa, o hafta günü için saati ve o güne ezmesi olmayan personel şubenin özel gün saatlerinde çalışıyor sayılır.
    /// O gün saati olan personelde normal kesişim geçerlidir; izin ve meşguliyet her durumda çıkarılır (T-48).
    /// </summary>
    public static IReadOnlyList<LocalTimeRange> StaffHoursOn(AvailabilitySnapshot snapshot, StaffSchedule staff, LocalDate date)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(staff);
        var ranges = staff.Hours.RangesOn(date);
        var isExtraOpening = ranges.Count == 0
            && !staff.Hours.Overrides.ContainsKey(date)
            && snapshot.BranchHours.Overrides.TryGetValue(date, out var branchOverride) && branchOverride.Count > 0
            && !(snapshot.BranchHours.Days.TryGetValue(date.DayOfWeek, out var weekly) && weekly.Count > 0);
        return isExtraOpening ? snapshot.BranchHours.Overrides[date] : ranges;
    }

    /// <summary>Bir hesabın önbellekleri (şube günü, personel günü, meşguliyet kümeleri, şablonlar).</summary>
    private sealed class Run
    {
        private readonly AvailabilitySnapshot _s;
        private readonly AvailabilityRequest _r;
        private readonly Dictionary<LocalDate, IntervalSet> _branchOpen = [];
        private readonly Dictionary<LocalDate, IReadOnlyList<GridPoint>> _grid = [];
        private readonly Dictionary<(Guid Staff, LocalDate Date), StaffDay> _staffDays = [];
        private readonly Dictionary<Guid, StaffSets> _staffSets = [];
        private readonly Dictionary<(Guid Service, Guid Staff), ItemTemplate> _templates = [];
        private readonly Dictionary<LocalDate, IReadOnlyList<IReadOnlyList<Guid>>> _orders = [];
        private readonly IReadOnlyList<IReadOnlyList<Guid>> _eligible;
        private readonly bool _hasResourceNeeds;
        private readonly Instant _earliest;
        private readonly LocalDate _lastDate;

        public Run(AvailabilitySnapshot snapshot, AvailabilityRequest request)
        {
            ArgumentNullException.ThrowIfNull(snapshot);
            ArgumentNullException.ThrowIfNull(request);
            _s = snapshot;
            _r = request;
            _eligible = EligibleStaff(onlineOnly: request.Mode == AvailabilityMode.Public);
            _hasResourceNeeds = request.Items.Any(i => snapshot.Services.TryGetValue(i.ServiceId, out var svc) && svc.ResourceNeeds.Count > 0);

            // Kural: musaitlik.md § 7. Hizmet değeri yoksa marka değeri; çok kalemde en kısıtlayıcısı (T-48).
            var services = request.Items.Select(i => snapshot.Services.GetValueOrDefault(i.ServiceId)).ToList();
            var minNotice = services.Max(svc => svc?.MinNoticeMinutes ?? snapshot.Settings.MinNoticeMinutes);
            var maxAdvance = services.Min(svc => svc?.MaxAdvanceDays ?? snapshot.Settings.MaxAdvanceDays);
            _earliest = snapshot.Now + Duration.FromMinutes(minNotice);
            _lastDate = snapshot.Today.PlusDays(maxAdvance);
        }

        private bool IsPublic => _r.Mode == AvailabilityMode.Public;

        public bool HasEligibleStaff => _eligible.All(list => list.Count > 0);

        public IntervalSet BranchOpen(LocalDate date)
        {
            if (!_branchOpen.TryGetValue(date, out var open))
            {
                open = ZoneTime.ToSet(date, _s.BranchHours.RangesOn(date), _s.Zone);
                _branchOpen[date] = open;
            }

            return open;
        }

        public IReadOnlyList<GridPoint> Grid(LocalDate date)
        {
            if (!_grid.TryGetValue(date, out var grid))
            {
                grid = ZoneTime.Grid(date, _s.Zone, _s.Settings.SlotIntervalMinutes);
                _grid[date] = grid;
            }

            return grid;
        }

        /// <summary>Kural: musaitlik.md § 7. Panelde pencere uygulanmaz.</summary>
        public bool InWindow(Instant start, LocalDate date) => !IsPublic || (start >= _earliest && date <= _lastDate);

        public bool DateInWindow(LocalDate date) => !IsPublic || (date <= _lastDate && date >= _earliest.InZone(_s.Zone).Date);

        public SlotViolation WindowViolations(Instant start, LocalDateTime local)
        {
            if (!IsPublic)
            {
                return SlotViolation.None;
            }

            var violations = SlotViolation.None;
            if (start < _earliest)
            {
                violations |= SlotViolation.BeforeMinNotice;
            }

            if (local.Date > _lastDate)
            {
                violations |= SlotViolation.BeyondMaxAdvance;
            }

            var step = _s.Settings.SlotIntervalMinutes * NodaConstants.TicksPerMinute;
            if (local.TimeOfDay.TickOfDay % step != 0 || ZoneTime.ToInstant(local.Date, local.TimeOfDay, _s.Zone) != start)
            {
                violations |= SlotViolation.OffGrid;
            }

            return violations;
        }

        public SlotViolation EligibilityViolations()
        {
            if (EligibleStaff(onlineOnly: false).Any(list => list.Count == 0))
            {
                return SlotViolation.NoEligibleStaff;
            }

            return HasEligibleStaff ? SlotViolation.None : SlotViolation.NotBookableOnline;
        }

        /// <summary>Bütün kısıtlara uyan ilk atama (fark etmez sırası: musaitlik.md § "Fark etmez").</summary>
        public SlotCandidate? FindStrict(Instant start, LocalTime local, LocalDate date)
        {
            if (!HasEligibleStaff)
            {
                return null;
            }

            var orders = Orders(date);
            if (_r.SameStaffForAll)
            {
                foreach (var staffId in orders[0])
                {
                    var assigned = new List<AssignedItem>(_r.Items.Count);
                    if (TryChainSameStaff(staffId, start, date, assigned))
                    {
                        return new SlotCandidate(start, local, assigned);
                    }
                }

                return null;
            }

            var chain = new List<AssignedItem>(_r.Items.Count);
            var failed = _hasResourceNeeds ? null : new HashSet<(int, Instant)>();
            return TryChain(0, start, date, orders, chain, failed) ? new SlotCandidate(start, local, [.. chain]) : null;
        }

        /// <summary>
        /// Personeli sabit zincir (her kalemde tek olası personel): ihlaller toplanır, aday yine üretilir. Fark etmez
        /// kalem varsa null.
        /// </summary>
        public (SlotViolation Violations, SlotCandidate Candidate)? FixedChain(Instant start, LocalTime local, LocalDate date)
        {
            var fixedId = _r.SameStaffForAll ? _r.Items.FirstOrDefault(i => i.StaffId is not null)?.StaffId : null;
            var staffIds = _r.Items.Select(i => i.StaffId ?? fixedId).ToList();
            if (staffIds.Exists(id => id is null))
            {
                return null;
            }

            var violations = SlotViolation.None;
            var assigned = new List<AssignedItem>(_r.Items.Count);
            var allocated = new List<ResourceAllocation>();
            var cursor = start;
            for (var i = 0; i < _r.Items.Count; i++)
            {
                var staffId = staffIds[i]!.Value;
                var template = Template(_r.Items[i].ServiceId, staffId);
                violations |= Fit(staffId, template, cursor, date);
                var plan = template.At(i + 1, cursor);
                if (!ResourceAllocator.TryAllocate(_s.Resources, Needs(i), plan, allocated, _r.IgnoreAppointmentId, out var resources))
                {
                    violations |= SlotViolation.ResourceUnavailable;
                }

                allocated.AddRange(resources);
                assigned.Add(new AssignedItem(staffId, plan, resources));
                cursor += template.StaffBusyEnd;
            }

            return (violations, new SlotCandidate(start, local, assigned));
        }

        private bool TryChainSameStaff(Guid staffId, Instant start, LocalDate date, List<AssignedItem> assigned)
        {
            var allocated = new List<ResourceAllocation>();
            var cursor = start;
            for (var i = 0; i < _r.Items.Count; i++)
            {
                if (!TryItem(i, staffId, cursor, date, allocated, out var item))
                {
                    return false;
                }

                assigned.Add(item);
                allocated.AddRange(item.Resources);
                cursor = item.Plan.StaffBusyEndUtc;
            }

            return true;
        }

        /// <summary>
        /// Kural: musaitlik.md § Çok kalemli randevu. Kalem n+1, kalem n'in son personel-meşgul diliminin bitişinde başlar;
        /// her kalemde adaylar o günün sırasıyla denenir (derinlik öncelikli; kaynak yoksa başarısız (kalem, an) çiftleri
        /// hatırlanır).
        /// </summary>
        private bool TryChain(int index, Instant cursor, LocalDate date, IReadOnlyList<IReadOnlyList<Guid>> orders, List<AssignedItem> chain, HashSet<(int, Instant)>? failed)
        {
            if (index == _r.Items.Count)
            {
                return true;
            }

            if (failed is not null && failed.Contains((index, cursor)))
            {
                return false;
            }

            var allocated = chain.SelectMany(c => c.Resources).ToList();
            foreach (var staffId in orders[index])
            {
                if (!TryItem(index, staffId, cursor, date, allocated, out var item))
                {
                    continue;
                }

                chain.Add(item);
                if (TryChain(index + 1, item.Plan.StaffBusyEndUtc, date, orders, chain, failed))
                {
                    return true;
                }

                chain.RemoveAt(chain.Count - 1);
            }

            failed?.Add((index, cursor));
            return false;
        }

        private bool TryItem(int index, Guid staffId, Instant start, LocalDate date, List<ResourceAllocation> allocated, out AssignedItem item)
        {
            var template = Template(_r.Items[index].ServiceId, staffId);
            item = null!;
            if (!FitsStrict(staffId, template, start, date))
            {
                return false;
            }

            var plan = template.At(index + 1, start);
            if (!ResourceAllocator.TryAllocate(_s.Resources, Needs(index), plan, allocated, _r.IgnoreAppointmentId, out var resources))
            {
                return false;
            }

            item = new AssignedItem(staffId, plan, resources);
            return true;
        }

        /// <summary>
        /// Kural: musaitlik.md § 8. Personel-meşgul dilim personelin boş kümesine sığmalı; bekleme dilimi personele kısıt
        /// koymaz ama şube açık aralığında kalmalı (M3).
        /// </summary>
        private bool FitsStrict(Guid staffId, ItemTemplate template, Instant start, LocalDate date)
        {
            var day = StaffDay(staffId, date);
            var open = BranchOpen(date);
            foreach (var segment in template.Segments)
            {
                var during = new Interval(start + segment.Offset, start + segment.Offset + segment.Length);
                if (!(segment.StaffBusy ? day.Free : open).Contains(during))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Uymayan dilimlerin nedenleri (panel uyarısı ve yazma yolunun hata kodu için).</summary>
        private SlotViolation Fit(Guid staffId, ItemTemplate template, Instant start, LocalDate date)
        {
            var day = StaffDay(staffId, date);
            var sets = StaffSets(staffId);
            var open = BranchOpen(date);
            var violations = SlotViolation.None;
            foreach (var segment in template.Segments)
            {
                var during = new Interval(start + segment.Offset, start + segment.Offset + segment.Length);
                if (!segment.StaffBusy)
                {
                    if (!open.Contains(during))
                    {
                        violations |= SlotViolation.OutsideWorkingHours;
                    }

                    continue;
                }

                if (day.Free.Contains(during))
                {
                    continue;
                }

                if (sets.Busy.Overlaps(during))
                {
                    violations |= SlotViolation.StaffBusy;
                }

                if (sets.TimeOff.Overlaps(during))
                {
                    violations |= SlotViolation.StaffOnTimeOff;
                }

                if (!day.Working.Contains(during))
                {
                    violations |= SlotViolation.OutsideWorkingHours;
                }
            }

            return violations;
        }

        private IReadOnlyList<ResourceNeed> Needs(int index) =>
            _s.Services.TryGetValue(_r.Items[index].ServiceId, out var service) ? service.ResourceNeeds : [];

        private ItemTemplate Template(Guid serviceId, Guid staffId)
        {
            if (!_templates.TryGetValue((serviceId, staffId), out var template))
            {
                template = ItemTemplate.From(_s.Services[serviceId].OffersByStaff[staffId]);
                _templates[(serviceId, staffId)] = template;
            }

            return template;
        }

        /// <summary>
        /// Kalem başına uygun personel: hizmeti veren (teklifi olan), şubede çalışan; public kipte personel ve hizmet online
        /// açık. "Aynı kişiden" açıkken her kalemde ortak küme (M13).
        /// </summary>
        private IReadOnlyList<IReadOnlyList<Guid>> EligibleStaff(bool onlineOnly)
        {
            var fixedId = _r.SameStaffForAll ? _r.Items.FirstOrDefault(i => i.StaffId is not null)?.StaffId : null;
            var lists = _r.Items.Select(item =>
            {
                if (!_s.Services.TryGetValue(item.ServiceId, out var service) || (onlineOnly && !service.IsOnlineBookable))
                {
                    return new List<Guid>();
                }

                IEnumerable<Guid> ids = (item.StaffId ?? fixedId) is { } staffId ? [staffId] : service.OffersByStaff.Keys;
                return ids.Where(id => service.OffersByStaff.ContainsKey(id)
                        && _s.Staff.TryGetValue(id, out var staff)
                        && (!onlineOnly || staff.IsBookableOnline))
                    .ToList();
            }).ToList();

            if (_r.SameStaffForAll)
            {
                var common = lists.Skip(1).Aggregate(new HashSet<Guid>(lists[0]), (set, list) =>
                {
                    set.IntersectWith(list);
                    return set;
                });
                return [.. lists.Select(_ => (IReadOnlyList<Guid>)[.. common])];
            }

            return [.. lists.Select(list => (IReadOnlyList<Guid>)list)];
        }

        /// <summary>
        /// Kural: musaitlik.md § "Fark etmez". Sıra: (1) o gün en az doluluk oranı, (2) müşterinin tercih ettiği personel,
        /// (3) <c>Staff.Id</c> (uuid sırası; PostgreSQL ile aynı).
        /// </summary>
        private IReadOnlyList<IReadOnlyList<Guid>> Orders(LocalDate date)
        {
            if (!_orders.TryGetValue(date, out var orders))
            {
                orders = [.. _eligible.Select(list => (IReadOnlyList<Guid>)[.. list.Order(Comparer<Guid>.Create((a, b) => CompareForDay(a, b, date)))])];
                _orders[date] = orders;
            }

            return orders;
        }

        private int CompareForDay(Guid a, Guid b, LocalDate date)
        {
            var byOccupancy = StaffDay(a, date).CompareOccupancy(StaffDay(b, date));
            if (byOccupancy != 0)
            {
                return byOccupancy;
            }

            var preferredA = a == _r.PreferredStaffId;
            var preferredB = b == _r.PreferredStaffId;
            return preferredA != preferredB ? (preferredA ? -1 : 1) : a.CompareTo(b);
        }

        /// <summary>Kural: musaitlik.md § 2. Personel saati ∩ şube açık aralığı; izin ve meşguliyet çıkarılır (§ 3–5).</summary>
        private StaffDay StaffDay(Guid staffId, LocalDate date)
        {
            if (_staffDays.TryGetValue((staffId, date), out var day))
            {
                return day;
            }

            var staff = _s.Staff[staffId];
            var sets = StaffSets(staffId);
            var working = ZoneTime.ToSet(date, StaffRanges(staff, date), _s.Zone).Intersect(BranchOpen(date));
            var free = working.Subtract(sets.TimeOff).Subtract(sets.Busy);
            var busyToday = sets.RawBusy.Intersect(ZoneTime.Day(date, _s.Zone)).TotalDuration;
            day = new StaffDay(working, free, (long)busyToday.TotalSeconds, (long)working.TotalDuration.TotalSeconds);
            _staffDays[(staffId, date)] = day;
            return day;
        }

        private IReadOnlyList<LocalTimeRange> StaffRanges(StaffSchedule staff, LocalDate date) => StaffHoursOn(_s, staff, date);

        /// <summary>
        /// Kural: musaitlik.md § 3–5 ve M17, M18. Süresi geçmiş tutma, sorana ayrılmış teklif ve saati değişen randevunun
        /// kendi dilimleri meşguliyet sayılmaz. Başka şubedeki meşguliyetin önüne (bu şube → o şube) ve arkasına
        /// (o şube → bu şube) geçiş süresi eklenir.
        /// </summary>
        private StaffSets StaffSets(Guid staffId)
        {
            if (_staffSets.TryGetValue(staffId, out var sets))
            {
                return sets;
            }

            var staff = _s.Staff[staffId];
            var active = staff.Busy.Where(b =>
                    !(b.HoldExpiresUtc is { } expires && expires <= _s.Now)
                    && !(b.ReservedForCustomerId is { } owner && owner == _r.CustomerId)
                    && !(b.AppointmentId is { } appointmentId && appointmentId == _r.IgnoreAppointmentId))
                .ToList();
            var padded = active.Select(b => b.BranchId == _s.BranchId
                ? b.During
                : new Interval(b.During.Start - _s.Transfers.Between(_s.BranchId, b.BranchId), b.During.End + _s.Transfers.Between(b.BranchId, _s.BranchId)));
            sets = new StaffSets(IntervalSet.From(padded), IntervalSet.From(active.Select(b => b.During)), IntervalSet.From(staff.TimeOffs));
            _staffSets[staffId] = sets;
            return sets;
        }
    }

    private sealed record StaffSets(IntervalSet Busy, IntervalSet RawBusy, IntervalSet TimeOff);

    /// <summary>Personelin bir günü: çalışma kümesi, boş küme ve doluluk (meşgul / çalışma saniyesi).</summary>
    private sealed record StaffDay(IntervalSet Working, IntervalSet Free, long BusySeconds, long WorkingSeconds)
    {
        /// <summary>Doluluk oranını kesir olarak karşılaştırır; çalışma süresi olmayan en sona.</summary>
        public int CompareOccupancy(StaffDay other)
        {
            if (WorkingSeconds == 0 || other.WorkingSeconds == 0)
            {
                return (WorkingSeconds == 0).CompareTo(other.WorkingSeconds == 0);
            }

            return (BusySeconds * other.WorkingSeconds).CompareTo(other.BusySeconds * WorkingSeconds);
        }
    }
}
