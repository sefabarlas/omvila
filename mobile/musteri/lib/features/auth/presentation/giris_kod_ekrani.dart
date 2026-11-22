import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/features/auth/data/sms_code_retriever.dart';
import 'package:omvila_musteri/features/auth/presentation/auth_controller.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// SMS kodu (GirisKod.png): 6 hane, yeniden gönderim geri sayımı, Android'de
/// SMS'ten otomatik okuma. Doğrulanınca oturum açılır; yönlendirme
/// router'dadır.
class GirisKodEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const GirisKodEkrani({required this.args, super.key});

  /// Telefon ve kod yanıtı.
  final GirisKodArgs args;

  @override
  ConsumerState<GirisKodEkrani> createState() => _GirisKodEkraniState();
}

class _GirisKodEkraniState extends ConsumerState<GirisKodEkrani> {
  static const _length = 6;

  final _code = TextEditingController();
  late OtpChallengeResponse _challenge = widget.args.challenge;
  late DateTime _resendAt;
  Timer? _ticker;
  StreamSubscription<String>? _smsCodes;
  late final SmsCodeRetriever _retriever = ref.read(smsCodeRetrieverProvider);
  bool _verifying = false;
  bool _resending = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _startCountdown();
    _code.addListener(() => setState(() {}));
    _smsCodes = _retriever.codes().listen((code) {
      if (code.length == _length && !_verifying) {
        _code.text = code;
        unawaited(_verify());
      }
    });
  }

  @override
  void dispose() {
    _ticker?.cancel();
    unawaited(_smsCodes?.cancel());
    unawaited(_retriever.stop());
    _code.dispose();
    super.dispose();
  }

  void _startCountdown() {
    final clock = ref.read(clockProvider);
    _resendAt = clock.now().add(
      Duration(seconds: _challenge.resendAfterSeconds),
    );
    _ticker?.cancel();
    _ticker = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (_remaining == Duration.zero) {
        timer.cancel();
      }

      setState(() {});
    });
  }

  Duration get _remaining {
    final left = _resendAt.difference(ref.read(clockProvider).now());
    return left.isNegative ? Duration.zero : left;
  }

  Future<void> _verify() async {
    if (_code.text.length != _length || _verifying) {
      return;
    }

    setState(() {
      _verifying = true;
      _error = null;
    });
    try {
      await ref
          .read(authRepositoryProvider)
          .verifyOtp(otpId: _challenge.otpId, code: _code.text);
      ref.read(authControllerProvider.notifier).signedIn();
    } on ApiProblem catch (e) {
      setState(() => _error = _message(e));
      _code.clear();
    } finally {
      if (mounted) {
        setState(() => _verifying = false);
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
          .read(authRepositoryProvider)
          .requestOtp(widget.args.phone);
      _code.clear();
      _startCountdown();
    } on ApiProblem catch (e) {
      setState(() => _error = _message(e));
    } finally {
      if (mounted) {
        setState(() => _resending = false);
      }
    }
  }

  static String _message(ApiProblem problem) {
    final left = problem.meta['attemptsLeft'];
    if (problem.code == ErrorCode.otpInvalid.value && left is int) {
      return 'Kod hatalı. $left deneme hakkınız kaldı.';
    }

    return problem.message;
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final remaining = _remaining;
    final error = _error;
    return Scaffold(
      backgroundColor: r.zemin,
      appBar: AppBar(
        backgroundColor: r.zemin,
        leading: IconButton(
          tooltip: 'Geri',
          icon: const Icon(LucideIcons.arrowLeft),
          onPressed: () => context.pop(),
        ),
      ),
      body: SafeArea(
        child: CustomScrollView(
          slivers: [
            SliverPadding(
              padding: const EdgeInsets.all(OmvilaBosluk.bosluk6),
              sliver: SliverFillRemaining(
                hasScrollBody: false,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
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
                      '${_challenge.phoneMasked} numarasına $_length haneli '
                      'kod gönderdik.',
                      style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk6),
                    OtpGirisi(
                      controller: _code,
                      autofocus: true,
                      hatali: error != null,
                      onTamamlandi: (_) => _verify(),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk3),
                    if (error != null) ...[
                      Semantics(
                        liveRegion: true,
                        child: Text(
                          error,
                          style: OmvilaYazi.ikincil.copyWith(color: r.hata),
                        ),
                      ),
                      const SizedBox(height: OmvilaBosluk.bosluk2),
                    ],
                    if (remaining > Duration.zero)
                      Text(
                        'Kod gelmedi mi? ${_countdown(remaining)} sonra tekrar '
                        'gönderebilirsiniz.',
                        style: OmvilaYazi.aciklama.copyWith(
                          color: r.metinIkincil,
                        ),
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
                    const SizedBox(height: OmvilaBosluk.bosluk6),
                    _BilgiKutusu(renkler: r),
                    const Spacer(),
                    const SizedBox(height: OmvilaBosluk.bosluk6),
                    OmvilaButon(
                      etiket: 'Devam et',
                      yukleniyorEtiketi: 'Doğrulanıyor…',
                      boyut: OmvilaButonBoyutu.lg,
                      tamGenislik: true,
                      yukleniyor: _verifying,
                      onPressed: _code.text.length == _length ? _verify : null,
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  static String _countdown(Duration d) =>
      '${d.inMinutes}:${(d.inSeconds % 60).toString().padLeft(2, '0')}';
}

class _BilgiKutusu extends StatelessWidget {
  const _BilgiKutusu({required this.renkler});

  final OmvilaRenkler renkler;

  @override
  Widget build(BuildContext context) {
    final r = renkler;
    return DecoratedBox(
      decoration: BoxDecoration(
        color: r.zeminIkincil,
        borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
      ),
      child: Padding(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(LucideIcons.calendarCheck, size: 20, color: r.metinMarka),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Text(
                'Bu numarayla aldığınız randevular girişten sonra burada '
                'görünür.',
                style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
