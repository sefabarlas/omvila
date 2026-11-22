//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'commission_day_amount.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CommissionDayAmount {
  /// Returns a new [CommissionDayAmount] instance.
  CommissionDayAmount({

    required  this.date,

    required  this.amountMinor,
  });

      /// Şubenin yerel günü
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;



      /// O günün prim toplamı (kuruş; ters kayıt varsa eksi olabilir)
  @JsonKey(
    
    name: r'amountMinor',
    required: true,
    includeIfNull: false,
  )


  final int amountMinor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CommissionDayAmount &&
      other.date == date &&
      other.amountMinor == amountMinor;

    @override
    int get hashCode =>
        date.hashCode +
        amountMinor.hashCode;

  factory CommissionDayAmount.fromJson(Map<String, dynamic> json) => _$CommissionDayAmountFromJson(json);

  Map<String, dynamic> toJson() => _$CommissionDayAmountToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

