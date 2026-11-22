import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/features/auth/presentation/login_code_screen.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';
import 'package:omvila_isletme/shared/brand_backdrop.dart';
import 'package:omvila_isletme/shared/full_height_scroll.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Giriş (Giris): yalnız telefon + SMS kodu (ürün kuralı 7). Hesap
/// uygulamada açılmaz; işletme sahibi personel olarak ekler.
class LoginScreen extends ConsumerStatefulWidget {
  /// Giriş.
  const LoginScreen({super.key});

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _phone = TextEditingController();
  bool _sending = false;
  String? _error;

  @override
  void dispose() {
    _phone.dispose();
    super.dispose();
  }

  Future<void> _send() async {
    final e164 = parseMobilePhone(_phone.text);
    if (e164 == null) {
      setState(() => _error = 'Geçerli bir cep telefonu numarası girin.');
      return;
    }

    setState(() {
      _sending = true;
      _error = null;
    });
    try {
      final challenge = await ref
          .read(authControllerProvider.notifier)
          .requestOtp(e164);
      if (mounted) {
        await context.push(
          Routes.loginCode,
          extra: OtpStep(phoneE164: e164, challenge: challenge),
        );
      }
    } on ApiProblem catch (e) {
      setState(() => _error = errorMessage(e.code));
    } finally {
      if (mounted) {
        setState(() => _sending = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final error = _error;
    return Scaffold(
      body: FullHeightScroll(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            BrandBackdrop(
              secondShape: false,
              borderRadius: const BorderRadius.vertical(
                bottom: Radius.circular(28),
              ),
              child: SafeArea(
                bottom: false,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(
                    OmvilaBosluk.bosluk6,
                    OmvilaBosluk.bosluk12,
                    OmvilaBosluk.bosluk6,
                    OmvilaBosluk.bosluk12,
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          const AppIcon(size: 32),
                          const SizedBox(width: OmvilaBosluk.bosluk3),
                          Text(
                            'OMVILA',
                            style: OmvilaYazi.govdeGuclu.copyWith(
                              color: r.metinTers,
                              letterSpacing: 2,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: OmvilaBosluk.bosluk6),
                      Text(
                        'İŞLETME',
                        style: OmvilaYazi.aciklama.copyWith(
                          color: r.metinTers.withValues(alpha: 0.8),
                          fontWeight: FontWeight.w700,
                          letterSpacing: 3,
                        ),
                      ),
                      const SizedBox(height: 120),
                      Semantics(
                        header: true,
                        child: Text(
                          'İşim\ncebimde.',
                          style: OmvilaYazi.buyukBaslik.copyWith(
                            color: r.metinTers,
                            fontSize: 40,
                            height: 1.1,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            Expanded(
              child: SafeArea(
                top: false,
                child: Padding(
                  padding: const EdgeInsets.all(OmvilaBosluk.bosluk6),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Bugünün randevuları, onaylar ve sıra tek ekranda. '
                        'İşletmenizde kayıtlı telefonla girin.',
                        style: OmvilaYazi.ikincil.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                      const SizedBox(height: OmvilaBosluk.bosluk6),
                      LabeledField(
                        label: 'Cep telefonu',
                        child: TextField(
                          controller: _phone,
                          keyboardType: TextInputType.phone,
                          autofillHints: const [
                            AutofillHints.telephoneNumber,
                          ],
                          inputFormatters: [
                            FilteringTextInputFormatter.allow(
                              RegExp(r'[\d ]'),
                            ),
                            LengthLimitingTextInputFormatter(14),
                          ],
                          onSubmitted: (_) => _send(),
                          decoration: phoneDecoration(
                            context,
                            hintText: '532 418 77 05',
                            errorText: error,
                          ),
                        ),
                      ),
                      const Spacer(),
                      const SizedBox(height: OmvilaBosluk.bosluk6),
                      OmvilaButon(
                        etiket: 'Kod gönder',
                        boyut: OmvilaButonBoyutu.lg,
                        tamGenislik: true,
                        yukleniyor: _sending,
                        yukleniyorEtiketi: 'Gönderiliyor…',
                        onPressed: _sending ? null : _send,
                      ),
                      const SizedBox(height: OmvilaBosluk.bosluk3),
                      Text(
                        'Hesabınız yok mu? İşletme sahibiniz sizi personel '
                        'olarak eklemeli.',
                        textAlign: TextAlign.center,
                        style: OmvilaYazi.aciklama.copyWith(
                          color: r.metinIkincil,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
