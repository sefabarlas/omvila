//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'waitlist_overview.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WaitlistOverview {
  /// Returns a new [WaitlistOverview] instance.
  WaitlistOverview({

    required  this.waitingCount,

     this.oldestJoinedUtc,

    required  this.convertedThisMonth,

    required  this.offerHoldMinutes,
  });

      /// \"Bekleyen\"
  @JsonKey(
    
    name: r'waitingCount',
    required: true,
    includeIfNull: false,
  )


  final int waitingCount;



      /// \"en eskisi 6 gündür\"
  @JsonKey(
    
    name: r'oldestJoinedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? oldestJoinedUtc;



      /// \"Bu ay yerleşen · boşalan saatlerden\"
  @JsonKey(
    
    name: r'convertedThisMonth',
    required: true,
    includeIfNull: false,
  )


  final int convertedThisMonth;



      /// \"Teklif süresi\" (WaitlistOfferHoldMinutes)
  @JsonKey(
    
    name: r'offerHoldMinutes',
    required: true,
    includeIfNull: false,
  )


  final int offerHoldMinutes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WaitlistOverview &&
      other.waitingCount == waitingCount &&
      other.oldestJoinedUtc == oldestJoinedUtc &&
      other.convertedThisMonth == convertedThisMonth &&
      other.offerHoldMinutes == offerHoldMinutes;

    @override
    int get hashCode =>
        waitingCount.hashCode +
        (oldestJoinedUtc == null ? 0 : oldestJoinedUtc.hashCode) +
        convertedThisMonth.hashCode +
        offerHoldMinutes.hashCode;

  factory WaitlistOverview.fromJson(Map<String, dynamic> json) => _$WaitlistOverviewFromJson(json);

  Map<String, dynamic> toJson() => _$WaitlistOverviewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

