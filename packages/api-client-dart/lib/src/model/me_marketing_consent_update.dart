//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_marketing_consent_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeMarketingConsentUpdate {
  /// Returns a new [MeMarketingConsentUpdate] instance.
  MeMarketingConsentUpdate({

    required  this.brandId,

    required  this.granted,
  });

  @JsonKey(
    
    name: r'brandId',
    required: true,
    includeIfNull: false,
  )


  final String brandId;



  @JsonKey(
    
    name: r'granted',
    required: true,
    includeIfNull: false,
  )


  final bool granted;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeMarketingConsentUpdate &&
      other.brandId == brandId &&
      other.granted == granted;

    @override
    int get hashCode =>
        brandId.hashCode +
        granted.hashCode;

  factory MeMarketingConsentUpdate.fromJson(Map<String, dynamic> json) => _$MeMarketingConsentUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$MeMarketingConsentUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

