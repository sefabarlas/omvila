import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// "Kapora geldi mi?" (KaporaOnay, Dilim 4): tutar, açıklama kodu, müşteri
/// bildirimi, kalan süre; "Kapora geldi" (If-Match + Idempotency-Key) ve
/// "Henüz gelmedi, hatırlat" (randevu başına bir kez). Yalnız
/// `appointment.deposit.mark` izniyle açılır. Değişiklik olursa true döner.
class DepositSheet extends ConsumerStatefulWidget {
  /// Alt sayfa.
  const DepositSheet({required this.appointment, super.key});

  /// Kapora bekleyen randevu.
  final AppointmentDetail appointment;

  @override
  ConsumerState<DepositSheet> createState() => _DepositSheetState();
}

class _DepositSheetState extends ConsumerState<DepositSheet> {
  // Aynı "Kapora geldi" eylemi tekrar denenirken aynı anahtar gider.
  String _key = newIdempotencyKey();
  bool _marking = false;
  bool _reminding = false;
  bool _reminded = false;

  AppointmentDetail get _a => widget.appointment;

  Future<void> _mark() async {
    setState(() => _marking = true);
    final navigator = Navigator.of(context);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref
          .read(appointmentActionsProvider)
          .markDepositReceived(_a, key: _key);
      messenger.showSnackBar(
        const SnackBar(content: Text('Kapora işaretlendi, randevu onaylandı.')),
      );
      navigator.pop(true);
    } on ApiProblem catch (e) {
      if (e.status != 0) {
        _key = newIdempotencyKey();
      }

      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
      if (e.code == 'invalid_state_transition' || e.code == 'hold_expired') {
        navigator.pop(true);
      }
    } finally {
      if (mounted) {
        setState(() => _marking = false);
      }
    }
  }

  Future<void> _remind() async {
    setState(() => _reminding = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      await ref.read(appointmentActionsProvider).remindDeposit(_a);
      messenger.showSnackBar(
        const SnackBar(content: Text('Hatırlatma SMS’i gönderildi.')),
      );
      setState(() => _reminded = true);
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
      if (e.code == 'deposit_reminder_already_sent') {
        setState(() => _reminded = true);
      }
    } finally {
      if (mounted) {
        setState(() => _reminding = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final deposit = _a.deposit!;
    final tz = _a.timeZone;
    final now = ref.watch(clockProvider).now();
    final reported = deposit.customerReportedUtc;
    final hold = _a.holdExpiresUtc;
    final remaining = hold?.difference(now);
    final staff = {
      for (final i in activeItems(_a))
        if (i.staffName != null) i.staffName!,
    };
    final reminded = _reminded || deposit.remindedUtc != null;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    '${_a.customer.fullName} · '
                    '${formatTime(_a.startUtc, tz)}',
                    style: OmvilaYazi.baslik3.copyWith(
                      color: r.metin,
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                  Text(
                    [servicesOf(_a), ...staff].join(' · '),
                    style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                  ),
                ],
              ),
            ),
            const SizedBox(width: OmvilaBosluk.bosluk2),
            const OmvilaDurumRozeti(durum: RandevuDurumu.kaporaBekleniyor),
          ],
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        SurfaceCard(
          color: r.zeminIkincil,
          padding: const EdgeInsets.symmetric(
            horizontal: OmvilaBosluk.bosluk4,
            vertical: OmvilaBosluk.bosluk1,
          ),
          child: LabelValueRows(
            rows: [
              LabelValue('Tutar', money(deposit.required_)),
              if (deposit.transferCode != null)
                LabelValue('Açıklama kodu', deposit.transferCode!),
              LabelValue(
                'Müşteri bildirimi',
                reported == null
                    ? 'Henüz yok'
                    : '“Havaleyi yaptım” · ${formatTime(reported, tz)}',
              ),
              if (remaining != null)
                LabelValue(
                  'Kalan süre',
                  remaining.isNegative
                      ? 'Süre doldu'
                      : formatDuration((remaining.inSeconds / 60).ceil()),
                  valueColor: remaining.isNegative ? r.hata : null,
                ),
            ],
          ),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        Text(
          'Banka uygulamanızda açıklama kodunu arayın. İşaretleyince randevu '
          'kesinleşir ve müşteriye SMS gider.',
          style: OmvilaYazi.ikincil.copyWith(color: r.metin),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        OmvilaButon(
          etiket: 'Kapora geldi',
          ikon: LucideIcons.check,
          tamGenislik: true,
          yukleniyor: _marking,
          yukleniyorEtiketi: 'İşaretleniyor…',
          onPressed: _marking ? null : _mark,
        ),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        OmvilaButon(
          etiket: reminded
              ? 'Hatırlatma gönderildi'
              : 'Henüz gelmedi, hatırlat',
          ikon: LucideIcons.messageSquare,
          tur: OmvilaButonTuru.ikincil,
          tamGenislik: true,
          yukleniyor: _reminding,
          yukleniyorEtiketi: 'Gönderiliyor…',
          onPressed: reminded || _reminding ? null : _remind,
        ),
      ],
    );
  }
}
