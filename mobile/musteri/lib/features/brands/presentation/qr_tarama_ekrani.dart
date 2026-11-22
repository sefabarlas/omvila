import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/brands/domain/brand_link.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "QR kodu okut": salonun QR kartını okur, bağlantı adıyla geri döner.
/// Tasarımda ayrı ekran yok; tam ekran kamera, kapatma ve tek satır ipucu
/// (T-106).
class QrTaramaEkrani extends ConsumerStatefulWidget {
  /// Ekran.
  const QrTaramaEkrani({this.scanner, super.key});

  /// Kamera görünümü (testte yerine konur).
  final Widget Function(void Function(String value) onValue)? scanner;

  @override
  ConsumerState<QrTaramaEkrani> createState() => _QrTaramaEkraniState();
}

class _QrTaramaEkraniState extends ConsumerState<QrTaramaEkrani> {
  bool _invalid = false;
  bool _done = false;

  void _onValue(String value) {
    if (_done) {
      return;
    }

    final slug = brandSlugFromText(value, ref.read(configProvider).linkHosts);
    if (slug == null) {
      setState(() => _invalid = true);
      return;
    }

    _done = true;
    context.pop(slug);
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final scanner =
        widget.scanner?.call(_onValue) ??
        MobileScanner(
          onDetect: (capture) {
            final value = capture.barcodes.firstOrNull?.rawValue;
            if (value != null) {
              _onValue(value);
            }
          },
        );
    return Scaffold(
      backgroundColor: r.blokMarka,
      body: Stack(
        fit: StackFit.expand,
        children: [
          scanner,
          SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  IconButton.filledTonal(
                    tooltip: 'Kapat',
                    icon: const Icon(LucideIcons.x),
                    onPressed: () => context.pop(),
                  ),
                  const Spacer(),
                  DecoratedBox(
                    decoration: BoxDecoration(
                      color: r.zeminYukseltilmis,
                      borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
                    ),
                    child: Padding(
                      padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
                      child: Semantics(
                        liveRegion: true,
                        child: Text(
                          _invalid
                              ? 'Bu QR kod bir salon bağlantısı değil. '
                                    'Salondaki OMVILA QR kartını okutun.'
                              : 'Salondaki QR kartı çerçeveye getirin.',
                          style: OmvilaYazi.ikincil.copyWith(
                            color: _invalid ? r.hata : r.metin,
                          ),
                        ),
                      ),
                    ),
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
