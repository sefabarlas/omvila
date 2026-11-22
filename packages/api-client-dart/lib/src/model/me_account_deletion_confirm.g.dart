// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_account_deletion_confirm.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAccountDeletionConfirmCWProxy {
  MeAccountDeletionConfirm otpId(String otpId);

  MeAccountDeletionConfirm code(String code);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAccountDeletionConfirm(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAccountDeletionConfirm(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAccountDeletionConfirm call({String otpId, String code});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAccountDeletionConfirm.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAccountDeletionConfirm.copyWith.fieldName(...)`
class _$MeAccountDeletionConfirmCWProxyImpl
    implements _$MeAccountDeletionConfirmCWProxy {
  const _$MeAccountDeletionConfirmCWProxyImpl(this._value);

  final MeAccountDeletionConfirm _value;

  @override
  MeAccountDeletionConfirm otpId(String otpId) => this(otpId: otpId);

  @override
  MeAccountDeletionConfirm code(String code) => this(code: code);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAccountDeletionConfirm(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAccountDeletionConfirm(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAccountDeletionConfirm call({
    Object? otpId = const $CopyWithPlaceholder(),
    Object? code = const $CopyWithPlaceholder(),
  }) {
    return MeAccountDeletionConfirm(
      otpId: otpId == const $CopyWithPlaceholder()
          ? _value.otpId
          // ignore: cast_nullable_to_non_nullable
          : otpId as String,
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
    );
  }
}

extension $MeAccountDeletionConfirmCopyWith on MeAccountDeletionConfirm {
  /// Returns a callable class that can be used as follows: `instanceOfMeAccountDeletionConfirm.copyWith(...)` or like so:`instanceOfMeAccountDeletionConfirm.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAccountDeletionConfirmCWProxy get copyWith =>
      _$MeAccountDeletionConfirmCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAccountDeletionConfirm _$MeAccountDeletionConfirmFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('MeAccountDeletionConfirm', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['otpId', 'code']);
  final val = MeAccountDeletionConfirm(
    otpId: $checkedConvert('otpId', (v) => v as String),
    code: $checkedConvert('code', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$MeAccountDeletionConfirmToJson(
  MeAccountDeletionConfirm instance,
) => <String, dynamic>{'otpId': instance.otpId, 'code': instance.code};
