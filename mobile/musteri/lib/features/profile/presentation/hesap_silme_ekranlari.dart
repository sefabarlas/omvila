import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/auth/presentation/auth_controller.dart';
import 'package:omvila_musteri/features/profile/data/profile_repository.dart';
import 'package:omvila_musteri/features/profile/domain/hesap_silme_metinleri.dart';
import 'package:omvila_musteri/features/profile/presentation/profile_providers.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Hesabı sil (HesapSil): yaklaşan randevu uyarısı (meListAppointments
/// Upcoming), silinen ve salonlarda kalan veriler, "SMS koduyla onayla ve
/// sil" → meRequestAccountDeletionOtp → kod ekranı.
class HesapSilEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const HesapSilEkrani({super.key});

  @override
  ConsumerState<HesapSilEkrani> createState() => _HesapSilEkraniState();
}

class _HesapSilEkraniState extends ConsumerState<HesapSilEkrani> {
  bool _sending = false;
  ApiProblem? _error;

  Future<void> _requestCode() async {
    setState(() {
      _sending = true;
      _error = null;
    });
    try {
      final challenge = await ref
          .read(profileRepositoryProvider)
          .requestDeletionOtp();
      if (mounted) {
        unawaited(context.push(Routes.hesapSilKod, extra: challenge));
      }
    } on ApiProblem catch (e) {
      if (mounted) {
        setState(() => _error = e);
      }
    } finally {
      if (mounted) {
        setState(() => _sending = false);
      }
    }
  }

  void _close() => context.canPop() ? context.pop() : context.go(Routes.profil);

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final preview = ref.watch(deletionPreviewProvider);
    final error = _error;
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        centerTitle: true,
        title: Text(
          'Hesabı sil',
          style: OmvilaYazi.ustBarBasligi.copyWith(color: r.metin),
        ),
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: _close,
        ),
        actions: [
          IconButton(
            tooltip: 'Kapat',
            icon: const Icon(LucideIcons.x),
            onPressed: () => context.go(Routes.profil),
          ),
        ],
      ),
      body: switch (preview) {
        AsyncData(:final value) => ListView(
          padding: const EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk5,
            OmvilaBosluk.bosluk2,
            OmvilaBosluk.bosluk5,
            OmvilaBosluk.bosluk6,
          ),
          children: [
            Align(
              alignment: Alignment.centerLeft,
              child: Container(
                width: 56,
                height: 56,
                decoration: BoxDecoration(
                  color: r.hataZemin,
                  borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
                ),
                child: Icon(LucideIcons.trash2, color: r.hata),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            Semantics(
              header: true,
              child: Text(
                hesapSilBaslik,
                style: OmvilaYazi.baslik1.copyWith(color: r.metin),
              ),
            ),
            const SizedBox(height: OmvilaBosluk.bosluk4),
            if (value.upcoming.isNotEmpty) ...[
              _IptalUyarisi(onizleme: value),
              const SizedBox(height: OmvilaBosluk.bosluk5),
            ],
            const _Bolum('Silinenler'),
            for (final (i, (title, body)) in silinenler.indexed)
              _Madde(
                ikon: i == 0 ? LucideIcons.user : LucideIcons.bell,
                renk: r.hata,
                baslik: title,
                aciklama: body,
              ),
            const SizedBox(height: OmvilaBosluk.bosluk3),
            const _Bolum('Salonlarda kalanlar'),
            _Madde(
              ikon: LucideIcons.store,
              renk: r.metin,
              baslik: kalanKayitlar.$1,
              aciklama: kalanKayitlar.$2,
            ),
            if (value.usablePackages > 0)
              _Madde(
                ikon: LucideIcons.package,
                renk: r.metin,
                baslik: kalanPaketler.$1,
                aciklama: kalanPaketler.$2,
              ),
            if (error != null) ...[
              const SizedBox(height: OmvilaBosluk.bosluk4),
              if (isOtpLimit(error))
                OtpSiniriKutusu(problem: error)
              else
                DurumKutusu(
                  ikon: LucideIcons.alertCircle,
                  baslik: 'Kod gönderilemedi.',
                  aciklama: error.message,
                ),
            ],
          ],
        ),
        AsyncError(:final error) => HataDurumu(
          error: error,
          onRetry: () => ref.invalidate(deletionPreviewProvider),
        ),
        _ => const YukleniyorDurumu(),
      },
      bottomNavigationBar: preview.hasValue
          ? SafeArea(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(
                  OmvilaBosluk.bosluk4,
                  OmvilaBosluk.bosluk2,
                  OmvilaBosluk.bosluk4,
                  OmvilaBosluk.bosluk2,
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    OmvilaButon(
                      etiket: hesapSilEylem,
                      yukleniyorEtiketi: 'Kod gönderiliyor…',
                      tur: OmvilaButonTuru.tehlike,
                      boyut: OmvilaButonBoyutu.lg,
                      tamGenislik: true,
                      yukleniyor: _sending,
                      onPressed: _requestCode,
                    ),
                    TextButton(onPressed: _close, child: const Text('Vazgeç')),
                  ],
                ),
              ),
            )
          : null,
    );
  }
}

