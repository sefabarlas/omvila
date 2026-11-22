import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/features/appointments/domain/review_target.dart';
import 'package:omvila_musteri/features/appointments/presentation/gecmis_ekrani.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/appointments/presentation/randevu_detay_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/baglanti_gecersiz_ekrani.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Değerlendirme hedefi: (randevu kimliği, bağlantı kodu).
final FutureProviderFamily<ReviewTarget, (String?, String?)>
reviewTargetProvider = FutureProvider.autoDispose
    .family<ReviewTarget, (String?, String?)>(
      (ref, key) => ref
          .watch(appointmentsRepositoryProvider)
          .reviewTarget(appointmentId: key.$1, accessCode: key.$2),
    );

/// Puan adları (web/rezervasyon ReviewForm ile aynı).
const puanAdlari = ['Kötü', 'İdare eder', 'İyi', 'Çok iyi', 'Mükemmel'];

/// Değerlendir (Degerlendir): 5 yıldız, personel kartı, isteğe bağlı yorum.
/// Yalnız tamamlanan randevu ve bir kez (KK8); değerlendirilmişse gönderilen
/// puan gösterilir.
class DegerlendirEkrani extends ConsumerStatefulWidget {
  /// Ekran; [appointmentId] (uygulama içi) veya [accessCode] (`/d/{code}`).
  const DegerlendirEkrani({this.appointmentId, this.accessCode, super.key});

  /// Randevu kimliği.
  final String? appointmentId;

  /// Değerlendirme bağlantısının kodu.
  final String? accessCode;

  @override
  ConsumerState<DegerlendirEkrani> createState() => _DegerlendirEkraniState();
}

class _DegerlendirEkraniState extends ConsumerState<DegerlendirEkrani> {
  final _comment = TextEditingController();
  int _rating = 0;
  bool _sending = false;
  ApiProblem? _error;

  (String?, String?) get _key => (widget.appointmentId, widget.accessCode);

  @override
  void dispose() {
    _comment.dispose();
    super.dispose();
  }

  void _close() =>
      context.canPop() ? context.pop() : context.go(Routes.randevularim);

