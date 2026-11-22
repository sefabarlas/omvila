// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'audit_log_entry_actor.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AuditLogEntryActorCWProxy {
  AuditLogEntryActor type(AuditLogEntryActorTypeEnum type);

  AuditLogEntryActor id(String? id);

  AuditLogEntryActor name(String? name);

  AuditLogEntryActor platformRole(PlatformRole? platformRole);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AuditLogEntryActor(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AuditLogEntryActor(...).copyWith(id: 12, name: "My name")
  /// ````
  AuditLogEntryActor call({
    AuditLogEntryActorTypeEnum type,
    String? id,
    String? name,
    PlatformRole? platformRole,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAuditLogEntryActor.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAuditLogEntryActor.copyWith.fieldName(...)`
class _$AuditLogEntryActorCWProxyImpl implements _$AuditLogEntryActorCWProxy {
  const _$AuditLogEntryActorCWProxyImpl(this._value);

  final AuditLogEntryActor _value;

  @override
  AuditLogEntryActor type(AuditLogEntryActorTypeEnum type) => this(type: type);

  @override
  AuditLogEntryActor id(String? id) => this(id: id);

  @override
  AuditLogEntryActor name(String? name) => this(name: name);

  @override
  AuditLogEntryActor platformRole(PlatformRole? platformRole) =>
      this(platformRole: platformRole);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AuditLogEntryActor(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AuditLogEntryActor(...).copyWith(id: 12, name: "My name")
  /// ````
  AuditLogEntryActor call({
    Object? type = const $CopyWithPlaceholder(),
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? platformRole = const $CopyWithPlaceholder(),
  }) {
    return AuditLogEntryActor(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as AuditLogEntryActorTypeEnum,
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String?,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String?,
      platformRole: platformRole == const $CopyWithPlaceholder()
          ? _value.platformRole
          // ignore: cast_nullable_to_non_nullable
          : platformRole as PlatformRole?,
    );
  }
}

extension $AuditLogEntryActorCopyWith on AuditLogEntryActor {
  /// Returns a callable class that can be used as follows: `instanceOfAuditLogEntryActor.copyWith(...)` or like so:`instanceOfAuditLogEntryActor.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AuditLogEntryActorCWProxy get copyWith =>
      _$AuditLogEntryActorCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AuditLogEntryActor _$AuditLogEntryActorFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AuditLogEntryActor', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['type', 'id', 'name', 'platformRole'],
      );
      final val = AuditLogEntryActor(
        type: $checkedConvert(
          'type',
          (v) => $enumDecode(
            _$AuditLogEntryActorTypeEnumEnumMap,
            v,
            unknownValue: AuditLogEntryActorTypeEnum.unknownDefaultOpenApi,
          ),
        ),
        id: $checkedConvert('id', (v) => v as String?),
        name: $checkedConvert('name', (v) => v as String?),
        platformRole: $checkedConvert(
          'platformRole',
          (v) => $enumDecodeNullable(
            _$PlatformRoleEnumMap,
            v,
            unknownValue: PlatformRole.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AuditLogEntryActorToJson(AuditLogEntryActor instance) =>
    <String, dynamic>{
      'type': _$AuditLogEntryActorTypeEnumEnumMap[instance.type]!,
      'id': instance.id,
      'name': instance.name,
      'platformRole': _$PlatformRoleEnumMap[instance.platformRole],
    };

const _$AuditLogEntryActorTypeEnumEnumMap = {
  AuditLogEntryActorTypeEnum.user: 'User',
  AuditLogEntryActorTypeEnum.platformUser: 'PlatformUser',
  AuditLogEntryActorTypeEnum.customer: 'Customer',
  AuditLogEntryActorTypeEnum.system: 'System',
  AuditLogEntryActorTypeEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$PlatformRoleEnumMap = {
  PlatformRole.platformAdmin: 'PlatformAdmin',
  PlatformRole.platformSupport: 'PlatformSupport',
  PlatformRole.platformFinance: 'PlatformFinance',
  PlatformRole.unknownDefaultOpenApi: 'unknown_default_open_api',
};
