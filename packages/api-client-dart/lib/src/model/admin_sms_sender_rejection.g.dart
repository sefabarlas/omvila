// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_sms_sender_rejection.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSmsSenderRejectionCWProxy {
  AdminSmsSenderRejection reason(String reason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSmsSenderRejection(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSmsSenderRejection(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSmsSenderRejection call({String reason});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSmsSenderRejection.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSmsSenderRejection.copyWith.fieldName(...)`
class _$AdminSmsSenderRejectionCWProxyImpl
    implements _$AdminSmsSenderRejectionCWProxy {
  const _$AdminSmsSenderRejectionCWProxyImpl(this._value);

  final AdminSmsSenderRejection _value;

  @override
  AdminSmsSenderRejection reason(String reason) => this(reason: reason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSmsSenderRejection(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSmsSenderRejection(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSmsSenderRejection call({
    Object? reason = const $CopyWithPlaceholder(),
  }) {
    return AdminSmsSenderRejection(
      reason: reason == const $CopyWithPlaceholder()
          ? _value.reason
          // ignore: cast_nullable_to_non_nullable
          : reason as String,
    );
  }
}

extension $AdminSmsSenderRejectionCopyWith on AdminSmsSenderRejection {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSmsSenderRejection.copyWith(...)` or like so:`instanceOfAdminSmsSenderRejection.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSmsSenderRejectionCWProxy get copyWith =>
      _$AdminSmsSenderRejectionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSmsSenderRejection _$AdminSmsSenderRejectionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminSmsSenderRejection', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['reason']);
  final val = AdminSmsSenderRejection(
    reason: $checkedConvert('reason', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminSmsSenderRejectionToJson(
  AdminSmsSenderRejection instance,
) => <String, dynamic>{'reason': instance.reason};
