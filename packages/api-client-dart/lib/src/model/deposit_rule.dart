//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deposit_rule.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DepositRule {
  /// Returns a new [DepositRule] instance.
  DepositRule({

    required  this.mode,

     this.amount,

     this.percent,
  });

  @JsonKey(
    
    name: r'mode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DepositRuleModeEnum.unknownDefaultOpenApi,
  )


  final DepositRuleModeEnum mode;



  @JsonKey(
    
    name: r'amount',
    required: false,
    includeIfNull: false,
  )


  final Money? amount;



          // minimum: 1
          // maximum: 100
  @JsonKey(
    
    name: r'percent',
    required: false,
    includeIfNull: false,
  )


  final num? percent;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DepositRule &&
      other.mode == mode &&
      other.amount == amount &&
      other.percent == percent;

    @override
    int get hashCode =>
        mode.hashCode +
        amount.hashCode +
        percent.hashCode;

  factory DepositRule.fromJson(Map<String, dynamic> json) => _$DepositRuleFromJson(json);

  Map<String, dynamic> toJson() => _$DepositRuleToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum DepositRuleModeEnum {
@JsonValue(r'Inherit')
inherit(r'Inherit'),
@JsonValue(r'Always')
always(r'Always'),
@JsonValue(r'Never')
never(r'Never'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const DepositRuleModeEnum(this.value);

final String value;

@override
String toString() => value;
}


