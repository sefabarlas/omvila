// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sms_sender.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SmsSenderCWProxy {
  SmsSender name(String? name);

  SmsSender status(SmsSenderStatus status);

  SmsSender effectiveName(String effectiveName);

  SmsSender submittedUtc(DateTime? submittedUtc);

  SmsSender decidedUtc(DateTime? decidedUtc);

  SmsSender rejectionReason(String? rejectionReason);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsSender(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsSender(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsSender call({
    String? name,
    SmsSenderStatus status,
    String effectiveName,
    DateTime? submittedUtc,
    DateTime? decidedUtc,
    String? rejectionReason,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSmsSender.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSmsSender.copyWith.fieldName(...)`
class _$SmsSenderCWProxyImpl implements _$SmsSenderCWProxy {
  const _$SmsSenderCWProxyImpl(this._value);

  final SmsSender _value;

  @override
  SmsSender name(String? name) => this(name: name);

  @override
  SmsSender status(SmsSenderStatus status) => this(status: status);

  @override
  SmsSender effectiveName(String effectiveName) =>
      this(effectiveName: effectiveName);

  @override
  SmsSender submittedUtc(DateTime? submittedUtc) =>
      this(submittedUtc: submittedUtc);

  @override
  SmsSender decidedUtc(DateTime? decidedUtc) => this(decidedUtc: decidedUtc);

  @override
  SmsSender rejectionReason(String? rejectionReason) =>
      this(rejectionReason: rejectionReason);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsSender(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsSender(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsSender call({
    Object? name = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? effectiveName = const $CopyWithPlaceholder(),
    Object? submittedUtc = const $CopyWithPlaceholder(),
    Object? decidedUtc = const $CopyWithPlaceholder(),
    Object? rejectionReason = const $CopyWithPlaceholder(),
  }) {
    return SmsSender(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String?,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SmsSenderStatus,
      effectiveName: effectiveName == const $CopyWithPlaceholder()
          ? _value.effectiveName
          // ignore: cast_nullable_to_non_nullable
          : effectiveName as String,
      submittedUtc: submittedUtc == const $CopyWithPlaceholder()
          ? _value.submittedUtc
          // ignore: cast_nullable_to_non_nullable
          : submittedUtc as DateTime?,
      decidedUtc: decidedUtc == const $CopyWithPlaceholder()
          ? _value.decidedUtc
          // ignore: cast_nullable_to_non_nullable
          : decidedUtc as DateTime?,
      rejectionReason: rejectionReason == const $CopyWithPlaceholder()
          ? _value.rejectionReason
          // ignore: cast_nullable_to_non_nullable
          : rejectionReason as String?,
    );
  }
}

extension $SmsSenderCopyWith on SmsSender {
  /// Returns a callable class that can be used as follows: `instanceOfSmsSender.copyWith(...)` or like so:`instanceOfSmsSender.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SmsSenderCWProxy get copyWith => _$SmsSenderCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SmsSender _$SmsSenderFromJson(Map<String, dynamic> json) => $checkedCreate(
  'SmsSender',
  json,
  ($checkedConvert) {
    $checkKeys(json, requiredKeys: const ['name', 'status', 'effectiveName']);
    final val = SmsSender(
      name: $checkedConvert('name', (v) => v as String?),
      status: $checkedConvert(
        'status',
        (v) => $enumDecode(
          _$SmsSenderStatusEnumMap,
          v,
          unknownValue: SmsSenderStatus.unknownDefaultOpenApi,
        ),
      ),
      effectiveName: $checkedConvert('effectiveName', (v) => v as String),
      submittedUtc: $checkedConvert(
        'submittedUtc',
        (v) => v == null ? null : DateTime.parse(v as String),
      ),
      decidedUtc: $checkedConvert(
        'decidedUtc',
        (v) => v == null ? null : DateTime.parse(v as String),
      ),
      rejectionReason: $checkedConvert('rejectionReason', (v) => v as String?),
    );
    return val;
  },
);

Map<String, dynamic> _$SmsSenderToJson(SmsSender instance) => <String, dynamic>{
  'name': instance.name,
  'status': _$SmsSenderStatusEnumMap[instance.status]!,
  'effectiveName': instance.effectiveName,
  'submittedUtc': ?instance.submittedUtc?.toIso8601String(),
  'decidedUtc': ?instance.decidedUtc?.toIso8601String(),
  'rejectionReason': ?instance.rejectionReason,
};

const _$SmsSenderStatusEnumMap = {
  SmsSenderStatus.none: 'None',
  SmsSenderStatus.pending: 'Pending',
  SmsSenderStatus.approved: 'Approved',
  SmsSenderStatus.rejected: 'Rejected',
  SmsSenderStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
