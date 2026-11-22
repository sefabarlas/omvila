import 'package:flutter/foundation.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/appointments/domain/review_target.dart';
import 'package:omvila_musteri/shared/api_call.dart';
import 'package:omvila_ui/omvila_ui.dart';

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

  /// Geçmiş randevular (azalan); [notReviewed] "Değerlendirilmedi".
  Future<MeAppointmentPage> past({bool notReviewed = false, String? cursor});

  /// SMS bağlantısındaki kodun randevusu (`/r/{code}`); kod bu müşterinin
  /// değilse `not_found`.
  Future<MeAppointment> appointmentByCode(String accessCode);

  /// İptal (neden zorunlu; If-Match).
  Future<MeAppointment> cancel(
    MeAppointment appointment,
    CustomerCancelRequestReasonEnum reason,
  );

  /// Saat değişikliği gün şeridi: aynı hizmetler ve personel.
  Future<DaySearchResult> rescheduleDays(
    MeAppointment appointment, {
    required String from,
    required String to,
  });

  /// Saat değişikliği için bir günün saatleri.
  Future<SlotSearchResult> rescheduleSlots(String id, String date);

  /// Yeni saat (If-Match).
  Future<MeAppointment> reschedule(
    MeAppointment appointment,
    DateTime startUtc,
  );

  /// "Havaleyi yaptım, salona bildir".
  Future<MeAppointment> reportDeposit(String id);

  /// Değerlendirme hedefi; [appointmentId] veya [accessCode].
  Future<ReviewTarget> reviewTarget({
    String? appointmentId,
    String? accessCode,
  });

  /// Değerlendirmeyi gönderir.
  Future<void> submitReview(ReviewTarget target, int rating, String? comment);
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

  @override
  Future<MeAppointmentPage> past({bool notReviewed = false, String? cursor}) =>
      apiCall(() async {
        final response = await api.getMeAppointmentsApi().meListAppointments(
          scope: 'Past',
          notReviewed: notReviewed ? true : null,
          cursor: cursor,
        );
        return response.data!;
      });

  @override
  Future<MeAppointment> appointmentByCode(String accessCode) =>
      apiCall(() async {
        final link = await api
            .getPublicAppointmentLinkApi()
            .publicGetAppointmentByCode(accessCode: accessCode);
        final response = await api.getMeAppointmentsApi().meGetAppointment(
          appointmentId: link.data!.id,
        );
        return response.data!;
      });

  @override
  Future<MeAppointment> cancel(
    MeAppointment appointment,
    CustomerCancelRequestReasonEnum reason,
  ) => apiCall(() async {
    final response = await api.getMeAppointmentsApi().meCancelAppointment(
      ifMatch: ifMatch(appointment.version),
      appointmentId: appointment.id,
      customerCancelRequest: CustomerCancelRequest(reason: reason),
    );
    return response.data!;
  });

  @override
  Future<DaySearchResult> rescheduleDays(
    MeAppointment appointment, {
    required String from,
    required String to,
  }) => apiCall(() async {
    final staff = {for (final i in appointment.items) i.staffId};
    final response = await api.getPublicBookingApi().publicSearchDays(
      branchId: appointment.branch.id,
      daySearchRequest: DaySearchRequest(
        items: [
          for (final i in appointment.items)
            AppointmentItemRequest(serviceId: i.serviceId, staffId: i.staffId),
        ],
        sameStaffForAll: staff.length == 1,
        from: ApiDate(from),
        to: ApiDate(to),
      ),
    );
    return response.data!;
  });

  @override
  Future<SlotSearchResult> rescheduleSlots(String id, String date) => apiCall(
    () async {
      final response = await api.getMeAppointmentsApi().meSearchRescheduleSlots(
        appointmentId: id,
        meRescheduleSlotSearch: MeRescheduleSlotSearch(
          date: ApiDate(date),
        ),
      );
      return response.data!;
    },
  );

  @override
  Future<MeAppointment> reschedule(
    MeAppointment appointment,
    DateTime startUtc,
  ) => apiCall(() async {
    final response = await api.getMeAppointmentsApi().meRescheduleAppointment(
      ifMatch: ifMatch(appointment.version),
      appointmentId: appointment.id,
      meRescheduleRequest: MeRescheduleRequest(startUtc: startUtc),
    );
    return response.data!;
  });

  @override
  Future<MeAppointment> reportDeposit(String id) => apiCall(() async {
    final response = await api.getMeAppointmentsApi().meReportDepositTransfer(
      appointmentId: id,
    );
    return response.data!;
  });

  @override
  Future<ReviewTarget> reviewTarget({
    String? appointmentId,
    String? accessCode,
  }) => apiCall(() async {
    if (accessCode != null) {
      final invite = (await api.getPublicReviewApi().publicGetReviewInvite(
        accessCode: accessCode,
      )).data!;
      return ReviewTarget(
        accessCode: accessCode,
        brandName: invite.brand.name,
        serviceNames: invite.serviceNames,
        staffName: invite.staff?.name,
        startUtc: invite.appointmentStartUtc,
        timeZone: invite.timeZone,
        displayName: invite.customerDisplayName,
        existingRating: invite.review?.rating,
        existingComment: invite.review?.comment,
      );
    }

    final (appointment, profile) = await (
      api.getMeAppointmentsApi().meGetAppointment(
        appointmentId: appointmentId!,
      ),
      api.getMeProfileApi().meGetProfile(),
    ).wait;
    final a = appointment.data!;
    return ReviewTarget(
      appointmentId: a.id,
      brandName: a.brand.name,
      serviceNames: [for (final i in a.items) i.serviceName],
      staffName: a.items
          .map((i) => i.staffName)
          .firstWhere((n) => n != null, orElse: () => null),
      startUtc: a.startUtc,
      timeZone: a.timeZone,
      displayName: profile.data!.reviewerDisplayName,
      existingRating: a.review?.rating,
    );
  });

  @override
  Future<void> submitReview(
    ReviewTarget target,
    int rating,
    String? comment,
  ) => apiCall(() async {
    final code = target.accessCode;
    if (code != null) {
      await api.getPublicReviewApi().publicCreateReview(
        idempotencyKey: newIdempotencyKey(),
        accessCode: code,
        publicReviewCreate: PublicReviewCreate(
          rating: rating,
          comment: comment,
        ),
      );
      return;
    }

    await api.getMeAppointmentsApi().meCreateReview(
      idempotencyKey: newIdempotencyKey(),
      meReviewCreate: MeReviewCreate(
        appointmentId: target.appointmentId!,
        rating: rating,
        comment: comment,
      ),
    );
  });
}
