import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/new_appointment/data/booking_repository.dart';
import 'package:omvila_isletme/features/walk_ins/data/walk_in_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Sıraya ekle" alt sayfası (SiraEkle). Eklenen bileti döndürür.
Future<WalkInTicketView?> showWalkInAddSheet(
  BuildContext context, {
  required String branchId,
  WalkInQueue? queue,
}) => showOmvilaSheet<WalkInTicketView>(
  context,
  builder: (_) => WalkInAddForm(branchId: branchId, queue: queue),
);

/// SiraEkle: ad, telefon (isteğe bağlı), hizmet, personel. Numara
/// önizlenmez; sunucunun atomik verdiği numara eklendikten sonra yazılır.
class WalkInAddForm extends ConsumerStatefulWidget {
  /// Form.
  const WalkInAddForm({required this.branchId, this.queue, super.key});

  /// Şube.
  final String branchId;

  /// Güncel sıra ("Tahmini bekleme ~35 dk · önünde 3 kişi var.").
  final WalkInQueue? queue;

  @override
  ConsumerState<WalkInAddForm> createState() => _WalkInAddFormState();
}

class _WalkInAddFormState extends ConsumerState<WalkInAddForm> {
  final _name = TextEditingController();
  final _phone = TextEditingController();
  String? _serviceId;
  String? _staffId;
  bool _busy = false;
  String? _error;
  // Aynı kullanıcı eylemi için sabit (tekrar denemede çift bilet olmaz).
  final String _key = newIdempotencyKey();

  @override
  void dispose() {
    _name.dispose();
    _phone.dispose();
    super.dispose();
  }

  bool get _phoneValid =>
      _phone.text.trim().isEmpty || parseMobilePhone(_phone.text) != null;

  bool get _ready => _name.text.trim().length >= 2 && _phoneValid;

