//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/membership_role.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_user.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageUser {
  /// Returns a new [ManageUser] instance.
  ManageUser({

    required  this.userId,

    required  this.fullName,

    required  this.phoneMasked,

    required  this.role,

    required  this.branchIds,

    required  this.status,

    required  this.staffId,

    required  this.isCurrentUser,

    required  this.version,
  });

      /// Kullanıcı
  @JsonKey(
    
    name: r'userId',
    required: true,
    includeIfNull: false,
  )


  final String userId;



      /// Ad
  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// Maskeli telefon
  @JsonKey(
    
    name: r'phoneMasked',
    required: true,
    includeIfNull: false,
  )


  final String phoneMasked;



  @JsonKey(
    
    name: r'role',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MembershipRole.unknownDefaultOpenApi,
  )


  final MembershipRole role;



      /// Şubeler; boş dizi = tüm şubeler (\"Tümü\", Owner)
  @JsonKey(
    
    name: r'branchIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> branchIds;



      /// \"Aktif\", \"Davet gönderildi\", pasif
  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ManageUserStatusEnum.unknownDefaultOpenApi,
  )


  final ManageUserStatusEnum status;



      /// Bağlı personel kaydı; yoksa null
  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: true,
  )


  final String? staffId;



      /// Oturumdaki kullanıcı
  @JsonKey(
    
    name: r'isCurrentUser',
    required: true,
    includeIfNull: false,
  )


  final bool isCurrentUser;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageUser &&
      other.userId == userId &&
      other.fullName == fullName &&
      other.phoneMasked == phoneMasked &&
      other.role == role &&
      other.branchIds == branchIds &&
      other.status == status &&
      other.staffId == staffId &&
      other.isCurrentUser == isCurrentUser &&
      other.version == version;

    @override
    int get hashCode =>
        userId.hashCode +
        fullName.hashCode +
        phoneMasked.hashCode +
        role.hashCode +
        branchIds.hashCode +
        status.hashCode +
        (staffId == null ? 0 : staffId.hashCode) +
        isCurrentUser.hashCode +
        version.hashCode;

  factory ManageUser.fromJson(Map<String, dynamic> json) => _$ManageUserFromJson(json);

  Map<String, dynamic> toJson() => _$ManageUserToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// \"Aktif\", \"Davet gönderildi\", pasif
enum ManageUserStatusEnum {
@JsonValue(r'Invited')
invited(r'Invited'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Disabled')
disabled(r'Disabled'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ManageUserStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


