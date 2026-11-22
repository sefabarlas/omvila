//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'day_search_result_days_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DaySearchResultDaysInner {
  /// Returns a new [DaySearchResultDaysInner] instance.
  DaySearchResultDaysInner({

    required  this.date,

    required  this.hasAvailability,

     this.isClosed,

     this.firstSlotUtc,
  });

  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



  @JsonKey(
    
    name: r'hasAvailability',
    required: true,
    includeIfNull: false,
  )


  final bool hasAvailability;



  @JsonKey(
    
    name: r'isClosed',
    required: false,
    includeIfNull: false,
  )


  final bool? isClosed;



  @JsonKey(
    
    name: r'firstSlotUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? firstSlotUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DaySearchResultDaysInner &&
      other.date == date &&
      other.hasAvailability == hasAvailability &&
      other.isClosed == isClosed &&
      other.firstSlotUtc == firstSlotUtc;

    @override
    int get hashCode =>
        date.hashCode +
        hasAvailability.hashCode +
        isClosed.hashCode +
        (firstSlotUtc == null ? 0 : firstSlotUtc.hashCode);

  factory DaySearchResultDaysInner.fromJson(Map<String, dynamic> json) => _$DaySearchResultDaysInnerFromJson(json);

  Map<String, dynamic> toJson() => _$DaySearchResultDaysInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

