//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/platform_role.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'audit_log_entry_actor.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AuditLogEntryActor {
  /// Returns a new [AuditLogEntryActor] instance.
  AuditLogEntryActor({

    required  this.type,

    required  this.id,

    required  this.name,

    required  this.platformRole,
  });

      /// İşlemi yapan türü (\"Sistem · Otomatik\")
  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AuditLogEntryActorTypeEnum.unknownDefaultOpenApi,
  )


  final AuditLogEntryActorTypeEnum type;



      /// İşlemi yapan; System'de null
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: true,
  )


  final String? id;



      /// Ad (müşteride maskeli); System'de null
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: true,
  )


  final String? name;



      /// Platform kullanıcısında rol (\"Destek\", \"Yönetici\"); diğerlerinde null
  @JsonKey(
    
    name: r'platformRole',
    required: true,
    includeIfNull: true,
  unknownEnumValue: PlatformRole.unknownDefaultOpenApi,
  )


  final PlatformRole? platformRole;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AuditLogEntryActor &&
      other.type == type &&
      other.id == id &&
      other.name == name &&
      other.platformRole == platformRole;

    @override
    int get hashCode =>
        type.hashCode +
        (id == null ? 0 : id.hashCode) +
        (name == null ? 0 : name.hashCode) +
        (platformRole == null ? 0 : platformRole.hashCode);

  factory AuditLogEntryActor.fromJson(Map<String, dynamic> json) => _$AuditLogEntryActorFromJson(json);

  Map<String, dynamic> toJson() => _$AuditLogEntryActorToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// İşlemi yapan türü (\"Sistem · Otomatik\")
enum AuditLogEntryActorTypeEnum {
@JsonValue(r'User')
user(r'User'),
@JsonValue(r'PlatformUser')
platformUser(r'PlatformUser'),
@JsonValue(r'Customer')
customer(r'Customer'),
@JsonValue(r'System')
system(r'System'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AuditLogEntryActorTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


