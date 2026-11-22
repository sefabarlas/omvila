import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/calendar/data/calendar_repository.dart';
import 'package:omvila_isletme/features/calendar/presentation/calendar_screen.dart';
import 'package:omvila_isletme/features/calendar/presentation/day_strip.dart';
import 'package:omvila_isletme/features/new_appointment/data/booking_repository.dart';
import 'package:omvila_isletme/features/new_appointment/presentation/pickers.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/features/today/data/today_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Hizmet çiplerinde "Tümü"nden önce gösterilen hizmet sayısı.
const _visibleServices = 3;

/// Yeni randevu (YeniRandevu): nasıl geldi, müşteri (ara veya yeni), hizmet,
/// personel, gün, saat (manageSearchSlots), tekrar, "Müşteriye SMS gönder".
/// Panel/telefon kanalında randevu onaylı oluşur, kapora istenmez. Personel
/// rolünde yalnız kendine randevu ekler. Salt okunur abonelikte pasif.
class NewAppointmentScreen extends ConsumerStatefulWidget {
  /// Yeni randevu.
  const NewAppointmentScreen({
    this.initialDate,
    this.initialCustomer,
    super.key,
  });

  /// Takvimden gelindiyse seçili gün.
  final String? initialDate;

  /// Müşteri kartından ("Randevu ver") gelindiyse seçili müşteri.
  final CustomerSummary? initialCustomer;

  @override
  ConsumerState<NewAppointmentScreen> createState() =>
      _NewAppointmentScreenState();
}

class _NewAppointmentScreenState extends ConsumerState<NewAppointmentScreen> {
  ManageAppointmentCreateChannelEnum _channel =
      ManageAppointmentCreateChannelEnum.phone;
  late CustomerSummary? _customer = widget.initialCustomer;
  bool _newCustomer = false;
  final _search = TextEditingController();
  final _name = TextEditingController();
  final _phone = TextEditingController();
  Timer? _debounce;
  String _query = '';
  String? _serviceId;
  String? _staffId;
  late String _date =
      widget.initialDate ??
      branchToday(ref.read(clockProvider), defaultTimeZone);
  late final String _stripStart = addDays(_date, -1);
  Slot? _slot;
  RecurrenceRequestIntervalWeeksEnum? _interval;
  bool _notify = true;
  bool _busy = false;
  String? _error;
  String _key = newIdempotencyKey();

  @override
  void initState() {
    super.initState();
    final session = ref.read(authControllerProvider).value;
    if (session != null && session.isStaff) {
      _staffId = session.membership.staffId;
    }
  }

  @override
  void dispose() {
    _debounce?.cancel();
    _search.dispose();
    _name.dispose();
    _phone.dispose();
    super.dispose();
  }

