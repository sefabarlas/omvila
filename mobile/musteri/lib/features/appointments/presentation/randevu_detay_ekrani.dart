import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/presentation/appointment_providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_sayfalari.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_musteri/features/booking/presentation/akis_sayfasi.dart';
import 'package:omvila_musteri/features/booking/presentation/booking_controller.dart';
import 'package:omvila_musteri/features/booking/presentation/onay_ekrani.dart';
import 'package:omvila_musteri/shared/external_actions.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// SMS bağlantısındaki kodun randevusu (`/r/{code}`).
final FutureProviderFamily<MeAppointment, String> appointmentByCodeProvider =
    FutureProvider.autoDispose.family<MeAppointment, String>(
      (ref, code) =>
          ref.watch(appointmentsRepositoryProvider).appointmentByCode(code),
    );

/// Randevu sonrası listeleri tazeler.
void tazele(WidgetRef ref, String id) => ref
  ..invalidate(appointmentProvider(id))
  ..invalidate(homeProvider);

/// Yol tarifi.
Future<void> yolTarifi(BuildContext context, WidgetRef ref, MeAppointment a) {
  final branch = a.branch;
  return ref
      .read(externalActionsProvider)
      .openUrl(
        directionsUrl(
          address: '${branch.addressLine}, ${branch.district}',
          latitude: branch.latitude,
          longitude: branch.longitude,
          platform: Theme.of(context).platform,
        ),
      );
}

/// Takvime ekle.
Future<void> takvimeEkle(WidgetRef ref, MeAppointment a) => ref
    .read(externalActionsProvider)
    .addToCalendar(
      CalendarEntry(
        title: '${a.brand.name} randevusu',
        startUtc: a.startUtc,
        endUtc: a.endUtc,
        timeZone: a.timeZone,
        description: ozetSatiri([
          hizmetlerMetni(a.items),
          personelMetni(a.items),
        ]),
        location: '${a.branch.addressLine}, ${a.branch.district}',
      ),
    );

/// Randevu detayı (RandevuDetay).
class RandevuDetayEkrani extends ConsumerWidget {
  /// Ekran.
  const RandevuDetayEkrani({required this.id, super.key});

  /// Randevu kimliği.
  final String id;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final appointment = ref.watch(appointmentProvider(id));
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.canPop()
              ? context.pop()
              : context.go(Routes.randevularim),
        ),
      ),
      body: switch (appointment) {
        AsyncData(:final value) => RefreshIndicator(
          onRefresh: () => ref.refresh(appointmentProvider(id).future),
          child: _Detay(randevu: value),
        ),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(appointmentProvider(id)),
        ),
        _ => const YukleniyorDurumu(),
      },
    );
  }
}

/// `/r/{code}`: kod bu müşterinin randevusuysa detay, değilse "Bu bağlantı
/// geçersiz".
class KodluRandevuEkrani extends ConsumerWidget {
  /// Ekran.
  const KodluRandevuEkrani({required this.code, super.key});

  /// Bağlantı kodu.
  final String code;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final result = ref.watch(appointmentByCodeProvider(code));
    return switch (result) {
      AsyncData(:final value) => RandevuDetayEkrani(id: value.id),
      AsyncError(error: ApiProblem(status: 404)) =>
        const BaglantiGecersizEkrani(),
      AsyncError(:final error) => Scaffold(
        body: HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(appointmentByCodeProvider(code)),
        ),
      ),
      _ => const Scaffold(body: YukleniyorDurumu()),
    };
  }
}

class _Detay extends ConsumerWidget {
  const _Detay({required this.randevu});

