import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/presentation/appointment_providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_sayfalari.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/saat_ekrani.dart';
import 'package:omvila_musteri/features/booking/presentation/saat_secimi.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Saat değişikliği sorgusu.
@immutable
class _Query {
  const _Query(this.appointment, this.day);

  final MeAppointment appointment;
  final String day;

  @override
  bool operator ==(Object other) =>
      other is _Query &&
      other.appointment.id == appointment.id &&
      other.appointment.version == appointment.version &&
      other.day == day;

  @override
  int get hashCode => Object.hash(appointment.id, appointment.version, day);
}

final FutureProviderFamily<DaySearchResult, _Query> _daysProvider =
    FutureProvider.autoDispose.family<DaySearchResult, _Query>(
      (ref, q) => ref
          .watch(appointmentsRepositoryProvider)
          .rescheduleDays(
            q.appointment,
            from: q.day,
            to: addDays(q.day, dayStripDays),
          ),
    );

final FutureProviderFamily<SlotSearchResult, _Query> _slotsProvider =
    FutureProvider.autoDispose.family<SlotSearchResult, _Query>(
      (ref, q) => ref
          .watch(appointmentsRepositoryProvider)
          .rescheduleSlots(q.appointment.id, q.day),
    );

/// Saati değiştir (RandevuErtele): aynı hizmetler ve kişi; alınmış kapora
/// yeni saate aktarılır. Değiştirilince `true` ile kapanır.
class ErteleEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const ErteleEkrani({required this.id, super.key});

  /// Randevu.
  final String id;

  @override
  ConsumerState<ErteleEkrani> createState() => _ErteleEkraniState();
}

class _ErteleEkraniState extends ConsumerState<ErteleEkrani> {
  String? _date;
  Slot? _slot;
  bool _sending = false;
  ApiProblem? _error;

  Future<void> _save(MeAppointment a) async {
    setState(() {
      _sending = true;
      _error = null;
    });
    try {
      await ref
          .read(appointmentsRepositoryProvider)
          .reschedule(a, _slot!.startUtc);
      if (!mounted) {
        return;
      }

      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Yeni saat kaydedildi.')));
      context.pop(true);
    } on ApiProblem catch (e) {
      if (!mounted) {
        return;
      }

      setState(() {
        _sending = false;
        _error = e;
        _slot = null;
      });
      if (e.code == 'cutoff_passed') {
        await gecKaldinizGoster(context, a, iptal: false);
      } else {
        ref.invalidate(appointmentProvider(widget.id));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final appointment = ref.watch(appointmentProvider(widget.id));
    final a = appointment.value;
    final slot = _slot;
    return AkisSayfasi(
      baslik: 'Saati değiştir',
      onKapat: () => context.pop(),
      body: switch (appointment) {
        AsyncData(:final value) => _body(context, value),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(appointmentProvider(widget.id)),
        ),
        _ => const YukleniyorDurumu(),
      },
      alt: a == null || slot == null
          ? null
          : AltOzetCubugu(
              baslik:
                  '${formatDate(_date!, DateStyle.compact)} · '
                  '${slot.localTime}',
              altBaslik: bitisSaati(
                slot.startUtc.add(a.endUtc.difference(a.startUtc)),
                a.timeZone,
              ),
              eylem: 'Saati değiştir',
              yukleniyor: _sending,
              onPressed: () => _save(a),
            ),
    );
  }

  Widget _body(BuildContext context, MeAppointment a) {
    final r = context.omvilaRenkler;
    final today = branchToday(ref.watch(clockProvider), a.timeZone);
    final days = ref.watch(_daysProvider(_Query(a, today)));
    final deposit = a.deposit;
    final carried =
        deposit != null &&
        (deposit.status == DepositStatus.received ||
            deposit.status == DepositStatus.customerReported);
    final change = deadlineHours(a, a.rescheduleDeadlineUtc);
    final changeText = change == null
        ? ''
        : 'Saat değişikliği randevudan $change saat öncesine kadar '
              'yapılabilir. ';
    const approvalText = 'Salon onaylı çalışıyorsa yeni saat onaya düşer.';
    final error = _error;
    return switch (days) {
      AsyncData(value: final result) => () {
        final date = _date ?? firstAvailableDay(result);
        final slots = date == null
            ? null
            : ref.watch(_slotsProvider(_Query(a, date)));
        return ListView(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          children: [
            _SuAnki(randevu: a),
            const SizedBox(height: OmvilaBosluk.bosluk5),
            EkranBasligi(
              'Yeni saat seçin',
              aciklama:
                  'Aynı hizmetler ve aynı kişi · '
                  '${formatDuration(sureDakika(a.startUtc, a.endUtc))}',
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            SaatSecimi(
              days: result,
              date: date,
              slots: slots?.value,
              slotsError: slots?.error,
              selectedTime: _slot?.localTime,
              onDate: (d) => setState(() {
                _date = d;
                _slot = null;
              }),
              onSlot: (s) => setState(() {
                _date = date;
                _slot = s;
                _error = null;
              }),
            ),
            if (error != null) ...[
              const SizedBox(height: OmvilaBosluk.bosluk4),
              BilgiKutusu(
                ikon: LucideIcons.alertCircle,
                metin: error.message,
                zemin: r.hataZemin,
                renk: r.hata,
              ),
            ],
            if (carried) ...[
              const SizedBox(height: OmvilaBosluk.bosluk4),
              BilgiKutusu(
                metin:
                    '${formatMoney(deposit.required_.amountMinor)} kaporanız '
                    'yeni saate aktarılır, tekrar ödeme yapmazsınız.',
                zemin: r.bilgiZemin,
                renk: r.bilgi,
              ),
            ],
            const SizedBox(height: OmvilaBosluk.bosluk3),
            BilgiKutusu(
              ikon: LucideIcons.clock,
              metin: '$changeText$approvalText',
            ),
          ],
        );
      }(),
      AsyncError(:final error) => HataDurumu(
        error: error,
        onRetry: () => ref.invalidate(_daysProvider(_Query(a, today))),
      ),
      _ => const YukleniyorDurumu(),
    };
  }
}

class _SuAnki extends StatelessWidget {
  const _SuAnki({required this.randevu});

  final MeAppointment randevu;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final a = randevu;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminIkincil,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Padding(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        child: Row(
          children: [
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: r.zeminYukseltilmis,
                borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
              ),
              child: Icon(LucideIcons.calendarClock, size: 20, color: r.metin),
            ),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Şu anki randevunuz',
                    style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                  ),
                  Text(
                    '${formatDate(
                      toBranchDate(a.startUtc, a.timeZone),
                      DateStyle.compact,
                    )} · ${formatTime(a.startUtc, a.timeZone)}',
                    style: OmvilaYazi.govdeGuclu.copyWith(
                      color: r.metin,
                      decoration: TextDecoration.lineThrough,
                      decorationColor: r.metin,
                    ),
                  ),
                ],
              ),
            ),
            Flexible(
              child: Text(
                [
                  a.brand.name,
                  personelMetni(a.items),
                ].where((t) => t.isNotEmpty).join('\n'),
                textAlign: TextAlign.end,
                style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
