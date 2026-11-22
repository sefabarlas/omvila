//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'subscription_banner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SubscriptionBanner {
  /// Returns a new [SubscriptionBanner] instance.
  SubscriptionBanner({

    required  this.status,

     this.trialEndsOn,

     this.pastDueSince,

     this.readOnlyOn,
  });

  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SubscriptionBannerStatusEnum.unknownDefaultOpenApi,
  )


  final SubscriptionBannerStatusEnum status;



  @JsonKey(
    
    name: r'trialEndsOn',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? trialEndsOn;



  @JsonKey(
    
    name: r'pastDueSince',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? pastDueSince;



      /// PastDue ise salt okunura geçeceği gün
  @JsonKey(
    
    name: r'readOnlyOn',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? readOnlyOn;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SubscriptionBanner &&
      other.status == status &&
      other.trialEndsOn == trialEndsOn &&
      other.pastDueSince == pastDueSince &&
      other.readOnlyOn == readOnlyOn;

    @override
    int get hashCode =>
        status.hashCode +
        (trialEndsOn == null ? 0 : trialEndsOn.hashCode) +
        (pastDueSince == null ? 0 : pastDueSince.hashCode) +
        (readOnlyOn == null ? 0 : readOnlyOn.hashCode);

  factory SubscriptionBanner.fromJson(Map<String, dynamic> json) => _$SubscriptionBannerFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionBannerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum SubscriptionBannerStatusEnum {
@JsonValue(r'Trialing')
trialing(r'Trialing'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'PastDue')
pastDue(r'PastDue'),
@JsonValue(r'ReadOnly')
readOnly(r'ReadOnly'),
@JsonValue(r'Cancelled')
cancelled(r'Cancelled'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const SubscriptionBannerStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


