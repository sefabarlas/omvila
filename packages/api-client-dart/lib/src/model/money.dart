//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'money.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class Money {
  /// Returns a new [Money] instance.
  Money({

    required  this.amountMinor,

    required  this.currency,
  });

          // minimum: 0
  @JsonKey(
    
    name: r'amountMinor',
    required: true,
    includeIfNull: false,
  )


  final int amountMinor;



  @JsonKey(
    
    name: r'currency',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MoneyCurrencyEnum.unknownDefaultOpenApi,
  )


  final MoneyCurrencyEnum currency;





    @override
    bool operator ==(Object other) => identical(this, other) || other is Money &&
      other.amountMinor == amountMinor &&
      other.currency == currency;

    @override
    int get hashCode =>
        amountMinor.hashCode +
        currency.hashCode;

  factory Money.fromJson(Map<String, dynamic> json) => _$MoneyFromJson(json);

  Map<String, dynamic> toJson() => _$MoneyToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum MoneyCurrencyEnum {
@JsonValue(r'TRY')
TRY(r'TRY'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const MoneyCurrencyEnum(this.value);

final String value;

@override
String toString() => value;
}


