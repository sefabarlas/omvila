// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_resolution.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticeResolutionCWProxy {
  AdminPaymentNoticeResolution message(String? message);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeResolution(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeResolution(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeResolution call({String? message});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticeResolution.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticeResolution.copyWith.fieldName(...)`
class _$AdminPaymentNoticeResolutionCWProxyImpl
    implements _$AdminPaymentNoticeResolutionCWProxy {
  const _$AdminPaymentNoticeResolutionCWProxyImpl(this._value);

  final AdminPaymentNoticeResolution _value;

  @override
  AdminPaymentNoticeResolution message(String? message) =>
      this(message: message);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeResolution(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeResolution(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeResolution call({
    Object? message = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticeResolution(
      message: message == const $CopyWithPlaceholder()
          ? _value.message
          // ignore: cast_nullable_to_non_nullable
          : message as String?,
    );
  }
}

extension $AdminPaymentNoticeResolutionCopyWith
    on AdminPaymentNoticeResolution {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticeResolution.copyWith(...)` or like so:`instanceOfAdminPaymentNoticeResolution.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticeResolutionCWProxy get copyWith =>
      _$AdminPaymentNoticeResolutionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticeResolution _$AdminPaymentNoticeResolutionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPaymentNoticeResolution', json, ($checkedConvert) {
  final val = AdminPaymentNoticeResolution(
    message: $checkedConvert('message', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminPaymentNoticeResolutionToJson(
  AdminPaymentNoticeResolution instance,
) => <String, dynamic>{'message': ?instance.message};
