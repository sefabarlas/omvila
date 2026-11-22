import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/features/booking/domain/booking_draft.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Hizmet fiyatı ("450 ₺", "1.100 ₺'den"); salon fiyat göstermiyorsa null.
String? hizmetFiyati(PublicCatalogServicesInner s) {
  final price = s.price;
  if (price == null) {
    return null;
  }

  return formatPrice(
    price.amountMinor,
    display:
        s.priceDisplay ==
            PublicCatalogServicesInnerPriceDisplayEnum.startingFrom
        ? PriceDisplay.startingFrom
        : PriceDisplay.fixed,
  );
}

/// Alt çubuk tutarı: "450 ₺", "1.100 ₺'den", fiyat gizliyse süre.
String tutarMetni(BookingSummary s) {
  if (s.pricesHidden && s.totalMinor == 0) {
    return formatDuration(s.durationMinutes);
  }

  return formatPrice(
    s.totalMinor,
    display: s.startingFrom || s.pricesHidden
        ? PriceDisplay.startingFrom
        : PriceDisplay.fixed,
  );
}

/// "2 hizmet · 1 sa 15 dk".
String ozetMetni(BookingSummary s) =>
    '${s.services.length} hizmet · ${formatDuration(s.durationMinutes)}';
