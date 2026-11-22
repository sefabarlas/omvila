//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_subscription_page_counts.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSubscriptionPageCounts {
  /// Returns a new [AdminSubscriptionPageCounts] instance.
  AdminSubscriptionPageCounts({

    required  this.active,

    required  this.trialing,

    required  this.trialEndingThisWeek,

    required  this.pastDue,

    required  this.readOnly,
  });

      /// \"AKTİF\"
          // minimum: 0
  @JsonKey(
    
    name: r'active',
    required: true,
    includeIfNull: false,
  )


  final int active;



      /// \"DENEMEDE\"
          // minimum: 0
  @JsonKey(
    
    name: r'trialing',
    required: true,
    includeIfNull: false,
  )


  final int trialing;



      /// \"9'u bu hafta bitiyor\"
          // minimum: 0
  @JsonKey(
    
    name: r'trialEndingThisWeek',
    required: true,
    includeIfNull: false,
  )


  final int trialEndingThisWeek;



      /// \"ÖDEME GECİKTİ\"
          // minimum: 0
  @JsonKey(
    
    name: r'pastDue',
    required: true,
    includeIfNull: false,
  )


  final int pastDue;



      /// \"SALT OKUNUR\"
          // minimum: 0
  @JsonKey(
    
    name: r'readOnly',
    required: true,
    includeIfNull: false,
  )


  final int readOnly;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSubscriptionPageCounts &&
      other.active == active &&
      other.trialing == trialing &&
      other.trialEndingThisWeek == trialEndingThisWeek &&
      other.pastDue == pastDue &&
      other.readOnly == readOnly;

    @override
    int get hashCode =>
        active.hashCode +
        trialing.hashCode +
        trialEndingThisWeek.hashCode +
        pastDue.hashCode +
        readOnly.hashCode;

  factory AdminSubscriptionPageCounts.fromJson(Map<String, dynamic> json) => _$AdminSubscriptionPageCountsFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSubscriptionPageCountsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