  Future<void> _submit() async {
    setState(() {
      _busy = true;
      _error = null;
    });
    final navigator = Navigator.of(context);
    try {
      final ticket = await ref
          .read(walkInActionsProvider)
          .create(
            WalkInCreate(
              branchId: widget.branchId,
              fullName: _name.text.trim(),
              phone: _phone.text.trim().isEmpty
                  ? null
                  : parseMobilePhone(_phone.text),
              serviceId: _serviceId,
              preferredStaffId: _staffId,
            ),
            key: _key,
          );
      navigator.pop(ticket);
    } on ApiProblem catch (e) {
      setState(() => _error = errorMessage(e.code));
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final queue = widget.queue;
    final wait = queue?.estimatedWaitMinutesForNew;
    final error = _error;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Semantics(
          header: true,
          child: Text(
            'Sıraya ekle',
            style: OmvilaYazi.baslik1.copyWith(color: r.metin),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        LabeledField(
          label: 'Ad',
          child: TextField(
            controller: _name,
            textCapitalization: TextCapitalization.words,
            decoration: const InputDecoration(
              helperText:
                  'Telefon isteğe bağlı; girilirse sıra gelince SMS gider',
              helperMaxLines: 2,
            ),
            onChanged: (_) => setState(() {}),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        LabeledField(
          label: 'Telefon (isteğe bağlı)',
          child: TextField(
            controller: _phone,
            keyboardType: TextInputType.phone,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'[\d\s]')),
            ],
            decoration: phoneDecoration(
              context,
              hintText: '532 123 45 67',
              errorText: _phoneValid ? null : 'Cep telefonu numarası girin',
            ),
            onChanged: (_) => setState(() {}),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        ServiceStaffChoice(
          branchId: widget.branchId,
          serviceId: _serviceId,
          staffId: _staffId,
          allowAnyStaff: true,
          onService: (id) => setState(() => _serviceId = id),
          onStaff: (id) => setState(() => _staffId = id),
        ),
        if (queue != null && wait != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          Text(
            'Tahmini bekleme ~$wait dk · önünde ${queue.waitingCount} kişi '
            'var.',
            style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
          ),
        ],
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          Text(error, style: OmvilaYazi.aciklama.copyWith(color: r.hata)),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk4),
        OmvilaButon(
          etiket: 'Sıraya ekle',
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

/// Hizmet ve personel çipleri (SiraEkle, "Başlat" eksik bilgi).
class ServiceStaffChoice extends ConsumerWidget {
  /// Seçim.
  const ServiceStaffChoice({
    required this.branchId,
    required this.serviceId,
    required this.staffId,
    required this.onService,
    required this.onStaff,
    required this.allowAnyStaff,
    this.showService = true,
    this.showStaff = true,
    super.key,
  });

  /// Şube.
  final String branchId;

  /// Seçili hizmet.
  final String? serviceId;

  /// Seçili personel (null = fark etmez).
  final String? staffId;

  /// Hizmet seçildi (aynısına dokununca kaldırılır).
  final ValueChanged<String?> onService;

  /// Personel seçildi.
  final ValueChanged<String?> onStaff;

  /// "Fark etmez".
  final bool allowAnyStaff;

  /// Hizmet bölümü.
  final bool showService;

  /// Personel bölümü.
  final bool showStaff;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final services = ref.watch(servicesProvider(branchId)).value ?? const [];
    final staff = ref.watch(staffProvider(branchId)).value ?? const [];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (showService) ...[
          const SectionLabel('Hizmet'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Wrap(
            spacing: OmvilaBosluk.bosluk2,
            runSpacing: OmvilaBosluk.bosluk2,
            children: [
              for (final s in services)
                SelectChip(
                  label: s.name,
                  selected: serviceId == s.id,
                  onTap: () => onService(serviceId == s.id ? null : s.id),
                ),
            ],
          ),
          const SizedBox(height: OmvilaBosluk.bosluk4),
        ],
        if (showStaff) ...[
          const SectionLabel('Personel'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Wrap(
            spacing: OmvilaBosluk.bosluk2,
            runSpacing: OmvilaBosluk.bosluk2,
            children: [
              if (allowAnyStaff)
                SelectChip(
                  label: 'Fark etmez',
                  selected: staffId == null,
                  onTap: () => onStaff(null),
                ),
              for (final s in staff)
                SelectChip(
                  label: s.displayName ?? s.fullName.split(' ').first,
                  selected: staffId == s.id,
                  onTap: () => onStaff(s.id),
                ),
            ],
          ),
        ],
      ],
    );
  }
}

/// "Başlat" için eksik bilgi.
typedef WalkInStartChoice = ({String serviceId, String staffId});

/// Bilette hizmet veya personel ("Fark etmez") yoksa sorar.
Future<WalkInStartChoice?> showWalkInStartSheet(
  BuildContext context, {
  required String branchId,
  required WalkInTicketView ticket,
  required bool askService,
  required bool askStaff,
}) => showOmvilaSheet<WalkInStartChoice>(
  context,
  builder: (_) => _StartForm(
    branchId: branchId,
    ticket: ticket,
    askService: askService,
    askStaff: askStaff,
  ),
);

class _StartForm extends StatefulWidget {
  const _StartForm({
    required this.branchId,
    required this.ticket,
    required this.askService,
    required this.askStaff,
  });

  final String branchId;
  final WalkInTicketView ticket;
  final bool askService;
  final bool askStaff;

  @override
  State<_StartForm> createState() => _StartFormState();
}

class _StartFormState extends State<_StartForm> {
  late String? _serviceId = widget.ticket.serviceId;
  late String? _staffId = widget.ticket.preferredStaffId;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final serviceId = _serviceId;
    final staffId = _staffId;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Semantics(
          header: true,
          child: Text(
            '${widget.ticket.customerName} için başlat',
            style: OmvilaYazi.baslik2.copyWith(color: r.metin),
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        ServiceStaffChoice(
          branchId: widget.branchId,
          serviceId: serviceId,
          staffId: staffId,
          allowAnyStaff: false,
          showService: widget.askService,
          showStaff: widget.askStaff,
          onService: (id) => setState(() => _serviceId = id),
          onStaff: (id) => setState(() => _staffId = id),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        OmvilaButon(
          etiket: 'Başlat',
          boyut: OmvilaButonBoyutu.lg,
          tamGenislik: true,
          onPressed: serviceId == null || staffId == null
              ? null
              : () => Navigator.of(
                  context,
                ).pop((serviceId: serviceId, staffId: staffId)),
        ),
      ],
    );
  }
}
