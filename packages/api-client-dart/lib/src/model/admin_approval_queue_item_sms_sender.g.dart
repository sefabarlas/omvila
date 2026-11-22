// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_approval_queue_item_sms_sender.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminApprovalQueueItemSmsSenderCWProxy {
  AdminApprovalQueueItemSmsSender requestedName(String requestedName);

  AdminApprovalQueueItemSmsSender currentEffectiveName(
    String currentEffectiveName,
  );

  AdminApprovalQueueItemSmsSender version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItemSmsSender(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItemSmsSender(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItemSmsSender call({
    String requestedName,
    String currentEffectiveName,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminApprovalQueueItemSmsSender.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminApprovalQueueItemSmsSender.copyWith.fieldName(...)`
class _$AdminApprovalQueueItemSmsSenderCWProxyImpl
    implements _$AdminApprovalQueueItemSmsSenderCWProxy {
  const _$AdminApprovalQueueItemSmsSenderCWProxyImpl(this._value);

  final AdminApprovalQueueItemSmsSender _value;

  @override
  AdminApprovalQueueItemSmsSender requestedName(String requestedName) =>
      this(requestedName: requestedName);

  @override
  AdminApprovalQueueItemSmsSender currentEffectiveName(
    String currentEffectiveName,
  ) => this(currentEffectiveName: currentEffectiveName);

  @override
  AdminApprovalQueueItemSmsSender version(String version) =>
      this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItemSmsSender(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItemSmsSender(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItemSmsSender call({
    Object? requestedName = const $CopyWithPlaceholder(),
    Object? currentEffectiveName = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return AdminApprovalQueueItemSmsSender(
      requestedName: requestedName == const $CopyWithPlaceholder()
          ? _value.requestedName
          // ignore: cast_nullable_to_non_nullable
          : requestedName as String,
      currentEffectiveName: currentEffectiveName == const $CopyWithPlaceholder()
          ? _value.currentEffectiveName
          // ignore: cast_nullable_to_non_nullable
          : currentEffectiveName as String,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $AdminApprovalQueueItemSmsSenderCopyWith
    on AdminApprovalQueueItemSmsSender {
  /// Returns a callable class that can be used as follows: `instanceOfAdminApprovalQueueItemSmsSender.copyWith(...)` or like so:`instanceOfAdminApprovalQueueItemSmsSender.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminApprovalQueueItemSmsSenderCWProxy get copyWith =>
      _$AdminApprovalQueueItemSmsSenderCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminApprovalQueueItemSmsSender _$AdminApprovalQueueItemSmsSenderFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminApprovalQueueItemSmsSender', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['requestedName', 'currentEffectiveName', 'version'],
  );
  final val = AdminApprovalQueueItemSmsSender(
    requestedName: $checkedConvert('requestedName', (v) => v as String),
    currentEffectiveName: $checkedConvert(
      'currentEffectiveName',
      (v) => v as String,
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminApprovalQueueItemSmsSenderToJson(
  AdminApprovalQueueItemSmsSender instance,
) => <String, dynamic>{
  'requestedName': instance.requestedName,
  'currentEffectiveName': instance.currentEffectiveName,
  'version': instance.version,
};
