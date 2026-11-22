import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Kod ekranına geçen bilgi: girilen numara (yeniden gönderim için) ve
/// sunucunun döndürdüğü kod kaydı.
@immutable
class OtpStep {
  /// Adım.
  const OtpStep({required this.phoneE164, required this.challenge});

  /// E.164 numara.
  final String phoneE164;

  /// `manageRequestOtp` yanıtı.
  final OtpChallengeResponse challenge;
}

/// SMS kodu (GirisKod). Yeniden gönderim süresi sunucunun
/// `resendAfterSeconds` değerinden geri sayılır (sabit süre yok).
class LoginCodeScreen extends ConsumerStatefulWidget {
  /// Kod ekranı.
  const LoginCodeScreen({required this.step, super.key});

  /// Giriş adımı.
  final OtpStep step;

  @override
  ConsumerState<LoginCodeScreen> createState() => _LoginCodeScreenState();
}

class _LoginCodeScreenState extends ConsumerState<LoginCodeScreen> {
  final _code = TextEditingController();
  late OtpChallengeResponse _challenge = widget.step.challenge;
  Timer? _timer;
  int _resendIn = 0;
  bool _busy = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _code.addListener(() => setState(() {}));
    _startCountdown();
  }

  @override
  void dispose() {
    _timer?.cancel();
    _code.dispose();
    super.dispose();
  }

  void _startCountdown() {
    _timer?.cancel();
    _resendIn = _challenge.resendAfterSeconds;
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      setState(() => _resendIn--);
      if (_resendIn <= 0) {
        timer.cancel();
      }
    });
  }

  Future<void> _verify() async {
    if (_code.text.length != 6 || _busy) {
      return;
    }

    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await ref
          .read(authControllerProvider.notifier)
          .verifyOtp(otpId: _challenge.otpId, code: _code.text);
      // Yönlendirme oturum değişince router'dan gelir.
    } on NoAccountException {
      _fail(
        'Bu numarayla kayıtlı bir işletme hesabı yok. İşletme sahibiniz '
        'sizi personel olarak eklemeli.',
      );
    } on ApiProblem catch (e) {
      _fail(errorMessage(e.code));
    }
  }

  void _fail(String message) {
    if (mounted) {
      setState(() {
        _busy = false;
        _error = message;
      });
    }
  }

  Future<void> _resend() async {
    setState(() => _error = null);
    try {
      final challenge = await ref
          .read(authControllerProvider.notifier)
          .requestOtp(widget.step.phoneE164);
      setState(() {
        _challenge = challenge;
        _code.clear();
      });
      _startCountdown();
    } on ApiProblem catch (e) {
      _fail(errorMessage(e.code));
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final error = _error;
    final minutes = _resendIn ~/ 60;
    final seconds = (_resendIn % 60).toString().padLeft(2, '0');
    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(LucideIcons.arrowLeft),
          tooltip: 'Geri',
          onPressed: () => context.pop(),
        ),
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(
            horizontal: OmvilaBosluk.bosluk6,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                child: ListView(
                  children: [
                    Semantics(
                      header: true,
                      child: Text(
                        'Kodu girin',
                        style: OmvilaYazi.baslik1.copyWith(
                          color: r.metin,
                          fontSize: 32,
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk2),
                    Text(
                      '${_challenge.phoneMasked} numarasına 6 haneli kod '
                      'gönderdik.',
                      style: OmvilaYazi.govde.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk6),
                    OtpGirisi(
                      controller: _code,
                      autofocus: true,
                      hatali: error != null,
                      onTamamlandi: (_) => _verify(),
                    ),
                    const SizedBox(height: OmvilaBosluk.bosluk3),
                    if (error != null)
                      Text(
                        error,
                        style: OmvilaYazi.ikincil.copyWith(color: r.hata),
                      ),
                    if (_resendIn > 0)
                      Text(
                        'Kod gelmedi mi? $minutes:$seconds sonra tekrar '
                        'gönderebilirsiniz.',
                        style: OmvilaYazi.aciklama.copyWith(
                          color: r.metinIkincil,
                        ),
                      )
                    else
                      Align(
                        alignment: Alignment.centerLeft,
                        child: TextButton(
                          onPressed: _resend,
                          child: const Text('Kodu tekrar gönder'),
                        ),
                      ),
                  ],
                ),
              ),
              OmvilaButon(
                etiket: 'Giriş yap',
                boyut: OmvilaButonBoyutu.lg,
                tamGenislik: true,
                yukleniyor: _busy,
                yukleniyorEtiketi: 'Giriş yapılıyor…',
                onPressed: _code.text.length == 6 && !_busy ? _verify : null,
              ),
              const SizedBox(height: OmvilaBosluk.bosluk3),
              Text(
                'Bu telefonda giriş açık kalır.',
                textAlign: TextAlign.center,
                style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
              ),
              const SizedBox(height: OmvilaBosluk.bosluk3),
            ],
          ),
        ),
      ),
    );
  }
}
