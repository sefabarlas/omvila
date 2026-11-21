//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_approve_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageApproveRequest {
  /// Returns a new [ManageApproveRequest] instance.
  ManageApproveRequest({

     this.waiveDeposit = false,
  });

      /// \"Kaporasız onayla\": kapora istenmiş ve gelmemişken kapora almadan onayla. Kapora bekleniyorken gönderilmezse 409 deposit_required
  @JsonKey(
    defaultValue: false,
    name: r'waiveDeposit',
    required: false,
    includeIfNull: false,
  )


  final bool? waiveDeposit;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageApproveRequest &&
      other.waiveDeposit == waiveDeposit;

    @override
    int get hashCode =>
        waiveDeposit.hashCode;

  factory ManageApproveRequest.fromJson(Map<String, dynamic> json) => _$ManageApproveRequestFromJson(json);

  Map<String, dynamic> toJson() => _$ManageApproveRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

