import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/presentation/appointment_providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_sayfalari.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Başlık ve açıklama randevunun gerçek durumundan (T-108): tasarımda yalnız
/// "onaylandı" var; onay bekleyen ve kapora bekleyen randevuya "onaylandı"
/// denmez (S-03).
(String, String) basariMetni(MeAppointment a) {
  final deposit = a.deposit?.status;
  return switch (a.displayStatus) {
    DisplayStatus.confirmed => (
      'Randevunuz onaylandı',
      'Randevudan önce size hatırlatma göndereceğiz.',
    ),
    DisplayStatus.depositPending
        when deposit == DepositStatus.customerReported =>
      (
        'Havaleniz salona bildirildi',
        'Salon onayı bekleniyor. Salon havaleyi görünce onaylar, size SMS '
            'gelir.',
      ),
    DisplayStatus.depositPending => (
      'Kaporanız bekleniyor',
      'Randevunuz kapora gelince kesinleşir. Havale bilgileri randevu '
          'detayında.',
    ),
    _ => (
      'Talebiniz alındı',
      'Salon onaylayınca SMS ile bildireceğiz.',
    ),
  };
}

/// Randevu oluşturuldu (YRBasari): kart, "Takvime ekle", "Yol tarifi",
/// "Paylaş", "Randevularıma dön".
class BasariEkrani extends ConsumerWidget {
  /// Ekran.
  const BasariEkrani({required this.id, super.key});

  /// Randevu.
  final String id;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final appointment = ref.watch(appointmentProvider(id));
    return Scaffold(
      backgroundColor: r.zemin,
      body: SafeArea(
        child: switch (appointment) {
          AsyncData(:final value) => _Govde(randevu: value),
          AsyncError(:final error) => HataDurumu(
            error: error,
            onRetry: () => ref.invalidate(appointmentProvider(id)),
          ),
          _ => const YukleniyorDurumu(),
        },
      ),
      bottomNavigationBar: SafeArea(
        top: false,
        child: Padding(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          child: OmvilaButon(
            etiket: 'Randevularıma dön',
            boyut: OmvilaButonBoyutu.lg,
            tamGenislik: true,
            onPressed: () => context.go(Routes.randevularim),
          ),
        ),
      ),
    );
  }
}

class _Govde extends ConsumerWidget {
  const _Govde({required this.randevu});

  final MeAppointment randevu;

  Future<void> _share(BuildContext context) async {
    final a = randevu;
    final messenger = ScaffoldMessenger.of(context);
    final day = formatDate(toBranchDate(a.startUtc, a.timeZone));
    final text = [
      '${a.brand.name} randevusu',
      '$day ${formatTime(a.startUtc, a.timeZone)}',
      '${a.branch.addressLine}, ${a.branch.district}',
    ].join('\n');
    await Clipboard.setData(ClipboardData(text: text));
    messenger.showSnackBar(
      const SnackBar(
        content: Text(
          'Randevu bilgisi kopyalandı; dilediğiniz yere yapıştırın.',
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final a = randevu;
    final (title, subtitle) = basariMetni(a);
    final confirmed = a.displayStatus == DisplayStatus.confirmed;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    return ListView(
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk5,
        OmvilaBosluk.bosluk12,
        OmvilaBosluk.bosluk5,
        OmvilaBosluk.bosluk5,
      ),
      children: [
        Align(
          alignment: Alignment.centerLeft,
          child: Container(
            width: 80,
            height: 80,
            decoration: BoxDecoration(
              color: confirmed ? r.blokMarka : r.zeminIkincil,
              borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
            ),
            child: Icon(
              confirmed ? LucideIcons.check : LucideIcons.hourglass,
              color: confirmed ? r.metinTers : r.metin,
            ),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        Semantics(
          header: true,
          child: Text(
            title,
            style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Text(
          subtitle,
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        RandevuKarti(
          salonAdi: a.brand.name,
          hizmetVePersonel: ozetSatiri([
            hizmetlerMetni(a.items),
            personelMetni(a.items),
          ]),
          tarih: formatDate(
            toBranchDate(a.startUtc, a.timeZone),
            DateStyle.short,
          ),
          saat: formatTime(a.startUtc, a.timeZone),
          sure: formatDuration(sureDakika(a.startUtc, a.endUtc)),
          durum: status,
          tutar: a.amountDue.amountMinor > 0
              ? '${formatMoney(a.amountDue.amountMinor)} salonda'
              : null,
          eylemler: [
            if (a.canCancel && isActive(a))
              TextButton(
                onPressed: () async {
                  final cancelled = await iptalGoster(context, a);
                  if ((cancelled ?? false) && context.mounted) {
                    tazele(ref, a.id);
                  }
                },
                child: const Text('İptal et'),
              ),
            FilledButton.icon(
              onPressed: () => takvimeEkle(ref, a),
              icon: const Icon(LucideIcons.calendarPlus, size: 18),
              label: const Text('Takvime ekle'),
              style: FilledButton.styleFrom(
                backgroundColor: r.zeminIkincil,
                foregroundColor: r.metin,
                minimumSize: const Size(44, OmvilaBoyut.kontrolSm),
                textStyle: OmvilaYazi.etiket,
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
            Expanded(
              child: OmvilaButon(
                etiket: 'Paylaş',
                ikon: LucideIcons.share2,
                tur: OmvilaButonTuru.ikincil,
                tamGenislik: true,
                onPressed: () => _share(context),
              ),
            ),
          ],
        ),
        if (a.deposit?.instructions != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          OmvilaButon(
            etiket: 'Havale bilgilerini gör',
            ikon: LucideIcons.landmark,
            tur: OmvilaButonTuru.hayalet,
            tamGenislik: true,
            onPressed: () => context.push(Routes.havale(a.id)),
          ),
        ],
      ],
    );
  }
}
