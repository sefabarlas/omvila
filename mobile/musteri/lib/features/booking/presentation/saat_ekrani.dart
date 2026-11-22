import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/personel_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/saat_secimi.dart';
import 'package:omvila_musteri/features/waitlist/presentation/bekleme_katil_sayfasi.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Müsaitlik sorgusu (gün şeridi veya bir gün).
@immutable
class AvailabilityQuery {
  /// [draft]'ın şube ve kalemleriyle.
  AvailabilityQuery(BookingDraft draft, this.day, {this.to})
    : branchId = draft.branchId,
      items = draft.items,
      sameStaffForAll = draft.sameStaffForAll;

  /// Şube.
  final String branchId;

  /// Kalemler.
  final List<AppointmentItemRequest> items;

  /// Aynı kişi.
  final bool sameStaffForAll;

  /// Gün; [to] doluysa aralığın başı.
  final String day;

  /// Aralığın sonu (gün şeridi).
  final String? to;

  String get _key =>
      '$branchId|$sameStaffForAll|$day|$to|'
      '${items.map((i) => '${i.serviceId}:${i.staffId}').join(',')}';

  @override
  bool operator ==(Object other) =>
      other is AvailabilityQuery && other._key == _key;

  @override
  int get hashCode => _key.hashCode;
}

/// Gün şeridi.
final FutureProviderFamily<DaySearchResult, AvailabilityQuery> daysProvider =
    FutureProvider.autoDispose.family<DaySearchResult, AvailabilityQuery>(
      (ref, q) => ref
          .watch(bookingRepositoryProvider)
          .days(
            q.branchId,
            q.items,
            sameStaffForAll: q.sameStaffForAll,
            from: q.day,
            to: q.to!,
          ),
    );

/// Bir günün saatleri.
final FutureProviderFamily<SlotSearchResult, AvailabilityQuery> slotsProvider =
    FutureProvider.autoDispose.family<SlotSearchResult, AvailabilityQuery>(
      (ref, q) => ref
          .watch(bookingRepositoryProvider)
          .slots(
            q.branchId,
            q.items,
            sameStaffForAll: q.sameStaffForAll,
            date: q.day,
          ),
    );

/// Gün şeridinin uzunluğu (components/DayStrip: bugün ve 30 gün ileri; uç en
/// çok 31 gün kabul eder).
const dayStripDays = 30;

/// İlk boş gün; yoksa ilk gün.
String? firstAvailableDay(DaySearchResult days) {
  final day =
      days.days.where((d) => d.hasAvailability).firstOrNull ??
      days.days.firstOrNull;
  return day == null ? null : localDateOf(day.date);
}

/// "Elif Kaya · 15:45'te biter".
String bitisMetni(String? kisi, Slot slot, String timeZone) {
  final end = slot.items.isEmpty
      ? null
      : slot.items.map((i) => i.endUtc).reduce((a, b) => a.isAfter(b) ? a : b);
  final endText = end == null ? slot.endLocalTime : formatTime(end, timeZone);
  return [
    ?kisi,
    if (endText != null) "$endText'${saatEki(endText)} biter",
  ].join(' · ');
}

/// Ne zaman? (YRSaat): gün şeridi, ızgara, "Uygun saat yok mu?" bekleme kartı.
class SaatEkrani extends ConsumerWidget {
  /// Ekran.
  const SaatEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final draft = ref.watch(bookingProvider)!;
    final catalog = ref.watch(catalogProvider(draft.branchId));
    return AkisSayfasi(
      baslik: draft.brandName,
      adim: AkisAdimi.saat,
      body: switch (catalog) {
        AsyncData(:final value) => _Govde(catalog: value, draft: draft),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(catalogProvider(draft.branchId)),
        ),
        _ => const YukleniyorDurumu(),
      },
      alt: switch ((catalog.value, draft.slot)) {
        (final PublicCatalog c, final Slot slot) => AltOzetCubugu(
          baslik:
              '${formatDate(draft.date!, DateStyle.compact)} · '
              '${slot.localTime}',
          altBaslik: bitisMetni(kisiMetni(c, draft), slot, c.timeZone),
          eylem: 'Devam et',
          onPressed: () => context.push(Routes.yrOnay),
        ),
        _ => null,
      },
    );
  }
}

class _Govde extends ConsumerWidget {
  const _Govde({required this.catalog, required this.draft});

  final PublicCatalog catalog;
  final BookingDraft draft;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final today = branchToday(ref.watch(clockProvider), catalog.timeZone);
    final daysQuery = AvailabilityQuery(
      draft,
      today,
      to: addDays(today, dayStripDays),
    );
    final days = ref.watch(daysProvider(daysQuery));
    final summary = BookingSummary.of(catalog, draft);
    return switch (days) {
      AsyncData(value: final result) => () {
        final date = draft.date ?? firstAvailableDay(result);
        final slotsQuery = date == null ? null : AvailabilityQuery(draft, date);
        final slots = slotsQuery == null
            ? null
            : ref.watch(slotsProvider(slotsQuery));
        final controller = ref.read(bookingProvider.notifier);
        return ListView(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          children: [
            EkranBasligi(
              'Ne zaman?',
              aciklama:
                  '${kisiMetni(catalog, draft)} · '
                  '${formatDuration(summary.durationMinutes)}',
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            SaatSecimi(
              days: result,
              date: date,
              slots: slots?.value,
              slotsError: slots?.error,
              selectedTime: draft.slot?.localTime,
              onDate: controller.chooseDate,
              onSlot: (slot) {
                if (draft.date == null && date != null) {
                  controller.chooseDate(date);
                }

                controller.chooseSlot(slot);
              },
            ),
            const SizedBox(height: OmvilaBosluk.bosluk5),
            _BeklemeKarti(
              onTap: () async {
                final joined = await beklemeKatilGoster(context);
                if ((joined ?? false) && context.mounted) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text(
                        'Bekleme listesine katıldınız. Uygun saat boşalınca '
                        'haber vereceğiz.',
                      ),
                    ),
                  );
                }
              },
            ),
          ],
        );
      }(),
      AsyncError(:final error) => HataDurumu(
        error: error,
        onRetry: () => ref.invalidate(daysProvider(daysQuery)),
      ),
      _ => const YukleniyorDurumu(),
    };
  }
}

/// "Uygun saat yok mu?" (YRSaat): bekleme listesine katılma.
class _BeklemeKarti extends StatelessWidget {
  const _BeklemeKarti({required this.onTap});

  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final radius = BorderRadius.circular(OmvilaKose.koseXl);
    return Material(
      color: r.blokKum,
      borderRadius: radius,
      child: InkWell(
        borderRadius: radius,
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: Row(
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: r.blokKumMetin.withValues(alpha: 0.08),
                  borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                ),
                child: Icon(
                  LucideIcons.listChecks,
                  size: 20,
                  color: r.blokKumMetin,
                ),
              ),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Uygun saat yok mu?',
                      style: OmvilaYazi.govdeGuclu.copyWith(
                        color: r.blokKumMetin,
                      ),
                    ),
                    Text(
                      'Bekleme listesine katılın, boşalınca haber verelim.',
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.blokKumMetin,
                      ),
                    ),
                  ],
                ),
              ),
              Icon(LucideIcons.chevronRight, size: 20, color: r.blokKumMetin),
            ],
          ),
        ),
      ),
    );
  }
}