class _IptalUyarisi extends StatelessWidget {
  const _IptalUyarisi({required this.onizleme});

  final DeletionPreview onizleme;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final list = onizleme.upcoming
        .map((a) {
          final day = formatDate(
            toBranchDate(a.startUtc, a.timeZone),
            DateStyle.short,
          ).split(' ').take(2).join(' ');
          return '${a.brand.name} · $day';
        })
        .join(', ');
    return Container(
      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
      decoration: BoxDecoration(
        color: r.uyariZemin,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(LucideIcons.calendarX, size: 20, color: r.uyari),
          const SizedBox(width: OmvilaBosluk.bosluk3),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  iptalEdilecekBaslik(onizleme.upcoming.length),
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
                Text(
                  '$list. $iptalEdilecekNot',
                  style: OmvilaYazi.ikincil.copyWith(color: r.metin),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _Bolum extends StatelessWidget {
  const _Bolum(this.metin);

  final String metin;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk3),
    child: Semantics(
      header: true,
      child: Text(
        toUpperTr(metin),
        style: OmvilaYazi.etiketKucuk.copyWith(
          color: context.omvilaRenkler.metinIkincil,
          letterSpacing: 0.8,
        ),
      ),
    ),
  );
}

class _Madde extends StatelessWidget {
  const _Madde({
    required this.ikon,
    required this.renk,
    required this.baslik,
    required this.aciklama,
  });

  final IconData ikon;
  final Color renk;
  final String baslik;
  final String aciklama;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Padding(
      padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk4),
      child: MergeSemantics(
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(ikon, size: 20, color: renk),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    baslik,
                    style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                  ),
                  Text(
                    aciklama,
                    style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

/// Hesap silme kodu: 6 hane, yeniden gönderim geri sayımı; kod doğrulanınca
/// hesap silinir (meRequestAccountDeletion), oturum cihazdan silinir ve
/// HesapSilindi açılır.
class HesapSilKodEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const HesapSilKodEkrani({required this.challenge, super.key});

  /// meRequestAccountDeletionOtp yanıtı.
  final OtpChallengeResponse challenge;

  @override
  ConsumerState<HesapSilKodEkrani> createState() => _HesapSilKodEkraniState();
}

class _HesapSilKodEkraniState extends ConsumerState<HesapSilKodEkrani> {
  static const _length = 6;

  final _code = TextEditingController();
  late OtpChallengeResponse _challenge = widget.challenge;
  late DateTime _resendAt;
  Timer? _ticker;
  bool _deleting = false;
  bool _resending = false;
  ApiProblem? _error;

  @override
  void initState() {
    super.initState();
    _code.addListener(() => setState(() {}));
    _startCountdown();
  }

  @override
  void dispose() {
    _ticker?.cancel();
    _code.dispose();
    super.dispose();
  }

  void _startCountdown() {
    _resendAt = ref
        .read(clockProvider)
        .now()
        .add(Duration(seconds: _challenge.resendAfterSeconds));
    _ticker?.cancel();
    _ticker = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (_remaining == Duration.zero) {
        timer.cancel();
      }

      if (mounted) {
        setState(() {});
      }
    });
  }

  Duration get _remaining {
    final left = _resendAt.difference(ref.read(clockProvider).now());
    return left.isNegative ? Duration.zero : left;
  }

  Future<void> _delete() async {
    if (_code.text.length != _length || _deleting) {
      return;
    }

    setState(() {
      _deleting = true;
      _error = null;
    });
    try {
      await ref
          .read(profileRepositoryProvider)
          .deleteAccount(otpId: _challenge.otpId, code: _code.text);
      // Sunucu bütün oturumları kapattı; jetonlar cihazdan silinir, giriş
      // ekranına HesapSilindi'deki "Kapat" ile geçilir.
      await ref.read(authRepositoryProvider).forget();
      if (mounted) {
        context.go(Routes.hesapSilindi);
      }
    } on ApiProblem catch (e) {
      if (mounted) {
        setState(() {
          _deleting = false;
          _error = e;
        });
        _code.clear();
      }
    }
  }

  Future<void> _resend() async {
    setState(() {
      _resending = true;
      _error = null;
    });
    try {
      _challenge = await ref
          .read(profileRepositoryProvider)
          .requestDeletionOtp();
      _code.clear();
      _startCountdown();
    } on ApiProblem catch (e) {
      setState(() => _error = e);
    } finally {
      if (mounted) {
        setState(() => _resending = false);
      }
    }
  }

  String _message(ApiProblem problem) {
    final left = problem.meta['attemptsLeft'];
    if (problem.code == ErrorCode.otpInvalid.value && left is int) {
      return 'Kod hatalı. $left deneme hakkınız kaldı.';
    }

    return problem.message;
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final error = _error;
    final remaining = _remaining;
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        surfaceTintColor: r.zemin,
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.pop(),
        ),
      ),
      body: ListView(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk5),
        children: [
          Semantics(
            header: true,
            child: Text(
              'Kodu girin',
              style: OmvilaYazi.buyukBaslik.copyWith(color: r.metin),
            ),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(
            hesapSilKodAciklama(_challenge.phoneMasked),
            style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk6),
          OtpGirisi(
            controller: _code,
            autofocus: true,
            hatali: error != null,
            onTamamlandi: (_) => _delete(),
          ),
          const SizedBox(height: OmvilaBosluk.bosluk3),
          if (error != null) ...[
            if (isOtpLimit(error))
              OtpSiniriKutusu(problem: error)
            else
              Semantics(
                liveRegion: true,
                child: Text(
                  _message(error),
                  style: OmvilaYazi.ikincil.copyWith(color: r.hata),
                ),
              ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
          ],
          if (remaining > Duration.zero)
            Text(
              'Kod gelmedi mi? ${remaining.inMinutes}:'
              '${(remaining.inSeconds % 60).toString().padLeft(2, '0')} '
              'sonra tekrar gönderebilirsiniz.',
              style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
            )
          else
            Align(
              alignment: Alignment.centerLeft,
              child: OmvilaButon(
                etiket: 'Kodu tekrar gönder',
                yukleniyorEtiketi: 'Gönderiliyor…',
                tur: OmvilaButonTuru.hayalet,
                boyut: OmvilaButonBoyutu.sm,
                yukleniyor: _resending,
                onPressed: _resend,
              ),
            ),
        ],
      ),
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: EdgeInsets.fromLTRB(
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk2,
            OmvilaBosluk.bosluk4,
            OmvilaBosluk.bosluk4 + MediaQuery.viewInsetsOf(context).bottom,
          ),
          child: OmvilaButon(
            etiket: 'Hesabımı sil',
            yukleniyorEtiketi: 'Siliniyor…',
            tur: OmvilaButonTuru.tehlike,
            boyut: OmvilaButonBoyutu.lg,
            tamGenislik: true,
            yukleniyor: _deleting,
            onPressed: _code.text.length == _length ? _delete : null,
          ),
        ),
      ),
    );
  }
}

