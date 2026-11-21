//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'commission_week_day.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CommissionWeekDay {
  /// Returns a new [CommissionWeekDay] instance.
  CommissionWeekDay({

    required  this.date,

    required  this.amountMinor,

    required  this.appointmentCount,
  });

      /// Şubenin yerel günü
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )


  final DateTime date;



      /// Günün prim toplamı (kuruş)
  @JsonKey(
    
    name: r'amountMinor',
    required: true,
    includeIfNull: false,
  )


  final int amountMinor;



      /// Günün tamamlanan randevu sayısı (\"Sal en yoğun\")
          // minimum: 0
  @JsonKey(
    
    name: r'appointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int appointmentCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CommissionWeekDay &&
      other.date == date &&
      other.amountMinor == amountMinor &&
      other.appointmentCount == appointmentCount;

    @override
    int get hashCode =>
        date.hashCode +
        amountMinor.hashCode +
        appointmentCount.hashCode;

  factory CommissionWeekDay.fromJson(Map<String, dynamic> json) => _$CommissionWeekDayFromJson(json);

  Map<String, dynamic> toJson() => _$CommissionWeekDayToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

