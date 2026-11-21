//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/sms_sender.dart';
import 'package:omvila_api/src/model/sms_overview_sent_this_month.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'sms_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SmsOverview {
  /// Returns a new [SmsOverview] instance.
  SmsOverview({

    required  this.creditBalance,

    required  this.estimatedDaysLeft,

    required  this.lowCreditThreshold,

    required  this.isLow,

    required  this.sentThisMonth,

    required  this.sender,
  });

      /// SMS kontörü bakiyesi
          // minimum: 0
  @JsonKey(
    
    name: r'creditBalance',
    required: true,
    includeIfNull: false,
  )


  final int creditBalance;



      /// \"bu tempoyla ~18 gün yeter\"; son 30 günün ortalama tüketimine göre. Yeterli geçmiş yoksa veya tüketim yoksa null
          // minimum: 0
  @JsonKey(
    
    name: r'estimatedDaysLeft',
    required: true,
    includeIfNull: true,
  )


  final int? estimatedDaysLeft;



      /// Bu değerin altında \"Kontör az\" uyarısı (LowCreditThreshold)
  @JsonKey(
    
    name: r'lowCreditThreshold',
    required: true,
    includeIfNull: false,
  )


  final int lowCreditThreshold;



      /// creditBalance < lowCreditThreshold
  @JsonKey(
    
    name: r'isLow',
    required: true,
    includeIfNull: false,
  )


  final bool isLow;



  @JsonKey(
    
    name: r'sentThisMonth',
    required: true,
    includeIfNull: false,
  )


  final SmsOverviewSentThisMonth sentThisMonth;



  @JsonKey(
    
    name: r'sender',
    required: true,
    includeIfNull: false,
  )


  final SmsSender sender;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SmsOverview &&
      other.creditBalance == creditBalance &&
      other.estimatedDaysLeft == estimatedDaysLeft &&
      other.lowCreditThreshold == lowCreditThreshold &&
      other.isLow == isLow &&
      other.sentThisMonth == sentThisMonth &&
      other.sender == sender;

    @override
    int get hashCode =>
        creditBalance.hashCode +
        (estimatedDaysLeft == null ? 0 : estimatedDaysLeft.hashCode) +
        lowCreditThreshold.hashCode +
        isLow.hashCode +
        sentThisMonth.hashCode +
        sender.hashCode;

  factory SmsOverview.fromJson(Map<String, dynamic> json) => _$SmsOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$SmsOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

