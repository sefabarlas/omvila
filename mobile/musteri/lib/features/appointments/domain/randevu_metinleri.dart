import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Saç kesimi, Fön".
String hizmetlerMetni(List<MeAppointmentItem> items) =>
    items.map((i) => i.serviceName).join(', ');

/// Personel adları, tekrarsız ("Elif Kaya"); personelsiz randevuda boş.
String personelMetni(List<MeAppointmentItem> items) => {
  for (final i in items)
    if (i.staffName case final name? when name.isNotEmpty) name,
}.join(', ');

/// "Saç kesimi, Fön · Elif Kaya · Kadıköy": boş parçalar atlanır.
String ozetSatiri(Iterable<String> parcalar) =>
    parcalar.where((p) => p.isNotEmpty).join(' · ');

/// Randevu günü: "Bugün", "Yarın" veya "Cum 28 Kas" (şubenin saat dilimi).
String gunEtiketi(DateTime startUtc, String timeZone, Clock clock) {
  final day = toBranchDate(startUtc, timeZone);
  return switch (daysBetween(branchToday(clock, timeZone), day)) {
    0 => 'Bugün',
    1 => 'Yarın',
    _ => formatDate(day, DateStyle.compact),
  };
}

/// Başlangıca kalan süre: "45 dk sonra", "18 saat sonra", "3 gün sonra".
/// Başladıysa null.
String? kalanSureEtiketi(DateTime startUtc, Clock clock) {
  final minutes = startUtc.difference(clock.now()).inMinutes;
  if (minutes < 0) {
    return null;
  }

  if (minutes < 60) {
    return '$minutes dk sonra';
  }

  final hours = minutes ~/ 60;
  return hours < 24 ? '$hours saat sonra' : '${hours ~/ 24} gün sonra';
}

/// Randevunun süresi dakika.
int sureDakika(DateTime startUtc, DateTime endUtc) =>
    endUtc.difference(startUtc).inMinutes;

/// Özel adın ardına bulunma eki ve geçmiş zaman: "Kuaför Elif" →
/// "Kuaför Elif'teydi", "Spa Moda" → "Spa Moda'daydı".
String bulunmaGecmis(String name) {
  final ek = _bulunmaEki(name);
  return "$name'$ek${ek.endsWith('a') ? 'ydı' : 'ydi'}";
}

/// Bulunma eki: "Kuaför Elif" → "Kuaför Elif'te", "12 Kasım" → "12 Kasım'da".
String bulunma(String name) => "$name'${_bulunmaEki(name)}";

String _bulunmaEki(String name) {
  final lower = name
      .replaceAll('I', 'ı')
      .replaceAll('İ', 'i')
      .toLowerCase()
      .trimRight();
  const vowels = 'aıoueiöü';
  final lastVowel = lower
      .split('')
      .lastWhere(
        vowels.contains,
        orElse: () => 'e',
      );
  final last = lower.isEmpty ? '' : lower[lower.length - 1];
  final consonant = 'fstkçşhp'.contains(last) ? 't' : 'd';
  return '$consonant${'aıou'.contains(lastVowel) ? 'a' : 'e'}';
}
