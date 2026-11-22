//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_account_deletion_confirm.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAccountDeletionConfirm {
  /// Returns a new [MeAccountDeletionConfirm] instance.
  MeAccountDeletionConfirm({

    required  this.otpId,

    required  this.code,
  });

  @JsonKey(
    
    name: r'otpId',
    required: true,
    includeIfNull: false,
  )


  final String otpId;



  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeAccountDeletionConfirm &&
      other.otpId == otpId &&
      other.code == code;

    @override
    int get hashCode =>
        otpId.hashCode +
        code.hashCode;

  factory MeAccountDeletionConfirm.fromJson(Map<String, dynamic> json) => _$MeAccountDeletionConfirmFromJson(json);

  Map<String, dynamic> toJson() => _$MeAccountDeletionConfirmToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

