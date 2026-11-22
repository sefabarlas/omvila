import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/appointments/data/appointment_repository.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';
import 'package:omvila_isletme/features/auth/data/auth_controller.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// RandevuIslem seçenekleri.
enum AppointmentAction {
  /// "Saati veya personeli değiştir".
  reschedule,

  /// "Gelmedi işaretini geri al".
  undoNoShow,

  /// "Randevuyu iptal et".
  cancel,
}

/// Oturumun bu randevuda yapabileceği RandevuIslem seçenekleri
/// (roller-yetkiler.md). "Hizmet ekle" ve "Müşteriye SMS gönder"
/// sözleşmede uç olmadığı için yok (docs/acik-sorular.md T-109).
List<AppointmentAction> availableActions(
  ManageSession session,
  AppointmentDetail a,
) {
  final open =
      a.status == AppointmentStatus.pendingApproval ||
      a.status == AppointmentStatus.confirmed;
  final cancellable =
      (open && session.can('appointment.cancel')) ||
      (a.status == AppointmentStatus.checkedIn &&
          session.can('appointment.cancel.checkedin'));
  return [
    if (open && session.can('appointment.update')) AppointmentAction.reschedule,
    if (a.status == AppointmentStatus.noShow &&
        session.can('appointment.checkin'))
      AppointmentAction.undoNoShow,
    if (cancellable) AppointmentAction.cancel,
  ];
}

/// "…" alt sayfası (RandevuIslem). Seçilen eylemi döner.
class ActionsSheet extends StatelessWidget {
  /// Alt sayfa.
  const ActionsSheet({
    required this.appointment,
    required this.actions,
    super.key,
  });

  /// Randevu.
  final AppointmentDetail appointment;

  /// Gösterilecek seçenekler.
  final List<AppointmentAction> actions;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final a = appointment;
    final status = RandevuDurumu.fromDisplayStatus(a.displayStatus);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          '${a.customer.fullName} · ${formatTime(a.startUtc, a.timeZone)}',
          style: OmvilaYazi.baslik3.copyWith(
            color: r.metin,
            fontWeight: FontWeight.w800,
          ),
        ),
        Text(
          [servicesOf(a), ?status?.etiket].join(' · '),
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        for (final action in actions)
          switch (action) {
            AppointmentAction.reschedule => _ActionTile(
              icon: LucideIcons.calendarClock,
              title: 'Saati veya personeli değiştir',
              subtitle: 'Uygun saatleri gösterir',
              onTap: () => Navigator.of(context).pop(action),
            ),
            AppointmentAction.undoNoShow => _ActionTile(
              icon: LucideIcons.undo2,
              title: 'Gelmedi işaretini geri al',
              subtitle: 'Randevu Geldi olur',
              onTap: () => Navigator.of(context).pop(action),
            ),
            AppointmentAction.cancel => _ActionTile(
              icon: LucideIcons.calendarX,
              title: 'Randevuyu iptal et',
              subtitle: 'Sebep sorulur, saat bekleme listesine açılır',
              danger: true,
              onTap: () => Navigator.of(context).pop(action),
            ),
          },
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Text(
          '“Gelmedi” otomatik işaretlenir; yanlışsa randevu kartından geri '
          'alabilirsiniz.',
          style: OmvilaYazi.aciklama.copyWith(color: r.metinIkincil),
        ),
      ],
    );
  }
}

