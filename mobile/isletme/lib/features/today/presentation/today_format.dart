import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/shared/api_date.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Salı 25 Kasım · Kadıköy".
String todayHeader(ManageToday today) =>
    '${formatDate(localDateOf(today.date), DateStyle.weekdayDayMonth)} · '
    '${today.branchName}';

/// "50 dk sonra", "1 sa 20 dk sonra"; başlamışsa "Şimdi".
String startsIn(DateTime startUtc, DateTime nowUtc) {
  final minutes = (startUtc.difference(nowUtc).inSeconds / 60).ceil();
  return minutes <= 0 ? 'Şimdi' : '${formatDuration(minutes)} sonra';
}

/// Randevu süresi ("45 dk").
String durationOf(TodayAppointment a) =>
    formatDuration(a.endUtc.difference(a.startUtc).inMinutes);

/// "Saç kesimi · Can": personel adlarının ilk sözcüğü (satır kısa kalır).
String serviceAndStaff(TodayAppointment a) {
  final staff = a.staffNames.map((n) => n.split(' ').first).join(', ');
  return staff.isEmpty ? a.serviceSummary : '${a.serviceSummary} · $staff';
}

/// "Kasım primi" (BentoBlok gibi büyük harfle yazılır: "KASIM PRİMİ").
String commissionLabel(String yearMonth) {
  final month = formatDate('$yearMonth-01', DateStyle.monthYear).split(' ');
  return '${month.first} primi';
}

/// Grafik için ay içi kümülatif prim (kaydı olmayan gün listede yok).
List<int> cumulativeCommission(List<CommissionDayAmount> daily) {
  final sorted = [...daily]..sort((a, b) => a.date.compareTo(b.date));
  var total = 0;
  return [for (final day in sorted) total += day.amountMinor];
}
