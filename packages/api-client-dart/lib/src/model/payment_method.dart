//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Tahsilat yöntemi. OMVILA para almaz; bu yalnız salonun kendi tahsilatının kaydıdır. Cash = nakit, Card = salonun kendi POS'u, Transfer = havale/EFT, PackageRedemption = paketten düşülen kalem (0 TL'lik kayıt, docs/acik-sorular.md T-86). İstemciler bilinmeyen değeri genel yöntem olarak gösterir. 
enum PaymentMethod {
  @JsonValue(r'Cash')
  cash(r'Cash'),
  @JsonValue(r'Card')
  card(r'Card'),
  @JsonValue(r'Transfer')
  transfer(r'Transfer'),
  @JsonValue(r'PackageRedemption')
  packageRedemption(r'PackageRedemption'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const PaymentMethod(this.value);

  final String value;

  @override
  String toString() => value;
}
