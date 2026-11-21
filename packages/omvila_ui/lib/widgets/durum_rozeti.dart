import 'package:flutter/material.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// Randevu durumunun arayüz gösterimi (docs/alan/durum-makinesi.md § Arayüz
/// gösterimi). "Kapora bekleniyor" türetilmiş gösterimdir; `Draft` yoktur.
enum RandevuDurumu {
  /// PendingApproval
  onayBekliyor('Onay bekliyor'),

  /// DepositPending (PendingApproval + kapora bekleniyor)
  kaporaBekleniyor('Kapora bekleniyor'),

  /// Confirmed
  onaylandi('Onaylandı'),

  /// CheckedIn
  geldi('Geldi'),

  /// Completed
  tamamlandi('Tamamlandı'),

  /// NoShow
  gelmedi('Gelmedi'),

  /// Cancelled
  iptal('İptal');

  const RandevuDurumu(this.etiket);

  /// Sabit durum adı (AGENTS.md § 5).
  final String etiket;

  /// API'nin `displayStatus` alanından. Bilinmeyen değer (yeni sunucu) null
  /// döner; uygulama rozeti göstermez.
  static RandevuDurumu? fromDisplayStatus(DisplayStatus status) =>
      switch (status) {
        DisplayStatus.pendingApproval => onayBekliyor,
        DisplayStatus.depositPending => kaporaBekleniyor,
        DisplayStatus.confirmed => onaylandi,
        DisplayStatus.checkedIn => geldi,
        DisplayStatus.completed => tamamlandi,
        DisplayStatus.noShow => gelmedi,
        DisplayStatus.cancelled => iptal,
        DisplayStatus.unknownDefaultOpenApi => null,
      };
}

/// Durum rozeti: nokta ve durum adı; renk yalnız `durum-*` tokenlarından.
/// Durum yalnız renkle anlatılmaz, ad her zaman yazılır.
class OmvilaDurumRozeti extends StatelessWidget {
  /// [durum] için rozet.
  const OmvilaDurumRozeti({required this.durum, super.key});

  /// Gösterilen durum.
  final RandevuDurumu durum;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final (zemin, metin, nokta) = switch (durum) {
      RandevuDurumu.onayBekliyor => (
        r.durumOnayZemin,
        r.durumOnayMetin,
        r.durumOnayNokta,
      ),
      RandevuDurumu.kaporaBekleniyor => (
        r.durumKaporaZemin,
        r.durumKaporaMetin,
        r.durumKaporaNokta,
      ),
      RandevuDurumu.onaylandi => (
        r.durumOnaylandiZemin,
        r.durumOnaylandiMetin,
        r.durumOnaylandiNokta,
      ),
      RandevuDurumu.geldi => (
        r.durumGeldiZemin,
        r.durumGeldiMetin,
        r.durumGeldiNokta,
      ),
      RandevuDurumu.tamamlandi => (
        r.durumTamamlandiZemin,
        r.durumTamamlandiMetin,
        r.durumTamamlandiNokta,
      ),
      RandevuDurumu.gelmedi => (
        r.durumGelmediZemin,
        r.durumGelmediMetin,
        r.durumGelmediNokta,
      ),
      RandevuDurumu.iptal => (
        r.durumIptalZemin,
        r.durumIptalMetin,
        r.durumIptalNokta,
      ),
    };
    return DecoratedBox(
      decoration: BoxDecoration(
        color: zemin,
        borderRadius: BorderRadius.circular(OmvilaKose.koseTam),
      ),
      child: Padding(
        padding: const EdgeInsets.symmetric(
          horizontal: OmvilaBosluk.bosluk2 + 2,
          vertical: OmvilaBosluk.bosluk1,
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            ExcludeSemantics(
              child: DecoratedBox(
                decoration: BoxDecoration(color: nokta, shape: BoxShape.circle),
                child: const SizedBox.square(dimension: 6),
              ),
            ),
            const SizedBox(width: OmvilaBosluk.bosluk1 + 2),
            Flexible(
              child: Text(
                durum.etiket,
                style: OmvilaYazi.aciklama.copyWith(
                  color: metin,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
