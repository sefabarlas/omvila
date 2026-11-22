// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_match.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticeMatchCWProxy {
  AdminPaymentNoticeMatch bankTransactionId(String bankTransactionId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeMatch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeMatch(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeMatch call({String bankTransactionId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticeMatch.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticeMatch.copyWith.fieldName(...)`
class _$AdminPaymentNoticeMatchCWProxyImpl
    implements _$AdminPaymentNoticeMatchCWProxy {
  const _$AdminPaymentNoticeMatchCWProxyImpl(this._value);

  final AdminPaymentNoticeMatch _value;

  @override
  AdminPaymentNoticeMatch bankTransactionId(String bankTransactionId) =>
      this(bankTransactionId: bankTransactionId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeMatch(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeMatch(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeMatch call({
    Object? bankTransactionId = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticeMatch(
      bankTransactionId: bankTransactionId == const $CopyWithPlaceholder()
          ? _value.bankTransactionId
          // ignore: cast_nullable_to_non_nullable
          : bankTransactionId as String,
    );
  }
}

extension $AdminPaymentNoticeMatchCopyWith on AdminPaymentNoticeMatch {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticeMatch.copyWith(...)` or like so:`instanceOfAdminPaymentNoticeMatch.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticeMatchCWProxy get copyWith =>
      _$AdminPaymentNoticeMatchCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticeMatch _$AdminPaymentNoticeMatchFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPaymentNoticeMatch', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['bankTransactionId']);
  final val = AdminPaymentNoticeMatch(
    bankTransactionId: $checkedConvert('bankTransactionId', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$AdminPaymentNoticeMatchToJson(
  AdminPaymentNoticeMatch instance,
) => <String, dynamic>{'bankTransactionId': instance.bankTransactionId};