  void _onSearch(String text) {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 300), () {
      if (mounted) {
        setState(() => _query = text.trim());
      }
    });
  }

  String? get _newPhone => parseMobilePhone(_phone.text);

  bool get _customerReady =>
      _customer != null ||
      (_newCustomer && _name.text.trim().length >= 2 && _newPhone != null);

  Future<void> _create({
    required ManageSession session,
    bool allowOutsideWorkingHours = false,
  }) async {
    final slot = _slot!;
    final customer = _customer;
    final interval = _interval;
    setState(() {
      _busy = true;
      _error = null;
    });
    final router = GoRouter.of(context);
    final messenger = ScaffoldMessenger.of(context);
    try {
      final created = await ref.read(createAppointmentProvider)(
        ManageAppointmentCreate(
          branchId: session.branches.first.id,
          channel: _channel,
          brandCustomerId: customer?.id,
          newCustomer: customer == null
              ? ManageAppointmentCreateNewCustomer(
                  fullName: _name.text.trim(),
                  phone: _newPhone!,
                )
              : null,
          startUtc: slot.startUtc,
          items: [
            for (final item in slot.items)
              ManageAppointmentCreateItemsInner(
                serviceId: item.serviceId,
                staffId: item.staffId,
              ),
          ],
          recurrence: interval == null
              ? null
              : RecurrenceRequest(
                  frequency: RecurrenceRequestFrequencyEnum.weekly,
                  intervalWeeks: interval,
                ),
          notifyCustomer: _notify,
          allowOutsideWorkingHours: allowOutsideWorkingHours ? true : null,
        ),
        key: _key,
      );
      ref
        ..invalidate(todayProvider)
        ..invalidate(calendarProvider);
      final skipped = created.skippedOccurrences.length;
      messenger.showSnackBar(
        SnackBar(
          content: Text(
            skipped == 0
                ? 'Randevu oluşturuldu.'
                : 'Randevu oluşturuldu. $skipped tekrar çakıştığı için '
                      'eklenmedi.',
          ),
        ),
      );
      unawaited(
        router.pushReplacement(Routes.appointment(created.appointment.id)),
      );
    } on ApiProblem catch (e) {
      if (e.status != 0) {
        _key = newIdempotencyKey();
      }

      if (!mounted) {
        return;
      }

      setState(() => _busy = false);
      if (e.code == 'outside_working_hours' || e.code == 'staff_on_time_off') {
        final proceed = await _confirmOutsideHours(errorMessage(e.code));
        if (proceed) {
          await _create(session: session, allowOutsideWorkingHours: true);
        }

        return;
      }

      setState(() => _error = errorMessage(e.code));
      if (e.code == 'slot_taken') {
        setState(() => _slot = null);
        ref.invalidate(slotsProvider);
      }
    }
  }

  Future<bool> _confirmOutsideHours(String message) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Saati onaylayın'),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Vazgeç'),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Yine de oluştur'),
          ),
        ],
      ),
    );
    return result ?? false;
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final session = ref.watch(authControllerProvider).value;
    if (session == null) {
      return const Scaffold(body: Loading());
    }

    final branchId = session.branches.first.id;
    final readOnly =
        session.subscription.status == SubscriptionBannerStatusEnum.readOnly;
    final services = ref.watch(servicesProvider(branchId));
    final staff = ref.watch(staffProvider(branchId));
    final serviceList = services.value;
    final staffList = staff.value;

    final Widget content;
    if (serviceList != null && staffList != null) {
      content = _form(context, session, serviceList, staffList);
    } else if (services.hasError || staff.hasError) {
      content = LoadError(
        title: 'Randevu formu yüklenemedi',
        error: services.error ?? staff.error,
        onRetry: () => ref
          ..invalidate(servicesProvider(branchId))
          ..invalidate(staffProvider(branchId)),
      );
    } else {
      content = const Loading();
    }

    final service = serviceList?.where((s) => s.id == _serviceId).firstOrNull;
    final slot = _slot;
    final ready =
        !readOnly && _customerReady && service != null && slot != null;
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk2,
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk6,
          ),
          children: [
            Align(
              alignment: Alignment.centerLeft,
              child: IconButton(
                tooltip: 'Kapat',
                style: IconButton.styleFrom(
                  backgroundColor: r.zeminIkincil,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                  ),
                ),
                icon: Icon(LucideIcons.x, color: r.metin),
                onPressed: () =>
                    context.canPop() ? context.pop() : context.go(Routes.today),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
            Semantics(
              header: true,
              child: Text(
                'Yeni randevu',
                style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
              ),
            ),
            if (readOnly) ...[
              const SizedBox(height: OmvilaBosluk.bosluk3),
              SurfaceCard(
                color: r.hataZemin,
                child: Text(
                  errorMessage('subscription_read_only'),
                  style: OmvilaYazi.ikincil.copyWith(color: r.metin),
                ),
              ),
            ],
            const SizedBox(height: OmvilaBosluk.bosluk4),
            content,
          ],
        ),
      ),
      bottomNavigationBar: Material(
        color: r.zeminYukseltilmis,
        child: SafeArea(
          top: false,
          child: Padding(
            padding: const EdgeInsets.fromLTRB(
              OmvilaBosluk.bosluk4,
              OmvilaBosluk.bosluk3,
              OmvilaBosluk.bosluk4,
              OmvilaBosluk.bosluk3,
            ),
            child: Row(
              children: [
                Expanded(
                  child: slot == null || service == null
                      ? Text(
                          'Müşteri, hizmet ve saat seçin',
                          style: OmvilaYazi.ikincil.copyWith(
                            color: r.metinIkincil,
                          ),
                        )
                      : Column(
                          mainAxisSize: MainAxisSize.min,
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              '${formatDate(_date, DateStyle.compact)} · '
                              '${slot.localTime}',
                              style: OmvilaYazi.govdeGuclu.copyWith(
                                color: r.metin,
                              ),
                            ),
                            Text(
                              [
                                service.name,
                                ...{
                                  for (final i in slot.items)
                                    if (i.staffName != null)
                                      i.staffName!.split(' ').first,
                                },
                                formatMoney(
                                  slot.total.amountMinor,
                                  currency: slot.total.currency.value,
                                ),
                              ].join(' · '),
                              style: OmvilaYazi.aciklama.copyWith(
                                color: r.metinIkincil,
                              ),
                            ),
                          ],
                        ),
                ),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                OmvilaButon(
                  etiket: 'Oluştur',
                  yukleniyor: _busy,
                  yukleniyorEtiketi: 'Oluşturuluyor…',
                  onPressed: ready && !_busy
                      ? () => _create(session: session)
                      : null,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _form(
    BuildContext context,
    ManageSession session,
    List<Service> services,
    List<StaffSummary> allStaff,
  ) {
    final r = context.omvilaRenkler;
    final service = services.where((s) => s.id == _serviceId).firstOrNull;
    final ownStaffId = session.isStaff ? session.membership.staffId : null;
    final staff = [
      for (final s in allStaff)
        if ((ownStaffId == null || s.id == ownStaffId) &&
            (service == null ||
                service.staffIds.isEmpty ||
                service.staffIds.contains(s.id)))
          s,
    ];
    final chosenStaff = staff.where((s) => s.id == _staffId).firstOrNull;
    final visible = [
      ...services.take(_visibleServices),
      if (service != null && !services.take(_visibleServices).contains(service))
        service,
    ];
    final error = _error;
    const gap = SizedBox(height: OmvilaBosluk.bosluk5);
    const labelGap = SizedBox(height: OmvilaBosluk.bosluk2);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        const SectionLabel('Nasıl geldi?'),
        labelGap,
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          children: [
            SelectChip(
              label: 'Telefonla',
              icon: LucideIcons.phone,
              selected: _channel == ManageAppointmentCreateChannelEnum.phone,
              onTap: () => setState(
                () => _channel = ManageAppointmentCreateChannelEnum.phone,
              ),
            ),
            SelectChip(
              label: 'Kapıdan',
              icon: LucideIcons.doorOpen,
              selected: _channel == ManageAppointmentCreateChannelEnum.walkIn,
              onTap: () => setState(
                () => _channel = ManageAppointmentCreateChannelEnum.walkIn,
              ),
            ),
          ],
        ),
        gap,
        const SectionLabel('Müşteri'),
        labelGap,
        _customerSection(context),
        gap,
        const SectionLabel('Hizmet'),
        labelGap,
        if (services.isEmpty)
          Text(
            'Henüz hizmet yok. Hizmetleri salon panelinden ekleyin.',
            style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
          )
        else
          Wrap(
            spacing: OmvilaBosluk.bosluk2,
            runSpacing: OmvilaBosluk.bosluk2,
            children: [
              for (final s in visible)
                SelectChip(
                  label: '${s.name} · ${formatDuration(s.durationMinutes)}',
                  selected: s.id == _serviceId,
                  onTap: () => _chooseService(s),
                ),
              if (services.length > _visibleServices)
                SelectChip(
                  label: 'Tümü',
                  icon: LucideIcons.search,
                  selected: false,
                  onTap: () => _allServices(services),
                ),
            ],
          ),
        gap,
        const SectionLabel('Personel'),
        labelGap,
        StaffPicker(
          staff: staff,
          selected: _staffId,
          allowAny: ownStaffId == null,
          onSelect: (id) => setState(() {
            _staffId = id;
            _slot = null;
          }),
        ),
        gap,
        const SectionLabel('Gün'),
        labelGap,
        // Sayfa kenar boşluğunu şerit kendisi verir.
        FullBleed(
          height: dayStripHeight,
          child: DayStrip(
            start: _stripStart,
            count: stripDays,
            selected: _date,
            view: null,
            onSelect: (d) => setState(() {
              _date = d;
              _slot = null;
            }),
          ),
        ),
        gap,
        SectionLabel(
          'Saat',
          trailing: chosenStaff == null
              ? null
              : '${chosenStaff.fullName.split(' ').first} için boş',
        ),
        labelGap,
        if (service == null)
          Text(
            'Saatleri görmek için hizmet seçin.',
            style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
          )
        else
          SlotGrid(
            query: SlotQuery(
              branchId: session.branches.first.id,
              date: _date,
              serviceIds: [service.id],
              staffId: _staffId,
            ),
            selected: _slot?.startUtc,
            onSelect: (s) => setState(() => _slot = s),
          ),
        gap,
        const SectionLabel('Tekrar'),
        labelGap,
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          runSpacing: OmvilaBosluk.bosluk2,
          children: [
            for (final (label, value, icon) in const [
              ('Tekrarlanmasın', null, null),
              (
                'Her hafta',
                RecurrenceRequestIntervalWeeksEnum.number1,
                LucideIcons.repeat,
              ),
              (
                '2 haftada bir',
                RecurrenceRequestIntervalWeeksEnum.number2,
                null,
              ),
              (
                '4 haftada bir',
                RecurrenceRequestIntervalWeeksEnum.number4,
                null,
              ),
            ])
              SelectChip(
                label: label,
                icon: icon,
                selected: _interval == value,
                onTap: () => setState(() => _interval = value),
              ),
          ],
        ),
        gap,
        SurfaceCard(
          padding: const EdgeInsets.symmetric(
            horizontal: OmvilaBosluk.bosluk4,
          ),
          child: SwitchListTile(
            contentPadding: EdgeInsets.zero,
            value: _notify,
            onChanged: (v) => setState(() => _notify = v),
            title: Text(
              'Müşteriye SMS gönder',
              style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
            ),
            subtitle: Text(
              'Randevu bilgisi ve iptal bağlantısı',
              style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
            ),
          ),
        ),
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          Semantics(
            liveRegion: true,
            child: Text(
              error,
              style: OmvilaYazi.ikincil.copyWith(color: r.hata),
            ),
          ),
        ],
      ],
    );
  }

  void _chooseService(Service s) {
    setState(() {
      _serviceId = s.id;
      _slot = null;
      if (_staffId != null &&
          s.staffIds.isNotEmpty &&
          !s.staffIds.contains(_staffId) &&
          !(ref.read(authControllerProvider).value?.isStaff ?? false)) {
        _staffId = null;
      }
    });
  }

  Future<void> _allServices(List<Service> services) async {
    final chosen = await showOmvilaSheet<Service>(
      context,
      builder: (context) => Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Text(
            'Hizmetler',
            style: OmvilaYazi.baslik2.copyWith(
              color: context.omvilaRenkler.metin,
            ),
          ),
          for (final s in services)
            ListTile(
              contentPadding: EdgeInsets.zero,
              title: Text(s.name),
              subtitle: Text(formatDuration(s.durationMinutes)),
              trailing: Text(
                formatMoney(
                  s.price.amountMinor,
                  currency: s.price.currency.value,
                ),
              ),
              onTap: () => Navigator.of(context).pop(s),
            ),
        ],
      ),
    );
    if (chosen != null) {
      _chooseService(chosen);
    }
  }

  Widget _customerSection(BuildContext context) {
    final r = context.omvilaRenkler;
    final customer = _customer;
    if (customer != null) {
      final phone =
          customer.phoneMasked ??
          (customer.phone == null ? null : formatPhone(customer.phone!));
      return Container(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
        decoration: BoxDecoration(
          color: r.zeminYukseltilmis,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
          border: Border.all(color: r.marka, width: 2),
        ),
        child: Row(
          children: [
            InitialsAvatar(name: customer.fullName, size: 32),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    customer.fullName,
                    style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                  ),
                  Text(
                    [
                      ?phone,
                      if (customer.visitCount > 0)
                        '${customer.visitCount} ziyaret'
                      else
                        'Yeni müşteri',
                    ].join(' · '),
                    style: OmvilaYazi.aciklama.copyWith(
                      color: r.metinIkincil,
                    ),
                  ),
                ],
              ),
            ),
            IconButton(
              tooltip: 'Müşteriyi değiştir',
              icon: Icon(LucideIcons.x, color: r.metin),
              onPressed: () => setState(() => _customer = null),
            ),
          ],
        ),
      );
    }

    if (_newCustomer) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          TextField(
            controller: _name,
            textCapitalization: TextCapitalization.words,
            decoration: const InputDecoration(labelText: 'Ad soyad'),
            onChanged: (_) => setState(() {}),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          TextField(
            controller: _phone,
            keyboardType: TextInputType.phone,
            decoration: const InputDecoration(
              labelText: 'Cep telefonu',
              hintText: '0532 123 45 67',
            ),
            onChanged: (_) => setState(() {}),
          ),
          Align(
            alignment: Alignment.centerLeft,
            child: TextButton(
              onPressed: () => setState(() => _newCustomer = false),
              child: const Text('Kayıtlı müşteri ara'),
            ),
          ),
        ],
      );
    }

    final results = _query.length < 2
        ? null
        : ref.watch(customerSearchProvider(_query));
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        TextField(
          controller: _search,
          onChanged: _onSearch,
          decoration: const InputDecoration(
            labelText: 'Ad veya telefonla arayın',
            prefixIcon: Icon(LucideIcons.search, size: 20),
          ),
        ),
        if (results != null)
          switch (results) {
            AsyncData(:final value) when value.isEmpty => Padding(
              padding: const EdgeInsets.symmetric(
                vertical: OmvilaBosluk.bosluk2,
              ),
              child: Text(
                'Bu aramayla müşteri bulunamadı.',
                style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
              ),
            ),
            AsyncData(:final value) => Column(
              children: [
                for (final c in value)
                  ListTile(
                    contentPadding: EdgeInsets.zero,
                    leading: InitialsAvatar(name: c.fullName, size: 32),
                    title: Text(c.fullName),
                    subtitle: Text(
                      c.phoneMasked ??
                          (c.phone == null ? '' : formatPhone(c.phone!)),
                    ),
                    onTap: () => setState(() {
                      _customer = c;
                      _search.clear();
                      _query = '';
                    }),
                  ),
              ],
            ),
            AsyncError(:final error) => Padding(
              padding: const EdgeInsets.symmetric(
                vertical: OmvilaBosluk.bosluk2,
              ),
              child: Text(
                errorMessage(error is ApiProblem ? error.code : null),
                style: OmvilaYazi.ikincil.copyWith(color: r.hata),
              ),
            ),
            _ => const Loading(),
          },
        Align(
          alignment: Alignment.centerLeft,
          child: TextButton.icon(
            onPressed: () => setState(() => _newCustomer = true),
            icon: const Icon(LucideIcons.userPlus, size: 18),
            label: const Text('Yeni müşteri ekle'),
          ),
        ),
      ],
    );
  }
}
