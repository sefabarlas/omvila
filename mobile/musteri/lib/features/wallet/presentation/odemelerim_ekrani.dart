import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:lucide_icons_flutter/lucide_icons.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/domain/randevu_metinleri.dart';
import 'package:omvila_musteri/shared/widgets/durum_desenleri.dart';
import 'package:omvila_musteri/shared/widgets/ic_sayfa.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Ödeme sayfaları.
@immutable
class PaymentList {
  /// Liste.
  const PaymentList(this.items, this.nextCursor, {this.loadingMore = false});

  /// Satırlar (yeniden eskiye).
  final List<MePaymentRow> items;

  /// Sonraki sayfa.
  final String? nextCursor;

  /// Sonraki sayfa okunuyor.
  final bool loadingMore;
}

/// Ödemelerim (meListPayments).
final AsyncNotifierProvider<PaymentsController, PaymentList> paymentsProvider =
    AsyncNotifierProvider.autoDispose<PaymentsController, PaymentList>(
      PaymentsController.new,
    );

/// Ödeme listesi ve sonraki sayfalar.
class PaymentsController extends AsyncNotifier<PaymentList> {
  @override
  Future<PaymentList> build() async {
    final page = await ref.watch(walletRepositoryProvider).payments();
    return PaymentList(page.items, page.nextCursor);
  }

  /// "Daha fazla göster".
  Future<void> loadMore() async {
    final current = state.value;
    final cursor = current?.nextCursor;
    if (current == null || cursor == null || current.loadingMore) {
      return;
    }

    state = AsyncData(PaymentList(current.items, cursor, loadingMore: true));
    try {
      final page = await ref
          .read(walletRepositoryProvider)
          .payments(cursor: cursor);
      state = AsyncData(
        PaymentList([...current.items, ...page.items], page.nextCursor),
      );
    } on ApiProblem {
      state = AsyncData(current);
    }
  }
}

/// "Boya ve fön": son iki ad "ve" ile.
String hizmetAdlari(List<String> names) => switch (names.length) {
  0 => '',
  1 => names.first,
  _ =>
    '${names.sublist(0, names.length - 1).join(', ')} ve '
        '${names.last.toLowerCase()}',
};

/// Yöntem: "Havale", "Nakit", "Kartla, salonda" (salonun kendi POS'u;
/// uygulamada kart formu yok).
String yontemMetni(PaymentMethod method) => switch (method) {
  PaymentMethod.cash => 'Nakit',
  PaymentMethod.card => 'Kartla, salonda',
  PaymentMethod.transfer => 'Havale',
  PaymentMethod.packageRedemption => 'Paketten',
  PaymentMethod.unknownDefaultOpenApi => '',
};

/// "21 Kas".
String kisaGun(String localDate) =>
    formatDate(localDate, DateStyle.short).split(' ').take(2).join(' ');

