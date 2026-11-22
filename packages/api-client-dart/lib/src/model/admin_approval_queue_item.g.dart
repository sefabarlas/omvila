// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_approval_queue_item.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminApprovalQueueItemCWProxy {
  AdminApprovalQueueItem type(AdminApprovalQueueItemTypeEnum type);

  AdminApprovalQueueItem id(String id);

  AdminApprovalQueueItem brandId(String brandId);

  AdminApprovalQueueItem brandName(String brandName);

  AdminApprovalQueueItem submittedUtc(DateTime submittedUtc);

  AdminApprovalQueueItem paymentNotice(
    AdminApprovalQueueItemPaymentNotice? paymentNotice,
  );

  AdminApprovalQueueItem smsSender(AdminApprovalQueueItemSmsSender? smsSender);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItem(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItem call({
    AdminApprovalQueueItemTypeEnum type,
    String id,
    String brandId,
    String brandName,
    DateTime submittedUtc,
    AdminApprovalQueueItemPaymentNotice? paymentNotice,
    AdminApprovalQueueItemSmsSender? smsSender,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminApprovalQueueItem.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminApprovalQueueItem.copyWith.fieldName(...)`
class _$AdminApprovalQueueItemCWProxyImpl
    implements _$AdminApprovalQueueItemCWProxy {
  const _$AdminApprovalQueueItemCWProxyImpl(this._value);

  final AdminApprovalQueueItem _value;

  @override
  AdminApprovalQueueItem type(AdminApprovalQueueItemTypeEnum type) =>
      this(type: type);

  @override
  AdminApprovalQueueItem id(String id) => this(id: id);

  @override
  AdminApprovalQueueItem brandId(String brandId) => this(brandId: brandId);

  @override
  AdminApprovalQueueItem brandName(String brandName) =>
      this(brandName: brandName);

  @override
  AdminApprovalQueueItem submittedUtc(DateTime submittedUtc) =>
      this(submittedUtc: submittedUtc);

  @override
  AdminApprovalQueueItem paymentNotice(
    AdminApprovalQueueItemPaymentNotice? paymentNotice,
  ) => this(paymentNotice: paymentNotice);

  @override
  AdminApprovalQueueItem smsSender(
    AdminApprovalQueueItemSmsSender? smsSender,
  ) => this(smsSender: smsSender);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItem(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItem(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItem call({
    Object? type = const $CopyWithPlaceholder(),
    Object? id = const $CopyWithPlaceholder(),
    Object? brandId = const $CopyWithPlaceholder(),
    Object? brandName = const $CopyWithPlaceholder(),
    Object? submittedUtc = const $CopyWithPlaceholder(),
    Object? paymentNotice = const $CopyWithPlaceholder(),
    Object? smsSender = const $CopyWithPlaceholder(),
  }) {
    return AdminApprovalQueueItem(
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as AdminApprovalQueueItemTypeEnum,
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      brandId: brandId == const $CopyWithPlaceholder()
          ? _value.brandId
          // ignore: cast_nullable_to_non_nullable
          : brandId as String,
      brandName: brandName == const $CopyWithPlaceholder()
          ? _value.brandName
          // ignore: cast_nullable_to_non_nullable
          : brandName as String,
      submittedUtc: submittedUtc == const $CopyWithPlaceholder()
          ? _value.submittedUtc
          // ignore: cast_nullable_to_non_nullable
          : submittedUtc as DateTime,
      paymentNotice: paymentNotice == const $CopyWithPlaceholder()
          ? _value.paymentNotice
          // ignore: cast_nullable_to_non_nullable
          : paymentNotice as AdminApprovalQueueItemPaymentNotice?,
      smsSender: smsSender == const $CopyWithPlaceholder()
          ? _value.smsSender
          // ignore: cast_nullable_to_non_nullable
          : smsSender as AdminApprovalQueueItemSmsSender?,
    );
  }
}

extension $AdminApprovalQueueItemCopyWith on AdminApprovalQueueItem {
  /// Returns a callable class that can be used as follows: `instanceOfAdminApprovalQueueItem.copyWith(...)` or like so:`instanceOfAdminApprovalQueueItem.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminApprovalQueueItemCWProxy get copyWith =>
      _$AdminApprovalQueueItemCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminApprovalQueueItem _$AdminApprovalQueueItemFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminApprovalQueueItem', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'type',
      'id',
      'brandId',
      'brandName',
      'submittedUtc',
      'paymentNotice',
      'smsSender',
    ],
  );
  final val = AdminApprovalQueueItem(
    type: $checkedConvert(
      'type',
      (v) => $enumDecode(
        _$AdminApprovalQueueItemTypeEnumEnumMap,
        v,
        unknownValue: AdminApprovalQueueItemTypeEnum.unknownDefaultOpenApi,
      ),
    ),
    id: $checkedConvert('id', (v) => v as String),
    brandId: $checkedConvert('brandId', (v) => v as String),
    brandName: $checkedConvert('brandName', (v) => v as String),
    submittedUtc: $checkedConvert(
      'submittedUtc',
      (v) => DateTime.parse(v as String),
    ),
    paymentNotice: $checkedConvert(
      'paymentNotice',
      (v) => v == null
          ? null
          : AdminApprovalQueueItemPaymentNotice.fromJson(
              v as Map<String, dynamic>,
            ),
    ),
    smsSender: $checkedConvert(
      'smsSender',
      (v) => v == null
          ? null
          : AdminApprovalQueueItemSmsSender.fromJson(v as Map<String, dynamic>),
    ),
  );
  return val;
});

Map<String, dynamic> _$AdminApprovalQueueItemToJson(
  AdminApprovalQueueItem instance,
) => <String, dynamic>{
  'type': _$AdminApprovalQueueItemTypeEnumEnumMap[instance.type]!,
  'id': instance.id,
  'brandId': instance.brandId,
  'brandName': instance.brandName,
  'submittedUtc': instance.submittedUtc.toIso8601String(),
  'paymentNotice': instance.paymentNotice?.toJson(),
  'smsSender': instance.smsSender?.toJson(),
};

const _$AdminApprovalQueueItemTypeEnumEnumMap = {
  AdminApprovalQueueItemTypeEnum.paymentNotice: 'PaymentNotice',
  AdminApprovalQueueItemTypeEnum.smsSender: 'SmsSender',
  AdminApprovalQueueItemTypeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
