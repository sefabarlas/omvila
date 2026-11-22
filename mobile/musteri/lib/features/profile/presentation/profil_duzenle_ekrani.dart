import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/presentation/home_provider.dart';
import 'package:omvila_musteri/features/profile/presentation/profile_providers.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

final _email = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$');

/// Profil bilgilerim (ProfilDuzenle): ad soyad, isteğe bağlı e-posta,
/// telefon salt okunur ("Değiştirmek için destek"). Dil seçimi yok (v1'de
/// yalnız Türkçe, T-101). Kayıt If-Match ile.
class ProfilDuzenleEkrani extends ConsumerWidget {
  /// Ekran.
  const ProfilDuzenleEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final profile = ref.watch(profileProvider);
    return switch (profile) {
      AsyncData(:final value) => _Form(
        key: ValueKey(value.version),
        profil: value,
      ),
      AsyncError(:final error) => IcSayfa(
        baslik: 'Profil bilgilerim',
        children: [
          HataDurumu(
            error: error,
            onRetry: () => ref.invalidate(profileProvider),
          ),
        ],
      ),
      _ => const IcSayfa(
        baslik: 'Profil bilgilerim',
        children: [
          Padding(
            padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
            child: YukleniyorDurumu(),
          ),
        ],
      ),
    };
  }
}

class _Form extends ConsumerStatefulWidget {
  const _Form({required this.profil, super.key});

  final MeProfile profil;

  @override
  ConsumerState<_Form> createState() => _FormState();
}

class _FormState extends ConsumerState<_Form> {
  late final _name = TextEditingController(text: widget.profil.fullName);
  late final _mail = TextEditingController(text: widget.profil.email);
  String? _nameError;
  String? _mailError;
  ApiProblem? _error;
  bool _saving = false;

  @override
  void dispose() {
    _name.dispose();
    _mail.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    final name = _name.text.trim();
    final mail = _mail.text.trim();
    setState(() {
      _error = null;
      _nameError = name.length < 2 ? 'Adınızı ve soyadınızı yazın.' : null;
      // Boş alan e-postayı siler: istemci `email: null` gönderir (T-113).
      _mailError = mail.isEmpty || _email.hasMatch(mail)
          ? null
          : 'Geçerli bir e-posta yazın.';
    });
    if (_nameError != null || _mailError != null) {
      return;
    }

    setState(() => _saving = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref
          .read(profileRepositoryProvider)
          .update(
            widget.profil,
            fullName: name,
            email: mail.isEmpty ? null : mail,
          );
      ref
        ..invalidate(profileProvider)
        ..invalidate(homeProvider);
      messenger.showSnackBar(
        const SnackBar(content: Text('Bilgileriniz kaydedildi.')),
      );
      if (!mounted) {
        return;
      }

      if (context.canPop()) {
        context.pop();
      } else {
        setState(() => _saving = false);
      }
    } on ApiProblem catch (e) {
      if (mounted) {
        setState(() {
          _saving = false;
          _error = e;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final error = _error;
    return IcSayfa(
      baslik: 'Profil bilgilerim',
      alt: OmvilaButon(
        etiket: 'Kaydet',
        yukleniyorEtiketi: 'Kaydediliyor…',
        boyut: OmvilaButonBoyutu.lg,
        tamGenislik: true,
        yukleniyor: _saving,
        onPressed: _save,
      ),
      children: [
        _Etiket('Ad soyad', renkler: r),
        TextField(
          controller: _name,
          textCapitalization: TextCapitalization.words,
          autofillHints: const [AutofillHints.name],
          decoration: InputDecoration(errorText: _nameError),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        _Etiket('E-posta (isteğe bağlı)', renkler: r),
        TextField(
          controller: _mail,
          keyboardType: TextInputType.emailAddress,
          autofillHints: const [AutofillHints.email],
          decoration: InputDecoration(
            errorText: _mailError,
            helperText: 'Makbuz ve hatırlatma için',
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        _Etiket('Telefon', renkler: r),
        Container(
          padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
          decoration: BoxDecoration(
            color: r.zeminIkincil,
            borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
          ),
          child: Row(
            children: [
              Expanded(
                child: Text(
                  formatPhone(widget.profil.phone),
                  style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                ),
              ),
              Text(
                'Değiştirmek için destek',
                style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
              ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Text(
          'Telefon numaranız giriş anahtarınızdır, buradan değiştirilemez.',
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          DurumKutusu(
            ikon: LucideIcons.alertCircle,
            baslik: 'Kaydedilemedi.',
            aciklama: error.message,
          ),
        ],
      ],
    );
  }
}

class _Etiket extends StatelessWidget {
  const _Etiket(this.metin, {required this.renkler});

  final String metin;
  final OmvilaRenkler renkler;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk2),
    child: Text(
      metin,
      style: OmvilaYazi.etiket.copyWith(color: renkler.metin),
    ),
  );
}