  Future<void> _send(ReviewTarget target) async {
    setState(() {
      _sending = true;
      _error = null;
    });
    final comment = _comment.text.trim();
    try {
      await ref
          .read(appointmentsRepositoryProvider)
          .submitReview(target, _rating, comment.isEmpty ? null : comment);
      if (!mounted) {
        return;
      }

      ref
        ..invalidate(pastProvider)
        ..invalidate(homeProvider);
      if (target.appointmentId case final id?) {
        tazele(ref, id);
      }

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Değerlendirmeniz gönderildi.')),
      );
      _close();
    } on ApiProblem catch (e) {
      if (!mounted) {
        return;
      }

      setState(() {
        _sending = false;
        _error = e;
      });
      if (e.code == 'already_reviewed') {
        ref.invalidate(reviewTargetProvider(_key));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final target = ref.watch(reviewTargetProvider(_key));
    final value = target.value;
    if (target case AsyncError(error: ApiProblem(status: 404))
        when widget.accessCode != null) {
      return const BaglantiGecersizEkrani();
    }

    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        automaticallyImplyLeading: false,
        leading: IconButton(
          tooltip: 'Kapat',
          icon: const Icon(LucideIcons.x),
          onPressed: _close,
        ),
      ),
      body: switch (target) {
        AsyncData(:final value) => _body(context, value),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(reviewTargetProvider(_key)),
        ),
        _ => const YukleniyorDurumu(),
      },
      bottomNavigationBar: value == null || value.alreadyReviewed
          ? null
          : SafeArea(
              top: false,
              child: Padding(
                padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
                child: OmvilaButon(
                  etiket: 'Gönder',
                  boyut: OmvilaButonBoyutu.lg,
                  tamGenislik: true,
                  yukleniyor: _sending,
                  onPressed: _rating == 0 ? null : () => _send(value),
                ),
              ),
            ),
    );
  }

  Widget _body(BuildContext context, ReviewTarget t) {
    final r = context.omvilaRenkler;
    final error = _error;
    final staff = t.staffName;
    final name = t.displayName;
    final existing = t.existingRating;
    final rating = existing ?? _rating;
    return ListView(
      padding: const EdgeInsets.fromLTRB(
        OmvilaBosluk.bosluk4,
        0,
        OmvilaBosluk.bosluk4,
        OmvilaBosluk.bosluk8,
      ),
      children: [
        Align(
          alignment: Alignment.centerLeft,
          child: SalonRozeti(ad: t.brandName, boyut: 64),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        Semantics(
          header: true,
          child: Text(
            '${bulunma(t.brandName)} nasıldı?',
            style: OmvilaYazi.baslik1.copyWith(color: r.metin),
          ),
        ),
        Text(
          ozetSatiri([
            t.serviceNames.join(', '),
            staff ?? '',
            formatDate(
              toBranchDate(t.startUtc, t.timeZone),
              DateStyle.dayMonth,
            ),
          ]),
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk5),
        DecoratedBox(
          decoration: BoxDecoration(
            color: r.zeminYukseltilmis,
            borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
            boxShadow: context.omvilaGolgeler.golgeSm,
          ),
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
            child: Column(
              children: [
                if (existing != null)
                  Yildizlar(puan: existing, boyut: 36)
                else
                  PuanSecici(
                    puan: _rating,
                    onSecildi: (v) => setState(() => _rating = v),
                  ),
                const SizedBox(height: OmvilaBosluk.bosluk2),
                Text(
                  rating == 0 ? 'Puan seçin' : puanAdlari[rating - 1],
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
              ],
            ),
          ),
        ),
        if (existing != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          OmvilaBosDurum(
            ikon: LucideIcons.badgeCheck,
            baslik: 'Bu randevuyu değerlendirdiniz',
            aciklama:
                t.existingComment ?? 'Değerlendirmeniz salonla paylaşıldı.',
          ),
        ] else ...[
          if (staff != null) ...[
            const SizedBox(height: OmvilaBosluk.bosluk3),
            DecoratedBox(
              decoration: BoxDecoration(
                color: r.zeminYukseltilmis,
                borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
              ),
              child: Padding(
                padding: const EdgeInsets.all(OmvilaBosluk.bosluk3),
                child: Row(
                  children: [
                    CircleAvatar(
                      radius: 16,
                      backgroundColor: r.avatar3Zemin,
                      child: Text(
                        initials(staff),
                        style: OmvilaYazi.aciklama.copyWith(
                          color: r.avatar3Metin,
                        ),
                      ),
                    ),
                    const SizedBox(width: OmvilaBosluk.bosluk3),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            staff,
                            style: OmvilaYazi.etiket.copyWith(color: r.metin),
                          ),
                          Text(
                            'Sizi bu randevuda ağırlayan kişi',
                            style: OmvilaYazi.aciklama.copyWith(
                              color: r.metinIkincil,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
          const SizedBox(height: OmvilaBosluk.bosluk4),
          Text(
            'Yorum (isteğe bağlı)',
            style: OmvilaYazi.etiket.copyWith(color: r.metin),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          TextField(
            controller: _comment,
            maxLength: 1000,
            maxLines: null,
            decoration: const InputDecoration(
              hintText: 'Deneyiminizi birkaç kelimeyle anlatın',
              counterText: '',
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            name == null
                ? 'Yorumunuz salonla paylaşılır.'
                : 'Yorumunuz salonla paylaşılır. Adınız "$name" olarak '
                      'görünür.',
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
          if (error != null) ...[
            const SizedBox(height: OmvilaBosluk.bosluk3),
            Text(
              error.message,
              style: OmvilaYazi.ikincil.copyWith(color: r.hata),
            ),
          ],
        ],
      ],
    );
  }
}

/// 1–5 yıldız puan (components/RatingInput): her yıldız 44×44 dokunma alanı.
class PuanSecici extends StatelessWidget {
  /// Seçici; [puan] 0 ise seçilmedi.
  const PuanSecici({required this.puan, required this.onSecildi, super.key});

  /// Seçili puan.
  final int puan;

  /// Seçildi.
  final ValueChanged<int> onSecildi;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        for (var i = 1; i <= 5; i++)
          Semantics(
            label: '$i yıldız, ${puanAdlari[i - 1]}',
            selected: i == puan,
            button: true,
            excludeSemantics: true,
            child: IconButton(
              iconSize: 36,
              constraints: const BoxConstraints(
                minWidth: OmvilaBoyut.dokunmaMin + 4,
                minHeight: OmvilaBoyut.dokunmaMin + 4,
              ),
              onPressed: () => onSecildi(i),
              icon: Icon(
                i <= puan ? Icons.star_rounded : Icons.star_outline_rounded,
                color: i <= puan ? r.yildiz : r.metinIkincil,
              ),
            ),
          ),
      ],
    );
  }
}
