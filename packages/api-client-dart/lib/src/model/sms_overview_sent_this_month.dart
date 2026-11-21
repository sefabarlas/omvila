//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'sms_overview_sent_this_month.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SmsOverviewSentThisMonth {
  /// Returns a new [SmsOverviewSentThisMonth] instance.
  SmsOverviewSentThisMonth({

    required  this.month,

    required  this.smsCount,

    required  this.creditsUsed,

    required  this.notificationCount,
  });

      /// Ayın ilk günü (yerel)
  @JsonKey(
    
    name: r'month',
    required: true,
    includeIfNull: false,
  )


  final DateTime month;



      /// Gönderilen SMS sayısı (\"862 SMS\")
          // minimum: 0
  @JsonKey(
    
    name: r'smsCount',
    required: true,
    includeIfNull: false,
  )


  final int smsCount;



      /// Düşülen kontör
          // minimum: 0
  @JsonKey(
    
    name: r'creditsUsed',
    required: true,
    includeIfNull: false,
  )


  final int creditsUsed;



      /// Bütün kanallarda gönderilen bildirim (\"1.904 bildirim\")
          // minimum: 0
  @JsonKey(
    
    name: r'notificationCount',
    required: true,
    includeIfNull: false,
  )


  final int notificationCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SmsOverviewSentThisMonth &&
      other.month == month &&
      other.smsCount == smsCount &&
      other.creditsUsed == creditsUsed &&
      other.notificationCount == notificationCount;

    @override
    int get hashCode =>
        month.hashCode +
        smsCount.hashCode +
        creditsUsed.hashCode +
        notificationCount.hashCode;

  factory SmsOverviewSentThisMonth.fromJson(Map<String, dynamic> json) => _$SmsOverviewSentThisMonthFromJson(json);

  Map<String, dynamic> toJson() => _$SmsOverviewSentThisMonthToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

