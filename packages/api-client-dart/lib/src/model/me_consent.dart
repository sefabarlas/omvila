//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand_ref.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_consent.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeConsent {
  /// Returns a new [MeConsent] instance.
  MeConsent({

    required  this.brand,

    required  this.marketingGranted,

    required  this.updatedUtc,
  });

  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final MeBrandRef brand;



      /// Son kampanya rızası verilmiş mi
  @JsonKey(
    
    name: r'marketingGranted',
    required: true,
    includeIfNull: false,
  )


  final bool marketingGranted;



      /// Son rıza kaydının anı; hiç kayıt yoksa null
  @JsonKey(
    
    name: r'updatedUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? updatedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeConsent &&
      other.brand == brand &&
      other.marketingGranted == marketingGranted &&
      other.updatedUtc == updatedUtc;

    @override
    int get hashCode =>
        brand.hashCode +
        marketingGranted.hashCode +
        (updatedUtc == null ? 0 : updatedUtc.hashCode);

  factory MeConsent.fromJson(Map<String, dynamic> json) => _$MeConsentFromJson(json);

  Map<String, dynamic> toJson() => _$MeConsentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

