//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_session_sms_credits.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageSessionSmsCredits {
  /// Returns a new [ManageSessionSmsCredits] instance.
  ManageSessionSmsCredits({

    required  this.balance,

    required  this.isLow,
  });

      /// SMS kontörü bakiyesi
          // minimum: 0
  @JsonKey(
    
    name: r'balance',
    required: true,
    includeIfNull: false,
  )


  final int balance;



      /// Bakiye LowCreditThreshold'un altında
  @JsonKey(
    
    name: r'isLow',
    required: true,
    includeIfNull: false,
  )


  final bool isLow;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageSessionSmsCredits &&
      other.balance == balance &&
      other.isLow == isLow;

    @override
    int get hashCode =>
        balance.hashCode +
        isLow.hashCode;

  factory ManageSessionSmsCredits.fromJson(Map<String, dynamic> json) => _$ManageSessionSmsCreditsFromJson(json);

  Map<String, dynamic> toJson() => _$ManageSessionSmsCreditsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

