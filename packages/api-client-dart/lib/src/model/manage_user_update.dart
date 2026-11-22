//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/membership_role.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_user_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageUserUpdate {
  /// Returns a new [ManageUserUpdate] instance.
  ManageUserUpdate({

    required  this.role,

    required  this.branchIds,
  });

  @JsonKey(
    
    name: r'role',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MembershipRole.unknownDefaultOpenApi,
  )


  final MembershipRole role;



      /// Owner'da yok sayılır; diğer rollerde en az bir şube
  @JsonKey(
    
    name: r'branchIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> branchIds;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageUserUpdate &&
      other.role == role &&
      other.branchIds == branchIds;

    @override
    int get hashCode =>
        role.hashCode +
        branchIds.hashCode;

  factory ManageUserUpdate.fromJson(Map<String, dynamic> json) => _$ManageUserUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$ManageUserUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

