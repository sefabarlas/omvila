//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/slot.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'slot_search_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SlotSearchResult {
  /// Returns a new [SlotSearchResult] instance.
  SlotSearchResult({

    required  this.timeZone,

    required  this.date,

    required  this.slots,

    required  this.unavailableTimes,
  });

      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



  @JsonKey(
    
    name: r'slots',
    required: true,
    includeIfNull: false,
  )


  final List<Slot> slots;



      /// Izgarada dolu görünen saatler (üstü çizili gösterim için)
  @JsonKey(
    
    name: r'unavailableTimes',
    required: true,
    includeIfNull: false,
  )


  final List<String> unavailableTimes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SlotSearchResult &&
      other.timeZone == timeZone &&
      other.date == date &&
      other.slots == slots &&
      other.unavailableTimes == unavailableTimes;

    @override
    int get hashCode =>
        timeZone.hashCode +
        date.hashCode +
        slots.hashCode +
        unavailableTimes.hashCode;

  factory SlotSearchResult.fromJson(Map<String, dynamic> json) => _$SlotSearchResultFromJson(json);

  Map<String, dynamic> toJson() => _$SlotSearchResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

