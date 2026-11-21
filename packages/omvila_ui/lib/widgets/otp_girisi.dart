import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:omvila_ui/theme/omvila_theme.dart';

/// SMS doğrulama kodu girişi (components/OtpInput): tek gizli metin alanı ve
/// hane kutuları. iOS'ta `AutofillHints.oneTimeCode` ile otomatik doldurulur;
/// kod tamamlanınca [onTamamlandi] çağrılır. Hata metni ve yeniden gönderme
/// ekranındır; [hatali] yalnız kutuları hata rengine boyar.
class OtpGirisi extends StatefulWidget {
  /// Kod girişi.
  const OtpGirisi({
    this.uzunluk = 6,
    this.onTamamlandi,
    this.onDegisti,
    this.hatali = false,
    this.controller,
    this.focusNode,
    this.autofocus = false,
    this.semanticsLabel = 'Doğrulama kodu',
    super.key,
  });

  /// Hane sayısı.
  final int uzunluk;

  /// Bütün haneler girildi.
  final ValueChanged<String>? onTamamlandi;

  /// Her değişiklikte.
  final ValueChanged<String>? onDegisti;

  /// Kod hatalı: kutular `hata` rengiyle çizilir.
  final bool hatali;

  /// Dışarıdan değer okuma/temizleme için.
  final TextEditingController? controller;

  /// Odak (ör. hata sonrası yeniden odaklama).
  final FocusNode? focusNode;

  /// Ekran açılınca klavye açılsın.
  final bool autofocus;

  /// Ekran okuyucu etiketi.
  final String semanticsLabel;

  @override
  State<OtpGirisi> createState() => _OtpGirisiState();
}

class _OtpGirisiState extends State<OtpGirisi> {
  TextEditingController? _ownController;
  FocusNode? _ownFocusNode;

  TextEditingController get _controller =>
      widget.controller ?? (_ownController ??= TextEditingController());

  FocusNode get _focusNode =>
      widget.focusNode ?? (_ownFocusNode ??= FocusNode());

  @override
  void initState() {
    super.initState();
    _controller.addListener(_rebuild);
    _focusNode.addListener(_rebuild);
  }

  @override
  void didUpdateWidget(OtpGirisi oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.controller != widget.controller) {
      (oldWidget.controller ?? _ownController)?.removeListener(_rebuild);
      _controller.addListener(_rebuild);
    }

    if (oldWidget.focusNode != widget.focusNode) {
      (oldWidget.focusNode ?? _ownFocusNode)?.removeListener(_rebuild);
      _focusNode.addListener(_rebuild);
    }
  }

  @override
  void dispose() {
    _controller.removeListener(_rebuild);
    _focusNode.removeListener(_rebuild);
    _ownController?.dispose();
    _ownFocusNode?.dispose();
    super.dispose();
  }

  void _rebuild() => setState(() {});

  void _onChanged(String value) {
    widget.onDegisti?.call(value);
    if (value.length == widget.uzunluk) {
      widget.onTamamlandi?.call(value);
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final code = _controller.text;
    final focused = _focusNode.hasFocus;
    return Stack(
      children: [
        ExcludeSemantics(
          child: Row(
            children: [
              for (var i = 0; i < widget.uzunluk; i++) ...[
                if (i > 0) const SizedBox(width: OmvilaBosluk.bosluk2),
                Expanded(
                  child: _Hane(
                    rakam: i < code.length ? code[i] : '',
                    aktif:
                        focused &&
                        (i == code.length ||
                            (code.length == widget.uzunluk &&
                                i == widget.uzunluk - 1)),
                    hatali: widget.hatali,
                    renkler: r,
                  ),
                ),
              ],
            ],
          ),
        ),
        Positioned.fill(
          child: Semantics(
            label: widget.semanticsLabel,
            child: TextField(
              controller: _controller,
              focusNode: _focusNode,
              autofocus: widget.autofocus,
              keyboardType: TextInputType.number,
              textInputAction: TextInputAction.done,
              autofillHints: const [AutofillHints.oneTimeCode],
              inputFormatters: [
                FilteringTextInputFormatter.digitsOnly,
                LengthLimitingTextInputFormatter(widget.uzunluk),
              ],
              onChanged: _onChanged,
              showCursor: false,
              enableInteractiveSelection: false,
              style: const TextStyle(color: Colors.transparent, fontSize: 1),
              decoration: const InputDecoration(
                border: InputBorder.none,
                enabledBorder: InputBorder.none,
                focusedBorder: InputBorder.none,
                filled: false,
                counterText: '',
                contentPadding: EdgeInsets.zero,
              ),
            ),
          ),
        ),
      ],
    );
  }
}

class _Hane extends StatelessWidget {
  const _Hane({
    required this.rakam,
    required this.aktif,
    required this.hatali,
    required this.renkler,
  });

  final String rakam;
  final bool aktif;
  final bool hatali;
  final OmvilaRenkler renkler;

  @override
  Widget build(BuildContext context) {
    final r = renkler;
    final border = hatali ? r.hata : (aktif ? r.odak : r.cizgiGuclu);
    return Container(
      height: 56,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: r.zeminYukseltilmis,
        borderRadius: BorderRadius.circular(OmvilaKose.koseMd),
        border: Border.all(color: border, width: aktif || hatali ? 2 : 1),
      ),
      child: Text(rakam, style: OmvilaYazi.baslik2.copyWith(color: r.metin)),
    );
  }
}