class _ActionTile extends StatelessWidget {
  const _ActionTile({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.onTap,
    this.danger = false,
  });

  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;
  final bool danger;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Semantics(
      button: true,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: OmvilaBosluk.bosluk3),
          child: Row(
            children: [
              Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: danger ? r.hataZemin : r.zeminIkincil,
                  borderRadius: BorderRadius.circular(OmvilaKose.koseSm),
                ),
                child: Icon(icon, size: 20, color: danger ? r.hata : r.metin),
              ),
              const SizedBox(width: OmvilaBosluk.bosluk3),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: OmvilaYazi.govdeGuclu.copyWith(
                        color: danger ? r.hata : r.metin,
                      ),
                    ),
                    Text(
                      subtitle,
                      style: OmvilaYazi.ikincil.copyWith(
                        color: r.metinIkincil,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// "Randevuyu iptal et": neden, müşteriye bildir, (izin varsa) saati bekleme
/// listesine aç, tekrarlayan randevuda kapsam. İptal edilirse true döner.
class CancelSheet extends ConsumerStatefulWidget {
  /// Alt sayfa.
  const CancelSheet({required this.appointment, super.key});

  /// Randevu.
  final AppointmentDetail appointment;

  @override
  ConsumerState<CancelSheet> createState() => _CancelSheetState();
}

class _CancelSheetState extends ConsumerState<CancelSheet> {
  ManageCancelRequestReasonEnum _reason =
      ManageCancelRequestReasonEnum.customerRequested;
  ManageCancelRequestScopeEnum _scope = ManageCancelRequestScopeEnum.thisOnly;
  bool _notify = true;
  bool _offerToWaitlist = true;
  bool _busy = false;
  final _note = TextEditingController();

  @override
  void dispose() {
    _note.dispose();
    super.dispose();
  }

  Future<void> _cancel({required bool canOfferWaitlist}) async {
    setState(() => _busy = true);
    final navigator = Navigator.of(context);
    final messenger = ScaffoldMessenger.of(context);
    final note = _note.text.trim();
    try {
      await ref
          .read(appointmentActionsProvider)
          .cancel(
            widget.appointment,
            ManageCancelRequest(
              reason: _reason,
              note: note.isEmpty ? null : note,
              scope: widget.appointment.isRecurring ? _scope : null,
              notifyCustomer: _notify,
              offerToWaitlist: canOfferWaitlist ? _offerToWaitlist : null,
            ),
          );
      messenger.showSnackBar(
        const SnackBar(content: Text('Randevu iptal edildi.')),
      );
      navigator.pop(true);
    } on ApiProblem catch (e) {
      messenger.showSnackBar(SnackBar(content: Text(errorMessage(e.code))));
      if (e.status == 412 || e.code == 'invalid_state_transition') {
        navigator.pop(true);
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
    final session = ref.watch(authControllerProvider).value;
    final canOfferWaitlist = session?.can('waitlist.manage') ?? false;
    const reasons = {
      ManageCancelRequestReasonEnum.customerRequested: 'Müşteri istedi',
      ManageCancelRequestReasonEnum.staffUnavailable: 'Personel müsait değil',
      ManageCancelRequestReasonEnum.other: 'Diğer',
    };
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          'Randevuyu iptal et',
          style: OmvilaYazi.baslik2.copyWith(color: r.metin),
        ),
        const SizedBox(height: OmvilaBosluk.bosluk4),
        const SectionLabel('Sebep'),
        const SizedBox(height: OmvilaBosluk.bosluk2),
        Wrap(
          spacing: OmvilaBosluk.bosluk2,
          runSpacing: OmvilaBosluk.bosluk2,
          children: [
            for (final MapEntry(key: reason, value: label) in reasons.entries)
              SelectChip(
                label: label,
                selected: _reason == reason,
                onTap: () => setState(() => _reason = reason),
              ),
          ],
        ),
        if (_reason == ManageCancelRequestReasonEnum.other) ...[
          const SizedBox(height: OmvilaBosluk.bosluk3),
          TextField(
            controller: _note,
            maxLength: 300,
            decoration: const InputDecoration(labelText: 'Not (isteğe bağlı)'),
          ),
        ],
        if (widget.appointment.isRecurring) ...[
          const SizedBox(height: OmvilaBosluk.bosluk4),
          const SectionLabel('Tekrarlayan randevu'),
          const SizedBox(height: OmvilaBosluk.bosluk2),
          Wrap(
            spacing: OmvilaBosluk.bosluk2,
            runSpacing: OmvilaBosluk.bosluk2,
            children: [
              SelectChip(
                label: 'Yalnız bu randevu',
                selected: _scope == ManageCancelRequestScopeEnum.thisOnly,
                onTap: () => setState(
                  () => _scope = ManageCancelRequestScopeEnum.thisOnly,
                ),
              ),
              SelectChip(
                label: 'Bu ve sonrakiler',
                selected:
                    _scope == ManageCancelRequestScopeEnum.thisAndFollowing,
                onTap: () => setState(
                  () => _scope = ManageCancelRequestScopeEnum.thisAndFollowing,
                ),
              ),
            ],
          ),
        ],
        const SizedBox(height: OmvilaBosluk.bosluk2),
        SwitchListTile(
          contentPadding: EdgeInsets.zero,
          value: _notify,
          onChanged: (v) => setState(() => _notify = v),
          title: const Text('Müşteriye bildir'),
          subtitle: const Text('İptal SMS’i gider'),
        ),
        if (canOfferWaitlist)
          SwitchListTile(
            contentPadding: EdgeInsets.zero,
            value: _offerToWaitlist,
            onChanged: (v) => setState(() => _offerToWaitlist = v),
            title: const Text('Saati bekleme listesine aç'),
            subtitle: const Text('Sıradaki uygun kişiye teklif edilir'),
          ),
        const SizedBox(height: OmvilaBosluk.bosluk3),
        OmvilaButon(
          etiket: 'Randevuyu iptal et',
          tur: OmvilaButonTuru.tehlike,
          tamGenislik: true,
          yukleniyor: _busy,
          yukleniyorEtiketi: 'İptal ediliyor…',
          onPressed: _busy
              ? null
              : () => _cancel(canOfferWaitlist: canOfferWaitlist),
        ),
      ],
    );
  }
}
