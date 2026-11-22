// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'audit_log_entry.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AuditLogEntryCWProxy {
  AuditLogEntry id(int id);

  AuditLogEntry occurredUtc(DateTime occurredUtc);

  AuditLogEntry actor(AuditLogEntryActor actor);

  AuditLogEntry action(AuditLogEntryActionEnum action);

  AuditLogEntry entityType(String entityType);

  AuditLogEntry entityId(String? entityId);

  AuditLogEntry brand(AdminBrandRef? brand);

  AuditLogEntry changes(Map<String, Object> changes);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AuditLogEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AuditLogEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  AuditLogEntry call({
    int id,
    DateTime occurredUtc,
    AuditLogEntryActor actor,
    AuditLogEntryActionEnum action,
    String entityType,
    String? entityId,
    AdminBrandRef? brand,
    Map<String, Object> changes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAuditLogEntry.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAuditLogEntry.copyWith.fieldName(...)`
class _$AuditLogEntryCWProxyImpl implements _$AuditLogEntryCWProxy {
  const _$AuditLogEntryCWProxyImpl(this._value);

  final AuditLogEntry _value;

  @override
  AuditLogEntry id(int id) => this(id: id);

  @override
  AuditLogEntry occurredUtc(DateTime occurredUtc) =>
      this(occurredUtc: occurredUtc);

  @override
  AuditLogEntry actor(AuditLogEntryActor actor) => this(actor: actor);

  @override
  AuditLogEntry action(AuditLogEntryActionEnum action) => this(action: action);

  @override
  AuditLogEntry entityType(String entityType) => this(entityType: entityType);

  @override
  AuditLogEntry entityId(String? entityId) => this(entityId: entityId);

  @override
  AuditLogEntry brand(AdminBrandRef? brand) => this(brand: brand);

  @override
  AuditLogEntry changes(Map<String, Object> changes) => this(changes: changes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AuditLogEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AuditLogEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  AuditLogEntry call({
    Object? id = const $CopyWithPlaceholder(),
    Object? occurredUtc = const $CopyWithPlaceholder(),
    Object? actor = const $CopyWithPlaceholder(),
    Object? action = const $CopyWithPlaceholder(),
    Object? entityType = const $CopyWithPlaceholder(),
    Object? entityId = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? changes = const $CopyWithPlaceholder(),
  }) {
    return AuditLogEntry(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as int,
      occurredUtc: occurredUtc == const $CopyWithPlaceholder()
          ? _value.occurredUtc
          // ignore: cast_nullable_to_non_nullable
          : occurredUtc as DateTime,
      actor: actor == const $CopyWithPlaceholder()
          ? _value.actor
          // ignore: cast_nullable_to_non_nullable
          : actor as AuditLogEntryActor,
      action: action == const $CopyWithPlaceholder()
          ? _value.action
          // ignore: cast_nullable_to_non_nullable
          : action as AuditLogEntryActionEnum,
      entityType: entityType == const $CopyWithPlaceholder()
          ? _value.entityType
          // ignore: cast_nullable_to_non_nullable
          : entityType as String,
      entityId: entityId == const $CopyWithPlaceholder()
          ? _value.entityId
          // ignore: cast_nullable_to_non_nullable
          : entityId as String?,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as AdminBrandRef?,
      changes: changes == const $CopyWithPlaceholder()
          ? _value.changes
          // ignore: cast_nullable_to_non_nullable
          : changes as Map<String, Object>,
    );
  }
}

extension $AuditLogEntryCopyWith on AuditLogEntry {
  /// Returns a callable class that can be used as follows: `instanceOfAuditLogEntry.copyWith(...)` or like so:`instanceOfAuditLogEntry.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AuditLogEntryCWProxy get copyWith => _$AuditLogEntryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AuditLogEntry _$AuditLogEntryFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AuditLogEntry', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'occurredUtc',
      'actor',
      'action',
      'entityType',
      'entityId',
      'brand',
      'changes',
    ],
  );
  final val = AuditLogEntry(
    id: $checkedConvert('id', (v) => (v as num).toInt()),
    occurredUtc: $checkedConvert(
      'occurredUtc',
      (v) => DateTime.parse(v as String),
    ),
    actor: $checkedConvert(
      'actor',
      (v) => AuditLogEntryActor.fromJson(v as Map<String, dynamic>),
    ),
    action: $checkedConvert(
      'action',
      (v) => $enumDecode(
        _$AuditLogEntryActionEnumEnumMap,
        v,
        unknownValue: AuditLogEntryActionEnum.unknownDefaultOpenApi,
      ),
    ),
    entityType: $checkedConvert('entityType', (v) => v as String),
    entityId: $checkedConvert('entityId', (v) => v as String?),
    brand: $checkedConvert(
      'brand',
      (v) =>
          v == null ? null : AdminBrandRef.fromJson(v as Map<String, dynamic>),
    ),
    changes: $checkedConvert(
      'changes',
      (v) =>
          (v as Map<String, dynamic>).map((k, e) => MapEntry(k, e as Object)),
    ),
  );
  return val;
});

Map<String, dynamic> _$AuditLogEntryToJson(AuditLogEntry instance) =>
    <String, dynamic>{
      'id': instance.id,
      'occurredUtc': instance.occurredUtc.toIso8601String(),
      'actor': instance.actor.toJson(),
      'action': _$AuditLogEntryActionEnumEnumMap[instance.action]!,
      'entityType': instance.entityType,
      'entityId': instance.entityId,
      'brand': instance.brand?.toJson(),
      'changes': instance.changes,
    };

const _$AuditLogEntryActionEnumEnumMap = {
  AuditLogEntryActionEnum.create: 'Create',
  AuditLogEntryActionEnum.update: 'Update',
  AuditLogEntryActionEnum.delete: 'Delete',
  AuditLogEntryActionEnum.view: 'View',
  AuditLogEntryActionEnum.approve: 'Approve',
  AuditLogEntryActionEnum.match: 'Match',
  AuditLogEntryActionEnum.export_: 'Export',
  AuditLogEntryActionEnum.anonymize: 'Anonymize',
  AuditLogEntryActionEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
