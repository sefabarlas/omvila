import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/new_appointment/data/booking_repository.dart';
import 'package:omvila_isletme/shared/ui.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Personel seçimi: yuvarlak baş harfler; "Fark etmez" isteğe bağlı.
class StaffPicker extends StatelessWidget {
  /// Seçici.
  const StaffPicker({
    required this.staff,
    required this.selected,
    required this.onSelect,
    this.allowAny = true,
    super.key,
  });

  /// Seçilebilir personel.
  final List<StaffSummary> staff;

  /// Seçili personel (null = fark etmez).
  final String? selected;

  /// Seçildi.
  final ValueChanged<String?> onSelect;

  /// "Fark etmez" gösterilsin mi.
  final bool allowAny;

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        spacing: OmvilaBosluk.bosluk3,
        children: [
          if (allowAny)
            _Person(
              label: 'Fark etmez',
              selected: selected == null,
              onTap: () => onSelect(null),
              child: Icon(
                LucideIcons.users,
                size: 22,
                color: context.omvilaRenkler.metinIkincil,
              ),
            ),
          for (final s in staff)
            _Person(
              label: s.displayName ?? s.fullName.split(' ').first,
              selected: selected == s.id,
              onTap: () => onSelect(s.id),
              child: Text(
                initialsOf(s.fullName),
                style: OmvilaYazi.baslik3.copyWith(
                  color: context.omvilaRenkler.metin,
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _Person extends StatelessWidget {
  const _Person({
    required this.label,
    required this.selected,
    required this.onTap,
    required this.child,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return Semantics(
      button: true,
      selected: selected,
      label: label,
      excludeSemantics: true,
      child: GestureDetector(
        onTap: onTap,
        behavior: HitTestBehavior.opaque,
        child: SizedBox(
          width: 64,
          child: Column(
            children: [
              Container(
                width: 56,
                height: 56,
                alignment: Alignment.center,
                decoration: BoxDecoration(
                  color: selected ? r.zeminVurguYumusak : r.zeminIkincil,
                  shape: BoxShape.circle,
                  border: Border.all(
                    color: selected ? r.marka : Colors.transparent,
                    width: 2,
                  ),
                ),
                child: child,
              ),
              const SizedBox(height: OmvilaBosluk.bosluk1),
              Text(
                label,
                textAlign: TextAlign.center,
                style: OmvilaYazi.aciklama.copyWith(
                  color: selected ? r.metinMarka : r.metin,
                  fontWeight: selected ? FontWeight.w700 : FontWeight.w500,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

/// Saat ızgarası: boş saatler seçilebilir, dolu saatler üstü çizili.
class SlotGrid extends ConsumerWidget {
  /// Izgara.
  const SlotGrid({
    required this.query,
    required this.selected,
    required this.onSelect,
    super.key,
  });

  /// Arama.
  final SlotQuery query;

  /// Seçili saatin başlangıcı.
  final DateTime? selected;

  /// Seçildi.
  final ValueChanged<Slot> onSelect;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final r = context.omvilaRenkler;
    final result = ref.watch(slotsProvider(query));
    final data = result.value;
    if (data == null) {
      return result.hasError
          ? LoadError(
              title: 'Saatler yüklenemedi',
              error: result.error,
              onRetry: () => ref.invalidate(slotsProvider(query)),
            )
          : const Loading();
    }

    final cells = <(String, Slot?)>[
      for (final s in data.slots) (s.localTime, s),
      for (final t in data.unavailableTimes)
        if (!data.slots.any((s) => s.localTime == t)) (t, null),
    ]..sort((a, b) => a.$1.compareTo(b.$1));
    if (cells.isEmpty) {
      return Padding(
        padding: const EdgeInsets.symmetric(vertical: OmvilaBosluk.bosluk3),
        child: Text(
          'Bu gün için boş saat yok. Başka bir gün seçin.',
          style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
        ),
      );
    }

    return LayoutBuilder(
      builder: (context, constraints) {
        const gap = OmvilaBosluk.bosluk2;
        final width = (constraints.maxWidth - gap * 3) / 4;
        return Wrap(
          spacing: gap,
          runSpacing: gap,
          children: [
            for (final (time, slot) in cells)
              SizedBox(
                width: width,
                child: _SlotCell(
                  time: time,
                  selected: slot != null && slot.startUtc == selected,
                  onTap: slot == null ? null : () => onSelect(slot),
                ),
              ),
          ],
        );
      },
    );
  }
}

class _SlotCell extends StatelessWidget {
  const _SlotCell({
    required this.time,
    required this.selected,
    required this.onTap,
  });

  final String time;
  final bool selected;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final taken = onTap == null;
    final radius = BorderRadius.circular(OmvilaKose.koseSm);
    return Semantics(
      container: true,
      button: !taken,
      selected: selected,
      label: taken ? '$time, dolu' : time,
      excludeSemantics: true,
      child: Material(
        color: selected
            ? r.zeminMarka
            : (taken ? r.zeminIkincil : r.zeminYukseltilmis),
        shape: RoundedRectangleBorder(
          borderRadius: radius,
          side: BorderSide(
            color: selected || taken ? Colors.transparent : r.cizgi,
          ),
        ),
        child: InkWell(
          onTap: onTap,
          borderRadius: radius,
          child: Container(
            height: OmvilaBoyut.dokunmaMin,
            alignment: Alignment.center,
            child: Text(
              time,
              style: OmvilaYazi.govdeGuclu.copyWith(
                color: selected
                    ? r.metinTers
                    : (taken ? r.metinDevreDisi : r.metin),
                decoration: taken ? TextDecoration.lineThrough : null,
              ),
            ),
          ),
        ),
      ),
    );
  }
}
