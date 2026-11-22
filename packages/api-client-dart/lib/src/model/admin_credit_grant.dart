//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_grant.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditGrant {
  /// Returns a new [AdminCreditGrant] instance.
  AdminCreditGrant({

    required  this.credits,

    required  this.reason,
  });

      /// Eklenecek kontör (eksi = düzeltme; 0 olamaz)
          // minimum: -100000
          // maximum: 100000
  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: false,
  )


  final int credits;



      /// Neden (ör. \"Destek talebi #4821\"); denetime yazılır
  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  )


  final String reason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditGrant &&
      other.credits == credits &&
      other.reason == reason;

    @override
    int get hashCode =>
        credits.hashCode +
        reason.hashCode;

  factory AdminCreditGrant.fromJson(Map<String, dynamic> json) => _$AdminCreditGrantFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditGrantToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

