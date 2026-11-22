// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_sms_sender_decision.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminSmsSenderDecisionCWProxy {
  AdminSmsSenderDecision senderId(String senderId);

  AdminSmsSenderDecision brandId(String brandId);

  AdminSmsSenderDecision name(String name);

  AdminSmsSenderDecision status(SmsSenderStatus status);

  AdminSmsSenderDecision decidedUtc(DateTime decidedUtc);

  AdminSmsSenderDecision rejectionReason(String? rejectionReason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSmsSenderDecision(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSmsSenderDecision(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSmsSenderDecision call({
    String senderId,
    String brandId,
    String name,
    SmsSenderStatus status,
    DateTime decidedUtc,
    String? rejectionReason,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminSmsSenderDecision.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminSmsSenderDecision.copyWith.fieldName(...)`
class _$AdminSmsSenderDecisionCWProxyImpl
    implements _$AdminSmsSenderDecisionCWProxy {
  const _$AdminSmsSenderDecisionCWProxyImpl(this._value);

  final AdminSmsSenderDecision _value;

  @override
  AdminSmsSenderDecision senderId(String senderId) => this(senderId: senderId);

  @override
  AdminSmsSenderDecision brandId(String brandId) => this(brandId: brandId);

  @override
  AdminSmsSenderDecision name(String name) => this(name: name);

  @override
  AdminSmsSenderDecision status(SmsSenderStatus status) => this(status: status);

  @override
  AdminSmsSenderDecision decidedUtc(DateTime decidedUtc) =>
      this(decidedUtc: decidedUtc);

  @override
  AdminSmsSenderDecision rejectionReason(String? rejectionReason) =>
      this(rejectionReason: rejectionReason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminSmsSenderDecision(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminSmsSenderDecision(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminSmsSenderDecision call({
    Object? senderId = const $CopyWithPlaceholder(),
    Object? brandId = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? decidedUtc = const $CopyWithPlaceholder(),
    Object? rejectionReason = const $CopyWithPlaceholder(),
  }) {
    return AdminSmsSenderDecision(
      senderId: senderId == const $CopyWithPlaceholder()
          ? _value.senderId
          // ignore: cast_nullable_to_non_nullable
          : senderId as String,
      brandId: brandId == const $CopyWithPlaceholder()
          ? _value.brandId
          // ignore: cast_nullable_to_non_nullable
          : brandId as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SmsSenderStatus,
      decidedUtc: decidedUtc == const $CopyWithPlaceholder()
          ? _value.decidedUtc
          // ignore: cast_nullable_to_non_nullable
          : decidedUtc as DateTime,
      rejectionReason: rejectionReason == const $CopyWithPlaceholder()
          ? _value.rejectionReason
          // ignore: cast_nullable_to_non_nullable
          : rejectionReason as String?,
    );
  }
}

extension $AdminSmsSenderDecisionCopyWith on AdminSmsSenderDecision {
  /// Returns a callable class that can be used as follows: `instanceOfAdminSmsSenderDecision.copyWith(...)` or like so:`instanceOfAdminSmsSenderDecision.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminSmsSenderDecisionCWProxy get copyWith =>
      _$AdminSmsSenderDecisionCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminSmsSenderDecision _$AdminSmsSenderDecisionFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminSmsSenderDecision', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'senderId',
      'brandId',
      'name',
      'status',
      'decidedUtc',
      'rejectionReason',
    ],
  );
  final val = AdminSmsSenderDecision(
    senderId: $checkedConvert('senderId', (v) => v as String),
    brandId: $checkedConvert('brandId', (v) => v as String),
    name: $checkedConvert('name', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$SmsSenderStatusEnumMap,
        v,
        unknownValue: SmsSenderStatus.unknownDefaultOpenApi,
      ),
    ),
    decidedUtc: $checkedConvert(
      'decidedUtc',
      (v) => DateTime.parse(v as String),
    ),
    rejectionReason: $checkedConvert('rejectionReason', (v) => v as String?),
  );
  return val;
});

Map<String, dynamic> _$AdminSmsSenderDecisionToJson(
  AdminSmsSenderDecision instance,
) => <String, dynamic>{
  'senderId': instance.senderId,
  'brandId': instance.brandId,
  'name': instance.name,
  'status': _$SmsSenderStatusEnumMap[instance.status]!,
  'decidedUtc': instance.decidedUtc.toIso8601String(),
  'rejectionReason': instance.rejectionReason,
};

const _$SmsSenderStatusEnumMap = {
  SmsSenderStatus.none: 'None',
  SmsSenderStatus.pending: 'Pending',
  SmsSenderStatus.approved: 'Approved',
  SmsSenderStatus.rejected: 'Rejected',
  SmsSenderStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
