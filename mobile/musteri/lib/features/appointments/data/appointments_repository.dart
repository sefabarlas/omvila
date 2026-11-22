import 'package:flutter/foundation.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';

/// Randevularım ana ekranının verisi.
@immutable
class HomeData {
  /// Veri.
  const HomeData({
    required this.firstName,
    required this.upcoming,
    required this.last,
    required this.hasBrands,
  });

  /// Selamlamadaki ad; profilde ad yoksa null.
  final String? firstName;

  /// Yaklaşan randevular, başlangıca göre artan (ilki koyu kart).
  final List<MeAppointmentSummary> upcoming;

  /// Yaklaşan yoksa son randevu ("Aynısını tekrar al", KK2).
  final MeAppointmentSummary? last;

  /// Kayıtlı veya randevu alınmış salon var.
  final bool hasBrands;

  /// İlk giriş: salon da randevu da yok (MainBos, KK1).
  bool get isFirstVisit => upcoming.isEmpty && last == null && !hasBrands;
}

/// Müşterinin randevuları (meListAppointments, meGetAppointment) ve profili.
abstract interface class AppointmentsRepository {
  /// Ana ekran verisi.
  Future<HomeData> loadHome();

  /// Randevu detayı (şube adresi ve konumu: "Yol tarifi").
  Future<MeAppointment> appointment(String id);
}

/// Üretilmiş istemciyle.
final class ApiAppointmentsRepository implements AppointmentsRepository {
  /// [api] müşteri jetonuyla.
  const ApiAppointmentsRepository(this.api);

  /// API.
  final OmvilaApi api;

  @override
  Future<HomeData> loadHome() => apiCall(() async {
    final appointments = api.getMeAppointmentsApi();
    final (profile, upcoming, brands) = await (
      api.getMeProfileApi().meGetProfile(),
      appointments.meListAppointments(scope: 'Upcoming'),
      api.getMeBrandsApi().meListBrands(),
    ).wait;
    final upcomingItems = upcoming.data!.items;
    MeAppointmentSummary? last;
    if (upcomingItems.isEmpty) {
      final past = await appointments.meListAppointments(
        scope: 'Past',
        limit: 1,
      );
      last = past.data!.items.firstOrNull;
    }

    final fullName = profile.data!.fullName?.trim();
    return HomeData(
      firstName: fullName == null || fullName.isEmpty
          ? null
          : fullName.split(RegExp(r'\s+')).first,
      upcoming: upcomingItems,
      last: last,
      hasBrands: brands.data!.items.isNotEmpty,
    );
  });

  @override
  Future<MeAppointment> appointment(String id) => apiCall(() async {
    final response = await api.getMeAppointmentsApi().meGetAppointment(
      appointmentId: id,
    );
    return response.data!;
  });
}
