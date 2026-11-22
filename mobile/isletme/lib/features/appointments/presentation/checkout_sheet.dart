import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';
import 'package:omvila_isletme/features/appointments/presentation/checkout.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

class _Row {
  _Row(this.method, int amountMinor)
    : amount = TextEditingController(
        text: amountMinor > 0 ? _input(amountMinor) : '',
      );

  CollectedPaymentMethod method;
  final TextEditingController amount;
}

String _input(int amountMinor) {
  final lira = amountMinor ~/ 100;
  final kurus = amountMinor % 100;
  return kurus == 0 ? '$lira' : '$lira,${kurus.toString().padLeft(2, '0')}';
}

/// "Tamamla ve ödeme al" (OdemeAl, dilim-05 KK5): kalemler, kapora (havale)
/// düşümü, alınacak tutar; yöntem Nakit / Kart (salonun POS'u) / Havale /
/// Paketten (müşterinin kullanılabilir paketi varsa); "İkinci ödeme ekle".
/// OMVILA ödeme almaz: kart formu yok, yalnız yöntem kaydedilir. Bir
/// gönderim bir Idempotency-Key; sunucu reddederse yenilenir. Tamamlanırsa
/// true döner.
class CheckoutSheet extends ConsumerStatefulWidget {
  /// Alt sayfa.
  const CheckoutSheet({required this.appointment, super.key});

  /// CheckedIn randevu.
  final AppointmentDetail appointment;

  @override
  ConsumerState<CheckoutSheet> createState() => _CheckoutSheetState();
}

class _CheckoutSheetState extends ConsumerState<CheckoutSheet> {
  late final List<_Row> _rows = [
    _Row(CollectedPaymentMethod.cash, widget.appointment.amountDue.amountMinor),
  ];
  bool _usePackage = false;
  bool _busy = false;
  String? _error;
  String _key = newIdempotencyKey();

  AppointmentDetail get _a => widget.appointment;

  @override
  void dispose() {
    for (final row in _rows) {
      row.amount.dispose();
    }

    super.dispose();
  }

  List<RedemptionMatch> _matches() {
    final session = ref.watch(authControllerProvider).value;
    if (!(session?.can('customer.view') ?? false)) {
      return const [];
    }

    final packages = ref
        .watch(customerPackagesProvider(_a.customer.brandCustomerId))
        .value;
    if (packages == null) {
      return const [];
    }

    final today = branchToday(ref.watch(clockProvider), _a.timeZone);
    return redemptionMatches(_a, packages, today);
  }

  void _resetRows(CollectedPaymentMethod method, int amountMinor) {
    for (final row in _rows) {
      row.amount.dispose();
    }

    _rows
      ..clear()
      ..add(_Row(method, amountMinor));
  }

  void _choose(CollectedPaymentMethod? method, List<RedemptionMatch> matches) {
    setState(() {
      _error = null;
      if (method == null) {
        _usePackage = true;
        _resetRows(CollectedPaymentMethod.cash, amountToCollect(_a, matches));
      } else if (_usePackage) {
        _usePackage = false;
        _resetRows(method, _a.amountDue.amountMinor);
      } else {
        _rows.first.method = method;
      }
    });
  }

  void _addRow(int due) {
    final entered = _rows.fold(
      0,
      (sum, r) => sum + (parseMoneyInput(r.amount.text) ?? 0),
    );
    setState(() {
      _error = null;
      _rows.add(_Row(CollectedPaymentMethod.card, due - entered));
    });
  }

