//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'credit_purchase_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CreditPurchaseRequest {
  /// Returns a new [CreditPurchaseRequest] instance.
  CreditPurchaseRequest({

    required  this.creditPackageId,
  });

      /// Seçilen paket
  @JsonKey(
    
    name: r'creditPackageId',
    required: true,
    includeIfNull: false,
  )


  final String creditPackageId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CreditPurchaseRequest &&
      other.creditPackageId == creditPackageId;

    @override
    int get hashCode =>
        creditPackageId.hashCode;

  factory CreditPurchaseRequest.fromJson(Map<String, dynamic> json) => _$CreditPurchaseRequestFromJson(json);

  Map<String, dynamic> toJson() => _$CreditPurchaseRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

