//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_permission.dart';
import 'package:omvila_api/src/model/admin_session_user.dart';
import 'package:omvila_api/src/model/platform_role.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_session.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSession {
  /// Returns a new [AdminSession] instance.
  AdminSession({

    required  this.user,

    required  this.role,

    required  this.permissions,

    required  this.pendingApprovalCount,
  });

  @JsonKey(
    
    name: r'user',
    required: true,
    includeIfNull: false,
  )


  final AdminSessionUser user;



  @JsonKey(
    
    name: r'role',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PlatformRole.unknownDefaultOpenApi,
  )


  final PlatformRole role;



      /// Rolün izinleri
  @JsonKey(
    
    name: r'permissions',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminPermission.unknownDefaultOpenApi,
  )


  final List<AdminPermission> permissions;



      /// Menüdeki \"Onay kuyrukları\" rozeti (kullanıcının görebildiği bekleyenler)
          // minimum: 0
  @JsonKey(
    
    name: r'pendingApprovalCount',
    required: true,
    includeIfNull: false,
  )


  final int pendingApprovalCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSession &&
      other.user == user &&
      other.role == role &&
      other.permissions == permissions &&
      other.pendingApprovalCount == pendingApprovalCount;

    @override
    int get hashCode =>
        user.hashCode +
        role.hashCode +
        permissions.hashCode +
        pendingApprovalCount.hashCode;

  factory AdminSession.fromJson(Map<String, dynamic> json) => _$AdminSessionFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSessionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

