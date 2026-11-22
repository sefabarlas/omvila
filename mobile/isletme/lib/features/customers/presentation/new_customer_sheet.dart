import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/customers/data/customer_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Müşteri ekle" / "Yeni müşteri olarak ekle": ad, telefon, not. Açılacak
/// kartın kimliğini döndürür (numara kayıtlıysa mevcut kart).
Future<String?> showNewCustomerSheet(BuildContext context, {String? initial}) =>
    showOmvilaSheet<String>(
      context,
      builder: (_) => NewCustomerForm(initial: initial),
    );

/// Yeni müşteri formu. Aramadaki metin numaraysa telefona, değilse ada
/// yazılır.
class NewCustomerForm extends ConsumerStatefulWidget {
  /// Form.
  const NewCustomerForm({this.initial, super.key});

  /// Aramadaki metin.
  final String? initial;

  @override
  ConsumerState<NewCustomerForm> createState() => _NewCustomerFormState();
}

class _NewCustomerFormState extends ConsumerState<NewCustomerForm> {
  late final TextEditingController _name;
  late final TextEditingController _phone;
  final _notes = TextEditingController();
  bool _busy = false;
  String? _error;
  String? _existingId;
  final String _key = newIdempotencyKey();

  @override
  void initState() {
    super.initState();
    final initial = widget.initial?.trim() ?? '';
    final isPhone = RegExp(r'^[\d\s+]+$').hasMatch(initial);
    _name = TextEditingController(text: isPhone ? '' : initial);
    _phone = TextEditingController(text: isPhone ? initial : '');
  }

  @override
  void dispose() {
    _name.dispose();
    _phone.dispose();
    _notes.dispose();
    super.dispose();
  }

  bool get _ready =>
      _name.text.trim().length >= 2 && parseMobilePhone(_phone.text) != null;

  Future<void> _submit() async {
    setState(() {
      _busy = true;
      _error = null;
      _existingId = null;
    });
    final navigator = Navigator.of(context);
    try {
      final notes = _notes.text.trim();
      final created = await ref.read(createCustomerProvider)(
        CustomerCreate(
          fullName: _name.text.trim(),
          phone: parseMobilePhone(_phone.text)!,
          notes: notes.isEmpty ? null : notes,
        ),
        _key,
      );
      navigator.pop(created.id);
    } on ApiProblem catch (e) {
      final existing = e.meta['brandCustomerId'];
      setState(() {
        _error = errorMessage(e.code);
        _existingId = e.code == 'phone_already_exists' && existing is String
            ? existing
            : null;
      });
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final error = _error;
    final existing = _existingId;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Semantics(
          header: true,
          child: Text(
            'Yeni müşteri',
            style: OmvilaYazi.baslik1.copyWith(color: r.metin),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        TextField(
          controller: _name,
          textCapitalization: TextCapitalization.words,
          decoration: const InputDecoration(labelText: 'Ad soyad'),
          onChanged: (_) => setState(() {}),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        TextField(
          controller: _phone,
          keyboardType: TextInputType.phone,
          inputFormatters: [
            FilteringTextInputFormatter.allow(RegExp(r'[\d\s+]')),
          ],
          decoration: const InputDecoration(
            labelText: 'Telefon',
            hintText: '0532 123 45 67',
          ),
          onChanged: (_) => setState(() {}),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        TextField(
          controller: _notes,
          maxLines: 3,
          maxLength: 2000,
          decoration: const InputDecoration(
            labelText: 'Not (isteğe bağlı)',
            helperText:
                'Notlar yalnızca salon ekibine görünür. Sağlık bilgisi '
                'yazmayın.',
            helperMaxLines: 2,
          ),
        ),
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Text(error, style: OmvilaYazi.aciklama.copyWith(color: r.hata)),
        ],
        if (existing != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          OmvilaButon(
            etiket: 'Mevcut kartı aç',
            tur: OmvilaButonTuru.ikincil,
            tamGenislik: true,
            onPressed: () => Navigator.of(context).pop(existing),
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk4),
        OmvilaButon(
          etiket: 'Müşteriyi ekle',
          boyut: OmvilaButonBoyutu.lg,
          tamGenislik: true,
          yukleniyor: _busy,
          yukleniyorEtiketi: 'Ekleniyor…',
          onPressed: _ready && !_busy ? _submit : null,
        ),
      ],
    );
  }
}
