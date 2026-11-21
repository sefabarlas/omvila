//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/package_redemption_request.dart';
import 'package:omvila_api/src/model/completion_payment.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_complete_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageCompleteRequest {
  /// Returns a new [ManageCompleteRequest] instance.
  ManageCompleteRequest({

    required  this.payments,

     this.packageRedemptions,
  });

  @JsonKey(
    
    name: r'payments',
    required: true,
    includeIfNull: false,
  )


  final List<CompletionPayment> payments;



  @JsonKey(
    
    name: r'packageRedemptions',
    required: false,
    includeIfNull: false,
  )


  final List<PackageRedemptionRequest>? packageRedemptions;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageCompleteRequest &&
      other.payments == payments &&
      other.packageRedemptions == packageRedemptions;

    @override
    int get hashCode =>
        payments.hashCode +
        packageRedemptions.hashCode;

  factory ManageCompleteRequest.fromJson(Map<String, dynamic> json) => _$ManageCompleteRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageCompleteRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