  final MeAppointment randevu;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final clock = ref.watch(clockProvider);
    final active = isActive(a);
    final deposit = a.deposit;
    final depositText = kaporaMetni(deposit);
    final note = sureNotu(a);
    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk4,
        0,
        OmvilaBosluk.bosluk4,
        OmvilaBosluk.bosluk8,
      ),
      children: [
        _Kahraman(randevu: a, clock: clock),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        _SalonKarti(randevu: a),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        SatirKarti(
          satirlar: [
            for (final i in a.items)
              (
                ozetSatiri([
                  i.serviceName,
                  i.staffName?.split(' ').first ?? '',
                ]),
                i.paidFromPackage
                    ? 'Paketten'
                    : formatMoney(i.price.amountMinor),
                false,
              ),
            if (depositText != null) ('Kapora', depositText, false),
            ('Salonda ödenecek', formatMoney(a.amountDue.amountMinor), false),
          ],
        ),
        if (deposit?.status == DepositStatus.received) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            "Kapora salonun IBAN'ına havale ile ödendi ve salon tarafından "
            'onaylandı. İade koşullarını salon belirler.',
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
        ],
        if (active && deposit?.instructions != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          OmvilaButon(
            etiket: deposit!.status == DepositStatus.customerReported
                ? 'Havale bilgilerini gör'
                : 'Kaporayı gönder',
            ikon: LucideIcons.landmark,
            tamGenislik: true,
            onPressed: () async {
              await context.push<void>(Routes.havale(a.id));
              tazele(ref, a.id);
            },
          ),
        ],
        if (a.preAppointmentNote case final n? when n.isNotEmpty) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          BilgiKutusu(metin: n, ikon: LucideIcons.messageSquare),
        ],
        if (a.displayStatus == DisplayStatus.cancelled) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          const BilgiKutusu(
            ikon: LucideIcons.calendarX,
            metin: 'Bu randevu iptal edildi.',
          ),
        ],
        if (active) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          OmvilaButon(
            etiket: 'Saati değiştir',
            ikon: LucideIcons.calendarClock,
            tur: OmvilaButonTuru.ikincil,
            tamGenislik: true,
            onPressed: () async {
              if (!a.canReschedule) {
                await gecKaldinizGoster(context, a, iptal: false);
                return;
              }

              final changed = await context.push<bool>(Routes.ertele(a.id));
              if (changed ?? false) {
                tazele(ref, a.id);
              }
            },
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          TextButton(
            style: TextButton.styleFrom(foregroundColor: r.hata),
            onPressed: () async {
              if (!a.canCancel) {
                await gecKaldinizGoster(context, a, iptal: true);
                return;
              }

              final cancelled = await iptalGoster(context, a);
              if ((cancelled ?? false) && context.mounted) {
                tazele(ref, a.id);
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('Randevunuz iptal edildi.')),
                );
              }
            },
            child: const Text('Randevuyu iptal et'),
          ),
          if (note != null)
            Text(
              note,
              textAlign: TextAlign.center,
              style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
            ),
        ],
        if (a.canReview) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          OmvilaButon(
            etiket: 'Değerlendir',
            ikon: LucideIcons.star,
            tamGenislik: true,
            onPressed: () async {
              await context.push<void>(Routes.degerlendir(a.id));
              tazele(ref, a.id);
            },
          ),
        ],
        if (!active && a.canRebook) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          OmvilaButon(
            etiket: 'Aynısını tekrar al',
            ikon: LucideIcons.repeat,
            tur: OmvilaButonTuru.ikincil,
            tamGenislik: true,
            onPressed: () => aynisiniTekrarAl(
              context,
              ref,
              BookingDraft.rebook(
                brand: a.brand,
                branchId: a.branch.id,
                branchName: a.branch.name,
                items: a.items,
              ),
            ),
          ),
        ],
      ],
    );
  }
}

class _Kahraman extends StatelessWidget {
  const _Kahraman({required this.randevu, required this.clock});

  final MeAppointment randevu;
  final Clock clock;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final onCard = r.metinTers;
    final day = toBranchDate(a.startUtc, a.timeZone);
    final relative = gunEtiketi(a.startUtc, a.timeZone, clock);
    final long = formatDate(day);
    final top = relative == 'Bugün' || relative == 'Yarın'
        ? '$relative · $long'
        : long;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
      decoration: BoxDecoration(
        color: r.blokMarka,
        borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Text(
                  toUpperTr(top),
                  style: OmvilaYazi.aciklama.copyWith(
                    color: onCard,
                    fontWeight: FontWeight.w700,
                    letterSpacing: 0.6,
                  ),
                ),
              ),
              if (status != null) OmvilaDurumRozeti(durum: status),
            ],
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            formatTime(a.startUtc, a.timeZone),
            style: OmvilaYazi.metrik.copyWith(color: onCard, fontSize: 48),
          ),
          Text(
            '${formatDuration(sureDakika(a.startUtc, a.endUtc))} · '
            '${bitisSaati(a.endUtc, a.timeZone)}',
            style: OmvilaYazi.ikincil.copyWith(
              color: onCard.withValues(alpha: 0.85),
            ),
          ),
        ],
      ),
    );
  }
}

class _SalonKarti extends ConsumerWidget {
  const _SalonKarti({required this.randevu});

  final MeAppointment randevu;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final phone = a.branch.phone;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        boxShadow: context.omvilaGolgeler.golgeSm,
      ),
      child: Padding(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        child: Column(
          children: [
            Row(
              children: [
                SalonRozeti(ad: a.brand.name),
                const SizedBox(width: OmvilaBosluk.bosluk3),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        a.brand.name,
                        style: OmvilaYazi.baslik3.copyWith(color: r.metin),
                      ),
                      Text(
                        '${a.branch.addressLine}, ${a.branch.district}',
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: OmvilaBosluk.bosluk3),
            Row(
              spacing: OmvilaBosluk.bosluk2,
              children: [
                Expanded(
                  child: OmvilaButon(
                    etiket: 'Yol tarifi',
                    ikon: LucideIcons.mapPin,
                    tur: OmvilaButonTuru.ikincil,
                    tamGenislik: true,
                    onPressed: () => yolTarifi(context, ref, a),
                  ),
                ),
                if (phone != null)
                  Expanded(
                    child: OmvilaButon(
                      etiket: 'Ara',
                      ikon: LucideIcons.phone,
                      tur: OmvilaButonTuru.ikincil,
                      tamGenislik: true,
                      onPressed: () => salonuAra(ref, phone),
                    ),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

/// Salonun baş harf rozeti (52×52).
class SalonRozeti extends StatelessWidget {
  /// Rozet.
  const SalonRozeti({required this.ad, this.boyut = 48, super.key});

  /// Salon adı.
  final String ad;

  /// Kenar.
  final double boyut;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Container(
      width: boyut,
      height: boyut,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: r.avatar1Zemin,
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
      ),
      child: Text(
        initials(ad),
        style: OmvilaYazi.govdeGuclu.copyWith(color: r.avatar1Metin),
      ),
    );
  }
}
