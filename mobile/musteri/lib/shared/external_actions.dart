import 'package:add_2_calendar/add_2_calendar.dart';
import 'package:flutter/foundation.dart';
import 'package:url_launcher/url_launcher.dart';

/// Takvime eklenecek randevu.
@immutable
class CalendarEntry {
  /// Takvim kaydı; zamanlar UTC an.
  const CalendarEntry({
    required this.title,
    required this.startUtc,
    required this.endUtc,
    required this.timeZone,
    this.description,
    this.location,
  });

  /// "Kuaför Elif randevusu".
  final String title;

  /// Başlangıç.
  final DateTime startUtc;

  /// Bitiş.
  final DateTime endUtc;

  /// Şubenin saat dilimi.
  final String timeZone;

  /// Hizmetler ve personel.
  final String? description;

  /// Şube adresi.
  final String? location;
}

/// Uygulama dışına çıkan eylemler (harita, tarayıcı, takvim). Testte sahtesi.
abstract interface class ExternalActions {
  /// Bağlantıyı dış uygulamada açar; açılamazsa false.
  Future<bool> openUrl(Uri url);

  /// Yerel takvimin "etkinlik ekle" ekranını açar.
  Future<bool> addToCalendar(CalendarEntry entry);
}

/// `url_launcher` ve `add_2_calendar`.
final class PlatformExternalActions implements ExternalActions {
  /// Platform eylemleri.
  const PlatformExternalActions();

  @override
  Future<bool> openUrl(Uri url) =>
      launchUrl(url, mode: LaunchMode.externalApplication);

  @override
  Future<bool> addToCalendar(CalendarEntry entry) => Add2Calendar.addEvent2Cal(
    Event(
      title: entry.title,
      description: entry.description,
      location: entry.location,
      startDate: entry.startUtc,
      endDate: entry.endUtc,
      timeZone: entry.timeZone,
    ),
  );
}

/// Yol tarifi bağlantısı: konum varsa koordinat, yoksa adres. iOS'ta Apple
/// Haritalar, diğerlerinde Google Haritalar.
Uri directionsUrl({
  required String address,
  required TargetPlatform platform,
  num? latitude,
  num? longitude,
}) {
  final destination = latitude != null && longitude != null
      ? '$latitude,$longitude'
      : address;
  return platform == TargetPlatform.iOS
      ? Uri.https('maps.apple.com', '/', {'daddr': destination})
      : Uri.https('www.google.com', '/maps/dir/', {
          'api': '1',
          'destination': destination,
        });
}
