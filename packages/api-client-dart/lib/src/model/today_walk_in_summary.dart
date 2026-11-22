//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'today_walk_in_summary.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TodayWalkInSummary {
  /// Returns a new [TodayWalkInSummary] instance.
  TodayWalkInSummary({

    required  this.waitingCount,

     this.estimatedWaitMinutesForNew,
  });

      /// Sırada bekleyen
          // minimum: 0
  @JsonKey(
    
    name: r'waitingCount',
    required: true,
    includeIfNull: false,
  )


  final int waitingCount;



      /// Yeni gelen için tahmini bekleme; müsait personel yoksa null
  @JsonKey(
    
    name: r'estimatedWaitMinutesForNew',
    required: false,
    includeIfNull: false,
  )


  final int? estimatedWaitMinutesForNew;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TodayWalkInSummary &&
      other.waitingCount == waitingCount &&
      other.estimatedWaitMinutesForNew == estimatedWaitMinutesForNew;

    @override
    int get hashCode =>
        waitingCount.hashCode +
        (estimatedWaitMinutesForNew == null ? 0 : estimatedWaitMinutesForNew.hashCode);

  factory TodayWalkInSummary.fromJson(Map<String, dynamic> json) => _$TodayWalkInSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$TodayWalkInSummaryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

