//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_session_membership.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageSessionMembership {
  /// Returns a new [ManageSessionMembership] instance.
  ManageSessionMembership({

    required  this.role,

    required  this.branchIds,

     this.staffId,
  });

  @JsonKey(
    
    name: r'role',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageSessionMembershipRoleEnum.unknownDefaultOpenApi,
  )


  final ManageSessionMembershipRoleEnum role;



      /// Boş dizi = tüm şubeler (Owner)
  @JsonKey(
    
    name: r'branchIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> branchIds;



  @JsonKey(
    
    name: r'staffId',
    required: false,
    includeIfNull: false,
  )


  final String? staffId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageSessionMembership &&
      other.role == role &&
      other.branchIds == branchIds &&
      other.staffId == staffId;

    @override
    int get hashCode =>
        role.hashCode +
        branchIds.hashCode +
        (staffId == null ? 0 : staffId.hashCode);

  factory ManageSessionMembership.fromJson(Map<String, dynamic> json) => _$ManageSessionMembershipFromJson(json);

  Map<String, dynamic> toJson() => _$ManageSessionMembershipToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ManageSessionMembershipRoleEnum {
@JsonValue(r'Owner')
owner(r'Owner'),
@JsonValue(r'BranchManager')
branchManager(r'BranchManager'),
@JsonValue(r'Staff')
staff(r'Staff'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageSessionMembershipRoleEnum(this.value);

final String value;

@override
String toString() => value;
}


