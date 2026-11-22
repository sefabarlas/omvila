import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Personelin ilk adı ("Elif Kaya" → "Elif").
String firstNameOf(String? fullName) =>
    (fullName ?? '').trim().split(RegExp(r'\s+')).first;

/// Kalemin tahsil edilen tutarı: fiyat + personel farkı − indirim.
int itemAmount(AppointmentItemView item) =>
    item.price.amountMinor +
    (item.staffSurcharge?.amountMinor ?? 0) -
    (item.discount?.amountMinor ?? 0);

/// Etkin kalemler (iptal edilen kalem gösterilmez).
List<AppointmentItemView> activeItems(AppointmentDetail a) => [
  for (final i in a.items)
    if (i.status != AppointmentItemViewStatusEnum.cancelled) i,
];

/// "10:30–12:00".
String timeRange(DateTime start, DateTime end, String timeZone) =>
    '${formatTime(start, timeZone)}–${formatTime(end, timeZone)}';

/// "Boya ve fön": hizmet adları.
String servicesOf(AppointmentDetail a) =>
    activeItems(a).map((i) => i.serviceName).join(' ve ');

/// "Boya ve fön · Elif · 90 dk".
String summaryLine(AppointmentDetail a) {
  final staff = {
    for (final i in activeItems(a))
      if (i.staffName != null) firstNameOf(i.staffName),
  };
  final minutes = a.endUtc.difference(a.startUtc).inMinutes;
  return [
    servicesOf(a),
    if (staff.isNotEmpty) staff.join(', '),
    formatDuration(minutes),
  ].join(' · ');
}

/// "SAL 25 KASIM · ONLİNE".
String headerLine(AppointmentDetail a) {
  final day = formatDate(
    toBranchDate(a.startUtc, a.timeZone),
    DateStyle.short,
  ).split(' ');
  // "25 Kas Sal" → "Sal 25 Kasım" biçimi için uzun ay adı.
  final longMonth = formatDate(
    toBranchDate(a.startUtc, a.timeZone),
    DateStyle.dayMonth,
  );
  return toUpperTr('${day.last} $longMonth · ${channelLabel(a.channel)}');
}

/// Kanal adı.
String channelLabel(AppointmentChannel channel) => switch (channel) {
  AppointmentChannel.online => 'Online',
  AppointmentChannel.panel => 'Panel',
  AppointmentChannel.phone => 'Telefon',
  AppointmentChannel.walkIn => 'Kapıdan',
  AppointmentChannel.unknownDefaultOpenApi => 'Randevu',
};

/// "12. ziyaret · son 21 Ekim"; ilk ziyarette "Yeni müşteri".
String visitLine(AppointmentDetailCustomer c) {
  final last = c.lastVisitOn;
  if (c.visitCount == 0 || (c.isNew ?? false) && last == null) {
    return 'Yeni müşteri';
  }

  final visit = '${c.visitCount}. ziyaret';
  return last == null
      ? visit
      : '$visit · son ${formatDate(localDateOf(last), DateStyle.dayMonth)}';
}

/// Geldi işaretlendiği an (geçmişteki son CheckedIn).
DateTime? checkedInAt(AppointmentDetail a) {
  DateTime? at;
  for (final h in a.history) {
    if (h.kind == AppointmentHistoryEntryKindEnum.checkedIn) {
      at = h.occurredUtc;
    }
  }

  return at;
}

/// Kapora bekleniyor mu (havale gelmedi).
bool depositAwaited(AppointmentDetail a) {
  final status = a.deposit?.status;
  return a.status == AppointmentStatus.pendingApproval &&
      (status == DepositStatus.awaiting ||
          status == DepositStatus.customerReported);
}

/// Toplam kartındaki kapora değeri: "200 ₺ · havale ile alındı".
String? depositValue(AppointmentDetail a) {
  final d = a.deposit;
  if (d == null) {
    return null;
  }

  final amount = formatMoney(
    d.required_.amountMinor,
    currency: d.required_.currency.value,
  );
  return switch (d.status) {
    DepositStatus.received => '$amount · havale ile alındı',
    DepositStatus.awaiting => '$amount · bekleniyor',
    DepositStatus.customerReported => '$amount · havale bildirildi',
    DepositStatus.refunded => '$amount · salon iade etti',
    DepositStatus.waived => 'Kaporasız onaylandı',
    DepositStatus.unknownDefaultOpenApi => amount,
  };
}

/// Para gösterimi.
String money(Money m) => formatMoney(m.amountMinor, currency: m.currency.value);
