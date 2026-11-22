//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/day_search_result_days_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'day_search_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DaySearchResult {
  /// Returns a new [DaySearchResult] instance.
  DaySearchResult({

    required  this.timeZone,

     this.maxDate,

    required  this.days,

     this.nextAvailableUtc,
  });

      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// \"60 gün ileriye kadar\"
  @JsonKey(
    
    name: r'maxDate',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? maxDate;



  @JsonKey(
    
    name: r'days',
    required: true,
    includeIfNull: false,
  )


  final List<DaySearchResultDaysInner> days;



      /// Aralıkta hiç boş gün yoksa ilk uygun saat (\"en yakın uygun saat Cuma 10:30\")
  @JsonKey(
    
    name: r'nextAvailableUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? nextAvailableUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DaySearchResult &&
      other.timeZone == timeZone &&
      other.maxDate == maxDate &&
      other.days == days &&
      other.nextAvailableUtc == nextAvailableUtc;

    @override
    int get hashCode =>
        timeZone.hashCode +
        maxDate.hashCode +
        days.hashCode +
        (nextAvailableUtc == null ? 0 : nextAvailableUtc.hashCode);

  factory DaySearchResult.fromJson(Map<String, dynamic> json) => _$DaySearchResultFromJson(json);

  Map<String, dynamic> toJson() => _$DaySearchResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

