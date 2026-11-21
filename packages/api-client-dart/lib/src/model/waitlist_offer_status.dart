//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// Pending = teklif gönderildi, saat tutuluyor; Accepted = \"Randevuyu al\"; Declined = \"Bu saat olmaz\"; Expired = 15 dk doldu; Withdrawn = salon \"Teklifi geri al\" dedi
enum WaitlistOfferStatus {
  @JsonValue(r'Pending')
  pending(r'Pending'),
  @JsonValue(r'Accepted')
  accepted(r'Accepted'),
  @JsonValue(r'Declined')
  declined(r'Declined'),
  @JsonValue(r'Expired')
  expired(r'Expired'),
  @JsonValue(r'Withdrawn')
  withdrawn(r'Withdrawn'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const WaitlistOfferStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
