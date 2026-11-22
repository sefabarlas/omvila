//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_detail_credits.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandDetailCredits {
  /// Returns a new [AdminBrandDetailCredits] instance.
  AdminBrandDetailCredits({

    required  this.balance,

    required  this.estimatedDaysLeft,
  });

      /// Bakiye
          // minimum: 0
  @JsonKey(
    
    name: r'balance',
    required: true,
    includeIfNull: false,
  )


  final int balance;



      /// Son 30 günün tüketimine göre; hesaplanamazsa null
          // minimum: 0
  @JsonKey(
    
    name: r'estimatedDaysLeft',
    required: true,
    includeIfNull: true,
  )


  final int? estimatedDaysLeft;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandDetailCredits &&
      other.balance == balance &&
      other.estimatedDaysLeft == estimatedDaysLeft;

    @override
    int get hashCode =>
        balance.hashCode +
        (estimatedDaysLeft == null ? 0 : estimatedDaysLeft.hashCode);

  factory AdminBrandDetailCredits.fromJson(Map<String, dynamic> json) => _$AdminBrandDetailCreditsFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandDetailCreditsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