  Future<void> _submit(int due, List<RedemptionMatch> redemptions) async {
    final currency = _a.total.currency;
    final payments = <CompletionPayment>[];
    if (due > 0) {
      final amounts = [for (final r in _rows) parseMoneyInput(r.amount.text)];
      if (amounts.any((m) => m == null || m <= 0)) {
        setState(() => _error = 'Her ödeme için tutarı girin, ör. 450.');
        return;
      }

      final total = amounts.fold(0, (sum, m) => sum + m!);
      if (total != due) {
        setState(
          () => _error =
              'Ödemelerin toplamı (${formatMoney(total)}) alınacak tutarla '
              '(${formatMoney(due)}) aynı olmalı.',
        );
        return;
      }

      for (var i = 0; i < _rows.length; i++) {
        payments.add(
          CompletionPayment(
            method: _rows[i].method,
            amount: Money(amountMinor: amounts[i]!, currency: currency),
          ),
        );
      }
    }

    setState(() {
      _busy = true;
      _error = null;
    });
    final navigator = Navigator.of(context);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref
          .read(appointmentActionsProvider)
          .complete(
            _a,
            key: _key,
            request: ManageCompleteRequest(
              payments: payments,
              packageRedemptions: redemptions.isEmpty
                  ? null
                  : [for (final r in redemptions) r.request],
            ),
          );
      messenger.showSnackBar(
        SnackBar(
          content: Text(
            due > 0
                ? 'Randevu tamamlandı, ödeme kasaya işlendi.'
                : 'Randevu tamamlandı.',
          ),
        ),
      );
      navigator.pop(true);
    } on ApiProblem catch (e) {
      // Düzeltilmiş gövde yeni anahtarla gider.
      if (e.status != 0) {
        _key = newIdempotencyKey();
      }

      if (e.code == 'payment_total_mismatch') {
        final expected = e.meta['expectedAmountMinor'];
        setState(
          () => _error = expected is int
              ? 'Alınacak tutar ${formatMoney(expected)} olmalı. Tutarları '
                    'kontrol edin.'
              : errorMessage(e.code),
        );
      } else if (e.code == 'package_session_unavailable') {
        setState(() {
          _error = errorMessage(e.code);
          _usePackage = false;
          _resetRows(CollectedPaymentMethod.cash, _a.amountDue.amountMinor);
        });
        ref.invalidate(customerPackagesProvider(_a.customer.brandCustomerId));
      } else {
        messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
        if (e.status == 412 || e.code == 'invalid_state_transition') {
          navigator.pop(true);
        }
      }
    } finally {
      if (mounted) {
        setState(() => _busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final tz = _a.timeZone;
    final matches = _matches();
    final redemptions = _usePackage ? matches : const <RedemptionMatch>[];
    final due = amountToCollect(_a, redemptions);
    final deposit = depositHeld(_a);
    final status = RandevuDurumu.fromDisplayStatus(_a.displayStatus);
    final error = _error;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Semantics(
          header: true,
          child: Text(
            'Tamamla ve ödeme al',
            style: OmvilaYazi.baslik1.copyWith(
              color: r.metin,
              fontWeight: FontWeight.w800,
            ),
          ),
        ),
        Text(
          [
            _a.customer.fullName,
            timeRange(_a.startUtc, _a.endUtc, tz),
            ?status?.etiket,
          ].join(' · '),
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        LabelValueRows(
          rows: [
            for (final item in activeItems(_a))
              LabelValue(
                [
                  item.serviceName,
                  if (item.staffName != null) firstNameOf(item.staffName),
                ].join(' · '),
                (item.paidFromPackage ?? false)
                    ? 'Paketten'
                    : formatMoney(itemAmount(item)),
              ),
            if (deposit > 0)
              LabelValue(
                'Kapora (havale)',
                '−${formatMoney(deposit)}',
                valueColor: r.basari,
              ),
            for (final m in redemptions)
              LabelValue(
                'Paketten (${m.packageName})',
                '−${formatMoney(m.amountMinor)}',
                valueColor: r.basari,
              ),
            LabelValue('Alınacak', formatMoney(due), strong: true),
          ],
        ),
        if (due > 0 || matches.isNotEmpty) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          const SectionLabel('Ödeme yöntemi'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Wrap(
            spacing: OmvilaBosluk.bosluk2,
            runSpacing: OmvilaBosluk.bosluk2,
            children: [
              for (final m in collectedMethods)
                SelectChip(
                  label: methodLabel(m),
                  selected: !_usePackage && _rows.first.method == m,
                  onTap: _busy ? null : () => _choose(m, matches),
                ),
              if (matches.isNotEmpty)
                SelectChip(
                  label: 'Paketten',
                  icon: LucideIcons.package,
                  selected: _usePackage,
                  onTap: _busy ? null : () => _choose(null, matches),
                ),
            ],
          ),
        ],
        if (due > 0 && (_rows.length > 1 || _usePackage)) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          for (var i = 0; i < _rows.length; i++)
            _PaymentRowField(
              index: i,
              row: _rows[i],
              // Paket seçiliyken kalan tutarın yöntemi bu satırda seçilir.
              showMethods: i > 0 || _usePackage,
              onMethod: (m) => setState(() => _rows[i].method = m),
              onRemove: i == 0
                  ? null
                  : () => setState(() => _rows.removeAt(i).amount.dispose()),
            ),
        ],
        if (due > 0 && _rows.length < maxPaymentRows)
          Align(
            alignment: Alignment.centerLeft,
            child: TextButton.icon(
              onPressed: _busy ? null : () => _addRow(due),
              icon: const Icon(LucideIcons.plus, size: 20),
              label: const Text('İkinci ödeme ekle'),
              style: TextButton.styleFrom(
                foregroundColor: r.metinMarka,
                textStyle: OmvilaYazi.govdeGuclu,
                padding: EdgeInsets.zero,
              ),
            ),
          ),
        if (error != null) ...[
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Semantics(
            liveRegion: true,
            child: Text(
              error,
              style: OmvilaYazi.ikincil.copyWith(color: r.hata),
            ),
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk3),
        Text(
          'Tamamlanınca personelin primi hak edilir. Uygulama ödeme almaz, '
          'yalnız salonda alınan ödemeyi kaydeder.',
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        OmvilaButon(
          etiket: due > 0 ? '${formatMoney(due)} al ve tamamla' : 'Tamamla',
          tamGenislik: true,
          boyut: OmvilaButonBoyutu.lg,
          yukleniyor: _busy,
          yukleniyorEtiketi: 'Kaydediliyor…',
          onPressed: _busy ? null : () => _submit(due, redemptions),
        ),
      ],
    );
  }
}

class _PaymentRowField extends StatelessWidget {
  const _PaymentRowField({
    required this.index,
    required this.row,
    required this.showMethods,
    required this.onMethod,
    required this.onRemove,
  });

  final int index;
  final _Row row;
  final bool showMethods;
  final ValueChanged<CollectedPaymentMethod> onMethod;
  final VoidCallback? onRemove;

  @override
  Widget build(BuildContext context) {
    final remove = onRemove;
    return Padding(
      padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk3),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (showMethods) ...[
            Wrap(
              spacing: OmvilaBosluk.bosluk2,
              runSpacing: OmvilaBosluk.bosluk2,
              children: [
                for (final m in collectedMethods)
                  SelectChip(
                    label: methodLabel(m),
                    selected: row.method == m,
                    onTap: () => onMethod(m),
                  ),
              ],
            ),
            const SizedBox(height: OmvilaBosluk.bosluk2),
          ],
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: row.amount,
                  keyboardType: const TextInputType.numberWithOptions(
                    decimal: true,
                  ),
                  decoration: InputDecoration(
                    labelText:
                        '${index + 1}. ödeme · ${methodLabel(row.method)}',
                    suffixText: '₺',
                  ),
                ),
              ),
              if (remove != null)
                IconButton(
                  onPressed: remove,
                  tooltip: 'Ödemeyi kaldır',
                  icon: const Icon(LucideIcons.x, size: 20),
                ),
            ],
          ),
        ],
      ),
    );
  }
}
