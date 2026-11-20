//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/slot_items_inner.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'slot.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class Slot {
  /// Returns a new [Slot] instance.
  Slot({

    required  this.startUtc,

    required  this.localTime,

     this.endLocalTime,

    required  this.period,

    required  this.items,

    required  this.total,
  });

  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



      /// Yerel saat HH:mm (şubenin saat dilimine göre)
  @JsonKey(
    
    name: r'localTime',
    required: true,
    includeIfNull: false,
  )


  final String localTime;



      /// Yerel saat HH:mm (şubenin saat dilimine göre)
  @JsonKey(
    
    name: r'endLocalTime',
    required: false,
    includeIfNull: false,
  )


  final String? endLocalTime;



      /// Sabah < 12:00 ≤ Öğleden sonra < 17:00 ≤ Akşam
  @JsonKey(
    
    name: r'period',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SlotPeriodEnum.unknownDefaultOpenApi,
  )


  final SlotPeriodEnum period;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<SlotItemsInner> items;



  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final Money total;





    @override
    bool operator ==(Object other) => identical(this, other) || other is Slot &&
      other.startUtc == startUtc &&
      other.localTime == localTime &&
      other.endLocalTime == endLocalTime &&
      other.period == period &&
      other.items == items &&
      other.total == total;

    @override
    int get hashCode =>
        startUtc.hashCode +
        localTime.hashCode +
        endLocalTime.hashCode +
        period.hashCode +
        items.hashCode +
        total.hashCode;

  factory Slot.fromJson(Map<String, dynamic> json) => _$SlotFromJson(json);

  Map<String, dynamic> toJson() => _$SlotToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Sabah < 12:00 ≤ Öğleden sonra < 17:00 ≤ Akşam
enum SlotPeriodEnum {
@JsonValue(r'Morning')
morning(r'Morning'),
@JsonValue(r'Afternoon')
afternoon(r'Afternoon'),
@JsonValue(r'Evening')
evening(r'Evening'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const SlotPeriodEnum(this.value);

final String value;

@override
String toString() => value;
}


