import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Randevu detayı (meGetAppointment).
final FutureProviderFamily<MeAppointment, String> appointmentProvider =
    FutureProvider.autoDispose.family<MeAppointment, String>(
      (ref, id) => ref.watch(appointmentsRepositoryProvider).appointment(id),
    );

/// Randevu sürmekte: iptal ve saat değişikliği gösterilir.
bool isActive(MeAppointment a) => switch (a.displayStatus) {
  DisplayStatus.pendingApproval ||
  DisplayStatus.depositPending ||
  DisplayStatus.confirmed => true,
  _ => false,
};

/// Son sürenin başlangıca uzaklığı saat ("24").
int? deadlineHours(MeAppointment a, DateTime? deadline) =>
    deadline == null ? null : a.startUtc.difference(deadline).inHours;

/// "İptal 24 saat, saat değişikliği 12 saat öncesine kadar."; son süre
/// yoksa null.
String? sureNotu(MeAppointment a) {
  final cancel = deadlineHours(a, a.cancelDeadlineUtc);
  final change = deadlineHours(a, a.rescheduleDeadlineUtc);
  return switch ((cancel, change)) {
    (final c?, final r?) =>
      'İptal $c saat, saat değişikliği $r saat '
          'öncesine kadar.',
    (final c?, null) => 'İptal $c saat öncesine kadar.',
    (null, final r?) => 'Saat değişikliği $r saat öncesine kadar.',
    _ => null,
  };
}

/// Kapora satırı değeri: "200 ₺ alındı · havale"; istenmiyorsa null.
String? kaporaMetni(MeAppointmentDeposit? d) {
  if (d == null) {
    return null;
  }

  final amount = formatMoney(d.required_.amountMinor);
  return switch (d.status) {
    DepositStatus.awaiting => '$amount · bekleniyor',
    DepositStatus.customerReported => '$amount · salon onayı bekleniyor',
    DepositStatus.received => '$amount alındı · havale',
    DepositStatus.refunded => '$amount · salon iade etti',
    DepositStatus.waived || DepositStatus.unknownDefaultOpenApi => null,
  };
}

/// "Kuaför Elif · Yarın 14:30".
String randevuKisaMetni(MeAppointment a, Clock clock) {
  final day = gunEtiketi(a.startUtc, a.timeZone, clock);
  return ozetSatiri([
    a.brand.name,
    '$day ${formatTime(a.startUtc, a.timeZone)}',
  ]);
}
