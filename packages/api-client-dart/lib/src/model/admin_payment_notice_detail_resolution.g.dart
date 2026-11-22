// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_detail_resolution.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticeDetailResolutionCWProxy {
  AdminPaymentNoticeDetailResolution resolvedUtc(DateTime resolvedUtc);

  AdminPaymentNoticeDetailResolution resolvedByName(String resolvedByName);

  AdminPaymentNoticeDetailResolution matchedBankTransactionId(
    String? matchedBankTransactionId,
  );

  AdminPaymentNoticeDetailResolution message(String? message);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetailResolution(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetailResolution(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetailResolution call({
    DateTime resolvedUtc,
    String resolvedByName,
    String? matchedBankTransactionId,
    String? message,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticeDetailResolution.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticeDetailResolution.copyWith.fieldName(...)`
class _$AdminPaymentNoticeDetailResolutionCWProxyImpl
    implements _$AdminPaymentNoticeDetailResolutionCWProxy {
  const _$AdminPaymentNoticeDetailResolutionCWProxyImpl(this._value);

  final AdminPaymentNoticeDetailResolution _value;

  @override
  AdminPaymentNoticeDetailResolution resolvedUtc(DateTime resolvedUtc) =>
      this(resolvedUtc: resolvedUtc);

  @override
  AdminPaymentNoticeDetailResolution resolvedByName(String resolvedByName) =>
      this(resolvedByName: resolvedByName);

  @override
  AdminPaymentNoticeDetailResolution matchedBankTransactionId(
    String? matchedBankTransactionId,
  ) => this(matchedBankTransactionId: matchedBankTransactionId);

  @override
  AdminPaymentNoticeDetailResolution message(String? message) =>
      this(message: message);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetailResolution(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetailResolution(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetailResolution call({
    Object? resolvedUtc = const $CopyWithPlaceholder(),
    Object? resolvedByName = const $CopyWithPlaceholder(),
    Object? matchedBankTransactionId = const $CopyWithPlaceholder(),
    Object? message = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticeDetailResolution(
      resolvedUtc: resolvedUtc == const $CopyWithPlaceholder()
          ? _value.resolvedUtc
          // ignore: cast_nullable_to_non_nullable
          : resolvedUtc as DateTime,
      resolvedByName: resolvedByName == const $CopyWithPlaceholder()
          ? _value.resolvedByName
          // ignore: cast_nullable_to_non_nullable
          : resolvedByName as String,
      matchedBankTransactionId:
          matchedBankTransactionId == const $CopyWithPlaceholder()
          ? _value.matchedBankTransactionId
          // ignore: cast_nullable_to_non_nullable
          : matchedBankTransactionId as String?,
      message: message == const $CopyWithPlaceholder()
          ? _value.message
          // ignore: cast_nullable_to_non_nullable
          : message as String?,
    );
  }
}

extension $AdminPaymentNoticeDetailResolutionCopyWith
    on AdminPaymentNoticeDetailResolution {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticeDetailResolution.copyWith(...)` or like so:`instanceOfAdminPaymentNoticeDetailResolution.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticeDetailResolutionCWProxy get copyWith =>
      _$AdminPaymentNoticeDetailResolutionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticeDetailResolution _$AdminPaymentNoticeDetailResolutionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminPaymentNoticeDetailResolution', json, (
  $checkedConvert,
) {
  $checkKeys(
    json,
    requiredKeys: const [
      'resolvedUtc',
      'resolvedByName',
      'matchedBankTransactionId',
      'message',
    ],
  );
  final val = AdminPaymentNoticeDetailResolution(
    resolvedUtc: $checkedConvert(
      'resolvedUtc',
      (v) => DateTime.parse(v as String),
    ),
    resolvedByName: $checkedConvert('resolvedByName', (v) => v as String),
    matchedBankTransactionId: $checkedConvert(
      'matchedBankTransactionId',
      (v) => v as String?,
    ),
    message: $checkedConvert('message', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminPaymentNoticeDetailResolutionToJson(
  AdminPaymentNoticeDetailResolution instance,
) => <String, dynamic>{
  'resolvedUtc': instance.resolvedUtc.toIso8601String(),
  'resolvedByName': instance.resolvedByName,
  'matchedBankTransactionId': instance.matchedBankTransactionId,
  'message': instance.message,
};
