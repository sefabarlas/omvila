import 'package:flutter/gestures.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/app/router.dart';
import 'package:omvila_musteri/shared/widgets/omvila_logo.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Giriş: telefon ve "Kod gönder" (Giris.png). Şifre ve üyelik formu yok.
class GirisEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const GirisEkrani({super.key});

  @override
  ConsumerState<GirisEkrani> createState() => _GirisEkraniState();
}

class _GirisEkraniState extends ConsumerState<GirisEkrani> {
  final _phone = TextEditingController();
  late final TapGestureRecognizer _privacyTap = TapGestureRecognizer()
    ..onTap = _openPrivacy;
  bool _sending = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _phone.addListener(() => setState(() => _error = null));
  }

  @override
  void dispose() {
    _phone.dispose();
    _privacyTap.dispose();
    super.dispose();
  }

  String? get _e164 => parseMobilePhone(_phone.text);

  Future<void> _send() async {
    final phone = _e164;
    if (phone == null) {
      return;
    }

    setState(() => _sending = true);
    try {
      final challenge = await ref
          .read(authRepositoryProvider)
          .requestOtp(phone);
      if (!mounted) {
        return;
      }

      final from = GoRouterState.of(context)
          .uri
          .queryParameters[Routes.fromParam];
      await context.push(
        Uri(
          path: Routes.girisKod,
          queryParameters: from == null ? null : {Routes.fromParam: from},
        ).toString(),
        extra: GirisKodArgs(phone: phone, challenge: challenge),
      );
    } on ApiProblem catch (e) {
      setState(() => _error = e.message);
    } finally {
      if (mounted) {
        setState(() => _sending = false);
      }
    }
  }

  void _openPrivacy() {
    final url = ref.read(configProvider).privacyNoticeUrl;
    if (url != null) {
      ref.read(externalActionsProvider).openUrl(Uri.parse(url));
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final hasPrivacyLink = ref.watch(configProvider).privacyNoticeUrl != null;
    final error = _error;
    return Scaffold(
      backgroundColor: r.zemin,
      body: CustomScrollView(
        slivers: [
          SliverToBoxAdapter(child: _Kahraman(renkler: r)),
          SliverPadding(
            padding: const EdgeInsets.all(OmvilaBosluk.bosluk6),
            sliver: SliverFillRemaining(
              hasScrollBody: false,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Text(
                    'Randevu aldığınız telefonla girin. Şifre yok, üyelik '
                    'formu yok.',
                    style: OmvilaYazi.govde.copyWith(color: r.metinIkincil),
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk6),
                  Text(
                    'Cep telefonu',
                    style: OmvilaYazi.etiketKucuk.copyWith(color: r.metin),
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk2),
                  TextField(
                    controller: _phone,
                    keyboardType: TextInputType.phone,
                    autofillHints: const [AutofillHints.telephoneNumber],
                    inputFormatters: [PhoneInputFormatter()],
                    style: OmvilaYazi.govde.copyWith(color: r.metin),
                    onSubmitted: (_) => _send(),
                    decoration: InputDecoration(
                      // Önek alan boşken de görünür (components/PhoneField).
                      prefixIcon: Padding(
                        padding: const EdgeInsets.only(
                          left: OmvilaBosluk.bosluk4,
                          right: OmvilaBosluk.bosluk2,
                        ),
                        child: Text(
                          '+90',
                          style: OmvilaYazi.govdeGuclu.copyWith(
                            color: r.metin,
                          ),
                        ),
                      ),
                      prefixIconConstraints: const BoxConstraints(),
                      hintText: '5XX XXX XX XX',
                      errorText: error,
                      errorMaxLines: 3,
                    ),
                  ),
                  const Spacer(),
                  const SizedBox(height: OmvilaBosluk.bosluk6),
                  OmvilaButon(
                    etiket: 'Kod gönder',
                    yukleniyorEtiketi: 'Gönderiliyor…',
                    boyut: OmvilaButonBoyutu.lg,
                    tamGenislik: true,
                    yukleniyor: _sending,
                    onPressed: _e164 == null ? null : _send,
                  ),
                  const SizedBox(height: OmvilaBosluk.bosluk4),
                  Text.rich(
                    TextSpan(
                      children: [
                        const TextSpan(text: 'Devam ederek '),
                        TextSpan(
                          text: 'aydınlatma metnini',
                          recognizer: hasPrivacyLink ? _privacyTap : null,
                          style: hasPrivacyLink
                              ? TextStyle(
                                  color: r.metinMarka,
                                  fontWeight: FontWeight.w700,
                                  decoration: TextDecoration.underline,
                                )
                              : null,
                        ),
                        const TextSpan(text: ' okuduğunuzu onaylarsınız.'),
                      ],
                    ),
                    textAlign: TextAlign.center,
                    style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _Kahraman extends StatelessWidget {
  const _Kahraman({required this.renkler});

  final OmvilaRenkler renkler;

  @override
  Widget build(BuildContext context) {
    final r = renkler;
    return ClipRRect(
      borderRadius: const BorderRadius.vertical(
        bottom: Radius.circular(OmvilaKose.koseXl * 1.6),
      ),
      child: ColoredBox(
        color: r.blokKum,
        child: Stack(
          children: [
            Positioned(
              right: -50,
              top: 110,
              child: DekorKare(renk: r.blokMarka, boyut: 210),
            ),
            Positioned(
              right: 120,
              top: 270,
              child: DekorKare(renk: r.vurgu, boyut: 110, aci: -0.12),
            ),
            Padding(
              padding: EdgeInsets.fromLTRB(
                OmvilaBosluk.bosluk6,
                MediaQuery.paddingOf(context).top + OmvilaBosluk.bosluk12,
                OmvilaBosluk.bosluk6,
                OmvilaBosluk.bosluk12,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OmvilaLogo(renk: r.metinMarka),
                  const SizedBox(height: 140),
                  Semantics(
                    header: true,
                    child: Text(
                      'Randevularım\nbir arada.',
                      style: OmvilaYazi.buyukBaslik.copyWith(
                        color: r.blokKumMetin,
                        fontSize: 40,
                        height: 1.05,
                      ),
                    ),
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

/// Telefon alanı: yalnız rakam, baştaki 0 atılır, "532 611 24 67" gruplanır
/// (components/PhoneField).
class PhoneInputFormatter extends TextInputFormatter {
  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) {
    var digits = newValue.text.replaceAll(RegExp(r'\D'), '');
    if (digits.startsWith('90') && digits.length > 10) {
      digits = digits.substring(2);
    }

    digits = digits.replaceFirst(RegExp('^0+'), '');
    if (digits.length > 10) {
      digits = digits.substring(0, 10);
    }

    final buffer = StringBuffer();
    for (var i = 0; i < digits.length; i++) {
      if (i == 3 || i == 6 || i == 8) {
        buffer.write(' ');
      }

      buffer.write(digits[i]);
    }

    final text = buffer.toString();
    return TextEditingValue(
      text: text,
      selection: TextSelection.collapsed(offset: text.length),
    );
  }
}