/// Hesabınız silindi (HesapSilindi): "Kapat" oturumu kapatır, Giriş açılır.
class HesapSilindiEkrani extends ConsumerWidget {
  /// Ekran.
  const HesapSilindiEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    void close() => ref.read(authControllerProvider.notifier).signedOut();
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, _) {
        if (!didPop) {
          close();
        }
      },
      child: Scaffold(
        backgroundColor: r.zemin,
        body: SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
            child: Column(
              children: [
                Expanded(
                  child: Center(
                    child: SingleChildScrollView(
                      child: Column(
                        children: [
                          Container(
                            width: 80,
                            height: 80,
                            decoration: BoxDecoration(
                              color: r.zeminIkincil,
                              borderRadius: BorderRadius.circular(
                                OmvilaKose.koseXl,
                              ),
                            ),
                            child: Icon(
                              LucideIcons.check,
                              size: 32,
                              color: r.metin,
                            ),
                          ),
                          const SizedBox(height: OmvilaBosluk.bosluk5),
                          Semantics(
                            header: true,
                            child: Text(
                              hesapSilindiBaslik,
                              textAlign: TextAlign.center,
                              style: OmvilaYazi.baslik1.copyWith(
                                color: r.metin,
                              ),
                            ),
                          ),
                          const SizedBox(height: OmvilaBosluk.bosluk2),
                          Text(
                            hesapSilindiAciklama,
                            textAlign: TextAlign.center,
                            style: OmvilaYazi.govde.copyWith(
                              color: r.metinIkincil,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                OmvilaButon(
                  etiket: 'Kapat',
                  tur: OmvilaButonTuru.ikincil,
                  boyut: OmvilaButonBoyutu.lg,
                  tamGenislik: true,
                  onPressed: close,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
