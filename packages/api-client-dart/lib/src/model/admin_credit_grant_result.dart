//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_credit_transaction.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_grant_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditGrantResult {
  /// Returns a new [AdminCreditGrantResult] instance.
  AdminCreditGrantResult({

    required  this.transaction,

    required  this.balance,
  });

  @JsonKey(
    
    name: r'transaction',
    required: true,
    includeIfNull: false,
  )


  final AdminCreditTransaction transaction;



      /// Güncel bakiye
          // minimum: 0
  @JsonKey(
    
    name: r'balance',
    required: true,
    includeIfNull: false,
  )


  final int balance;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditGrantResult &&
      other.transaction == transaction &&
      other.balance == balance;

    @override
    int get hashCode =>
        transaction.hashCode +
        balance.hashCode;

  factory AdminCreditGrantResult.fromJson(Map<String, dynamic> json) => _$AdminCreditGrantResultFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditGrantResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

