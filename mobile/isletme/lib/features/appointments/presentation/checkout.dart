import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/features/appointments/presentation/appointment_format.dart';

// "Tamamla ve ödeme al" (OdemeAl) hesapları; salon panelindeki
// web/salon-panel/src/features/checkout/checkout.ts ile aynı kurallar. Para
// kuruş cinsinden tamsayıdır; alınacak tutarın kaynağı sunucunun
// amountDue'sudur, istemci yalnız şimdi "Paketten" düşülen kalemleri çıkarır.
// Uyuşmazlıkta sunucu 409 payment_total_mismatch döner.

/// Bir tamamlamada en çok bu kadar ödeme satırı (sözleşme maxItems 3; tasarım
/// "İkinci ödeme ekle" ile iki satır).
const maxPaymentRows = 2;

/// Yöntem adı ("Kart" salonun kendi POS'u; uygulama ödeme almaz).
String methodLabel(CollectedPaymentMethod m) => switch (m) {
  CollectedPaymentMethod.cash => 'Nakit',
  CollectedPaymentMethod.card => 'Kart',
  CollectedPaymentMethod.transfer => 'Havale',
  CollectedPaymentMethod.unknownDefaultOpenApi => 'Diğer',
};

/// Seçilebilen yöntemler.
const List<CollectedPaymentMethod> collectedMethods = [
  CollectedPaymentMethod.cash,
  CollectedPaymentMethod.card,
  CollectedPaymentMethod.transfer,
];

/// Salonda kalan kapora (alınan − iade edilen); kapora OMVILA'dan geçmez,
/// salonun IBAN'ına gelen havaledir. Yoksa 0.
int depositHeld(AppointmentDetail a) {
  final payments = [
    for (final p in a.payments ?? const <PaymentView>[])
      if (p.isDeposit) p,
  ];
  if (payments.isNotEmpty) {
    return payments.fold(
      0,
      (sum, p) =>
          sum + p.amount.amountMinor - (p.refundedAmount?.amountMinor ?? 0),
    );
  }

  final d = a.deposit;
  if (d == null ||
      (d.status != DepositStatus.received &&
          d.status != DepositStatus.refunded)) {
    return 0;
  }

  return d.required_.amountMinor - (d.refundedAmount?.amountMinor ?? 0);
}

/// Şimdi paketten düşülebilecek kalem.
class RedemptionMatch {
  /// Eşleşme.
  const RedemptionMatch({
    required this.request,
    required this.serviceName,
    required this.packageName,
    required this.amountMinor,
  });

  /// Sunucuya giden düşüm.
  final PackageRedemptionRequest request;

  /// "Fön".
  final String serviceName;

  /// "Fön paketi".
  final String packageName;

  /// Kalemin tutarı.
  final int amountMinor;
}

/// Kalem paketten ödenmemiş ve etkin; paket bu müşterinin, Active, süresi
/// geçmemiş, şubede kullanılabilir ve aynı hizmetin kalan seansı var. Sunucu
/// aynı kuralı denetler (409 package_session_unavailable).
List<RedemptionMatch> redemptionMatches(
  AppointmentDetail a,
  List<CustomerPackage> packages,
  String today,
) {
  final usable = [
    for (final p in packages)
      if (p.status == CustomerPackageStatus.active &&
          _date(p.expiresOn).compareTo(today) >= 0 &&
          (p.isAvailableAllBranches || p.branchId == a.branchId))
        p,
  ];
  final remaining = <String, int>{
    for (final p in usable)
      for (final i in p.items) i.customerPackageItemId: i.remainingSessions,
  };
  final matches = <RedemptionMatch>[];
  for (final item in a.items) {
    if ((item.paidFromPackage ?? false) ||
        item.status != AppointmentItemViewStatusEnum.active) {
      continue;
    }

    for (final p in usable) {
      final candidates = p.items.where(
        (c) =>
            c.serviceId == item.serviceId &&
            (remaining[c.customerPackageItemId] ?? 0) > 0,
      );
      if (candidates.isEmpty) {
        continue;
      }

      final packageItem = candidates.first;
      remaining[packageItem.customerPackageItemId] =
          remaining[packageItem.customerPackageItemId]! - 1;
      matches.add(
        RedemptionMatch(
          request: PackageRedemptionRequest(
            appointmentItemId: item.id,
            customerPackageItemId: packageItem.customerPackageItemId,
          ),
          serviceName: item.serviceName,
          packageName: p.packageName,
          amountMinor: itemAmount(item),
        ),
      );
      break;
    }
  }

  return matches;
}

String _date(DateTime d) =>
    '${d.year.toString().padLeft(4, '0')}-'
    '${d.month.toString().padLeft(2, '0')}-'
    '${d.day.toString().padLeft(2, '0')}';

/// Alınacak: amountDue − şimdi paketten düşülenler (eksiye inmez).
int amountToCollect(AppointmentDetail a, List<RedemptionMatch> redemptions) {
  final redeemed = redemptions.fold(0, (sum, r) => sum + r.amountMinor);
  final due = a.amountDue.amountMinor - redeemed;
  return due < 0 ? 0 : due;
}
