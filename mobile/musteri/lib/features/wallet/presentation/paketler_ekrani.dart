import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/shell/sekme_kabugu.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Paketlerim (meListPackages).
final FutureProvider<MePackageList> packagesProvider =
    FutureProvider.autoDispose<MePackageList>(
      (ref) => ref.watch(walletRepositoryProvider).packages(),
    );

/// Paketlerim sekmesi (Paketler, PaketlerBos): kullanılabilir paketler kum
/// kartlarda (seans çubuğu, "Az kaldı" sunucunun eşiğiyle, "Son kullanım"),
/// altta "Biten paketler". Paketler uygulamadan satılmaz.
class PaketlerEkrani extends ConsumerWidget {
  /// Ekran.
  const PaketlerEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final packages = ref.watch(packagesProvider);
    final value = packages.value;
    return SekmeSayfasi(
      ustBaslik: value == null || value.activeCount == 0
          ? null
          : '${value.activeCount} aktif paket',
      baslik: 'Paketlerim',
      onRefresh: () => ref.refresh(packagesProvider.future),
      children: switch (packages) {
        AsyncData(:final value) when value.items.isEmpty => const [
          OmvilaBosDurum(
            ikon: LucideIcons.package,
            baslik: 'Henüz paketiniz yok',
            aciklama:
                'Salonunuzdan bir seans paketi aldığınızda kalan seanslarınızı '
                've son kullanma tarihini burada takip edersiniz.',
          ),
          _BilgiKarti(
            'Paketler uygulamadan satılmaz; salonda satın alınır ve hesabınıza '
            'salon tarafından eklenir.',
          ),
        ],
        AsyncData(:final value) => _content(ref, value),
        AsyncError(:final error) => [
          HataDurumu(
            error: error,
            onRetry: () => ref.invalidate(packagesProvider),
          ),
        ],
        _ => const [
          Padding(
            padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
            child: YukleniyorDurumu(),
          ),
        ],
      },
    );
  }

  List<Widget> _content(WidgetRef ref, MePackageList list) {
    final usable = list.items.where((p) => p.isUsable).toList();
    final finished = list.items.where((p) => !p.isUsable).toList();
    final today = branchToday(ref.watch(clockProvider), _istanbul);
    return [
      for (final p in usable) ...[
        PaketKarti(paket: p, bugun: today),
        const SizedBox(height: OmvilaBosluk.bosluk3),
      ],
      const _BilgiKarti(
        'Paketler salondan satın alınır. Bir seans kullandığınızda burada '
        'otomatik düşer.',
      ),
      if (finished.isNotEmpty) ...[
        const SizedBox(height: OmvilaBosluk.bosluk5),
        Builder(
          builder: (context) => Semantics(
            header: true,
            child: Text(
              'Biten paketler',
              style: OmvilaYazi.baslik3.copyWith(
                color: context.omvilaRenkler.metin,
                fontWeight: FontWeight.w800,
              ),
            ),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        for (final p in finished) ...[
          _BitenSatir(paket: p, bugun: today),
          const SizedBox(height: OmvilaBosluk.bosluk2),
        ],
      ],
    ];
  }
}

// Paket günleri yerel gündür (format: date); "bu yıl mı" karşılaştırması
// için varsayılan saat dilimi yeterlidir (yanıt şube saat dilimi taşımaz).
const _istanbul = 'Europe/Istanbul';

/// "31 Aralık"; başka yılsa "15 Mart 2027".
String gunAyMetni(String localDate, String today) {
  final text = formatDate(localDate, DateStyle.dayMonth);
  return localDate.substring(0, 4) == today.substring(0, 4)
      ? text
      : '$text ${localDate.substring(0, 4)}';
}

/// Kullanılabilir paket kartı (kum zemin).
class PaketKarti extends StatelessWidget {
  /// Kart; [bugun] yıl karşılaştırması için.
  const PaketKarti({required this.paket, required this.bugun, super.key});

  /// Paket.
  final MePackage paket;

  /// Bugünün yerel günü.
  final String bugun;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final p = paket;
    final ink = r.blokKumMetin;
    return MergeSemantics(
      child: Container(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
        decoration: BoxDecoration(
          color: r.blokKum,
          borderRadius: BorderRadius.circular(OmvilaKose.koseXl),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        p.packageName,
                        style: OmvilaYazi.baslik3.copyWith(
                          color: ink,
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                      Text(
                        ozetSatiri([p.brand.name, p.branchName ?? '']),
                        style: OmvilaYazi.ikincil.copyWith(color: ink),
                      ),
                    ],
                  ),
                ),
                if (p.isRunningLow)
                  EtiketCipi(
                    'Az kaldı',
                    zemin: r.zeminYukseltilmis,
                    renk: r.metin,
                  ),
              ],
            ),
            const SizedBox(height: OmvilaBosluk.bosluk3),
            Text.rich(
              TextSpan(
                children: [
                  TextSpan(
                    text: '${p.remainingSessions}',
                    style: OmvilaYazi.metrik.copyWith(color: ink),
                  ),
                  TextSpan(
                    text: ' / ${p.totalSessions} seans kaldı',
                    style: OmvilaYazi.govde.copyWith(color: ink),
                  ),
                ],
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
            SeansCubugu(toplam: p.totalSessions, kalan: p.remainingSessions),
            const SizedBox(height: OmvilaBosluk.bosluk3),
            Row(
              children: [
                Icon(LucideIcons.clock, size: 16, color: ink),
                const SizedBox(width: OmvilaBosluk.bosluk2),
                Expanded(
                  child: Text(
                    'Son kullanım '
                    '${gunAyMetni(localDateOf(p.expiresOn), bugun)}',
                    style: OmvilaYazi.ikincil.copyWith(color: ink),
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

/// Seans çubuğu: kullanılanlar açık, kalanlar koyu (sonda). Çok seanslı
/// pakette dilimler okunmaz hale geleceği için tek sürekli çubuk.
class SeansCubugu extends StatelessWidget {
  /// Çubuk.
  const SeansCubugu({required this.toplam, required this.kalan, super.key});

  /// Toplam seans.
  final int toplam;

  /// Kalan seans.
  final int kalan;

  static const _maxSegments = 12;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final used = r.blokKumMetin.withValues(alpha: 0.16);
    final left = r.zeminMarka;
    final radius = BorderRadius.circular(OmvilaKose.koseTam);
    if (toplam <= 0) {
      return const SizedBox.shrink();
    }

    if (toplam > _maxSegments) {
      return ClipRRect(
        borderRadius: radius,
        child: LinearProgressIndicator(
          value: kalan / toplam,
          minHeight: 6,
          backgroundColor: used,
          color: left,
        ),
      );
    }

    return ExcludeSemantics(
      child: Row(
        children: [
          for (var i = 0; i < toplam; i++) ...[
            if (i > 0) const SizedBox(width: OmvilaBosluk.bosluk1),
            Expanded(
              child: Container(
                height: 6,
                decoration: BoxDecoration(
                  color: i >= toplam - kalan ? left : used,
                  borderRadius: radius,
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _BitenSatir extends StatelessWidget {
  const _BitenSatir({required this.paket, required this.bugun});

  final MePackage paket;
  final String bugun;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final p = paket;
    final finished = p.finishedOn;
    String? when;
    if (finished != null) {
      final day = localDateOf(finished);
      if (day.substring(0, 4) == bugun.substring(0, 4)) {
        final month = formatDate(day, DateStyle.monthYear).split(' ').first;
        when = '${bulunma(month)} bitti';
      } else {
        when = '${formatDate(day, DateStyle.numeric)} tarihinde bitti';
      }
    }

    return MergeSemantics(
      child: Container(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        decoration: BoxDecoration(
          color: r.zeminYukseltilmis,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '${p.packageName} · ${p.totalSessions} seans',
                    style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                  ),
                  Text(
                    ozetSatiri([p.brand.name, when ?? '']),
                    style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                  ),
                ],
              ),
            ),
            EtiketCipi('Bitti', zemin: r.zeminIkincil, renk: r.metinIkincil),
          ],
        ),
      ),
    );
  }
}

class _BilgiKarti extends StatelessWidget {
  const _BilgiKarti(this.metin);

  final String metin;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Row(
        children: [
          IkonKutusu(
            LucideIcons.info,
            zemin: r.zeminMarkaYumusak,
            renk: r.metinMarka,
          ),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Text(
              metin,
              style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
            ),
          ),
        ],
      ),
    );
  }
}