/// Ödemelerim (Odemelerim, OdemelerimBos): salt okunur; kapora havaleleri
/// salon onayıyla, salondaki tahsilatlar ve paket satın almalar. Ay
/// başlıkları kaydın saat diliminde.
class OdemelerimEkrani extends ConsumerWidget {
  /// Ekran.
  const OdemelerimEkrani({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final list = ref.watch(paymentsProvider);
    final empty = list.value?.items.isEmpty ?? false;
    return IcSayfa(
      baslik: 'Ödemelerim',
      aciklama: empty
          ? null
          : "Uygulamadan ödeme alınmaz. Kaporalar salonun IBAN'ına "
                'havaleyle, diğer ödemeler salonda yapılır; burada '
                'kayıtlarını görürsünüz.',
      onRefresh: () => ref.refresh(paymentsProvider.future),
      children: switch (list) {
        AsyncData(:final value) when value.items.isEmpty => const [
          OmvilaBosDurum(
            ikon: LucideIcons.receipt,
            baslik: 'Ödeme kaydı yok',
            aciklama:
                'Kaporalı bir randevu aldığınızda havale bilgisi ve salonun '
                'onayı burada görünür. Uygulamadan ödeme alınmaz.',
          ),
        ],
        AsyncData(:final value) => _rows(context, ref, value),
        AsyncError(:final error) => [
          HataDurumu(
            error: error,
            onRetry: () => ref.invalidate(paymentsProvider),
          ),
        ],
        _ => const [
          Padding(
            padding: EdgeInsets.only(top: OmvilaBosluk.bosluk12),
            child: YukleniyorDurumu(),
          ),
        ],
      },
    );
  }

  List<Widget> _rows(BuildContext context, WidgetRef ref, PaymentList list) {
    final r = context.omvilaRenkler;
    final clock = ref.watch(clockProvider);
    final widgets = <Widget>[];
    String? month;
    for (final p in list.items) {
      final day = toBranchDate(p.occurredUtc, p.timeZone);
      final m = formatDate(day, DateStyle.monthYear);
      if (m != month) {
        month = m;
        final thisYear = branchToday(clock, p.timeZone).startsWith(
          day.substring(0, 4),
        );
        widgets.add(
          Padding(
            padding: const EdgeInsets.only(
              top: OmvilaBosluk.bosluk3,
              bottom: OmvilaBosluk.bosluk3,
            ),
            child: Semantics(
              header: true,
              child: Text(
                thisYear ? m.split(' ').first : m,
                style: OmvilaYazi.baslik3.copyWith(
                  color: r.metin,
                  fontWeight: FontWeight.w800,
                ),
              ),
            ),
          ),
        );
      }

      widgets.add(
        Padding(
          padding: const EdgeInsets.only(bottom: OmvilaBosluk.bosluk2),
          child: OdemeSatiri(odeme: p),
        ),
      );
    }

    if (list.nextCursor != null) {
      widgets.add(
        Center(
          child: list.loadingMore
              ? const Padding(
                  padding: EdgeInsets.all(OmvilaBosluk.bosluk4),
                  child: YukleniyorDurumu(),
                )
              : TextButton(
                  onPressed: () =>
                      ref.read(paymentsProvider.notifier).loadMore(),
                  child: const Text('Daha fazla göster'),
                ),
        ),
      );
    }

    return widgets;
  }
}

/// Ödeme satırı: salon, tutar, ne · gün · yöntem, kapora ve iade çipi.
class OdemeSatiri extends StatelessWidget {
  /// Satır.
  const OdemeSatiri({required this.odeme, super.key});

  /// Kayıt.
  final MePaymentRow odeme;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    final p = odeme;
    final day = kisaGun(toBranchDate(p.occurredUtc, p.timeZone));
    final what = switch (p.kind) {
      MePaymentKind.deposit => 'Kapora',
      MePaymentKind.appointmentPayment => hizmetAdlari(
        p.serviceNames ?? const [],
      ),
      MePaymentKind.packagePurchase => ozetSatiri([
        p.packageName ?? 'Paket',
        if (p.packageSessionCount case final n?) '$n seans',
      ]),
      MePaymentKind.unknownDefaultOpenApi => 'Ödeme',
    };
    final chip = _cip(r);
    return MergeSemantics(
      child: Container(
        padding: const EdgeInsets.all(OmvilaBosluk.bosluk4),
        decoration: BoxDecoration(
          color: r.zeminYukseltilmis,
          borderRadius: BorderRadius.circular(OmvilaKose.koseLg),
        ),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const IkonKutusu(LucideIcons.receipt),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    p.brand.name,
                    style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
                  ),
                  Text(
                    ozetSatiri([what, day, yontemMetni(p.method)]),
                    style: OmvilaYazi.ikincil.copyWith(color: r.metinIkincil),
                  ),
                  if (chip != null) ...[
                    const SizedBox(height: OmvilaBosluk.bosluk2),
                    chip,
                  ],
                ],
              ),
            ),
            const SizedBox(width: OmvilaBosluk.bosluk3),
            Text(
              formatMoney(p.amount.amountMinor),
              style: OmvilaYazi.govdeGuclu.copyWith(color: r.metin),
            ),
          ],
        ),
      ),
    );
  }

  Widget? _cip(OmvilaRenkler r) {
    final p = odeme;
    switch (p.status) {
      case MePaymentStatus.refunded || MePaymentStatus.partiallyRefunded:
        return EtiketCipi(
          'Salon iade etti',
          zemin: r.zeminIkincil,
          renk: r.metinIkincil,
        );
      case MePaymentStatus.awaitingConfirmation:
        return EtiketCipi(
          'Salon onayı bekleniyor',
          zemin: r.durumKaporaZemin,
          renk: r.durumKaporaMetin,
        );
      case MePaymentStatus.recorded when p.kind == MePaymentKind.deposit:
        final start = p.appointmentStartUtc;
        final day = start == null
            ? null
            : formatDate(toBranchDate(start, p.timeZone), DateStyle.dayMonth);
        return EtiketCipi(
          day == null ? 'Alındı' : 'Alındı · $day randevusu',
          zemin: r.durumOnaylandiZemin,
          renk: r.durumOnaylandiMetin,
        );
      case MePaymentStatus.recorded || MePaymentStatus.unknownDefaultOpenApi:
        return null;
    }
  }
}
