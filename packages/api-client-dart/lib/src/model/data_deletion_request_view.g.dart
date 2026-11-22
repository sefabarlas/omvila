// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'data_deletion_request_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DataDeletionRequestViewCWProxy {
  DataDeletionRequestView id(String id);

  DataDeletionRequestView brandCustomerId(String? brandCustomerId);

  DataDeletionRequestView customerDisplayName(String customerDisplayName);

  DataDeletionRequestView requestedVia(
    DataDeletionRequestViewRequestedViaEnum requestedVia,
  );

  DataDeletionRequestView requestedUtc(DateTime requestedUtc);

  DataDeletionRequestView dueOn(DateTime dueOn);

  DataDeletionRequestView status(DataDeletionRequestViewStatusEnum status);

  DataDeletionRequestView completedUtc(DateTime? completedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataDeletionRequestView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataDeletionRequestView(...).copyWith(id: 12, name: "My name")
  /// ````
  DataDeletionRequestView call({
    String id,
    String? brandCustomerId,
    String customerDisplayName,
    DataDeletionRequestViewRequestedViaEnum requestedVia,
    DateTime requestedUtc,
    DateTime dueOn,
    DataDeletionRequestViewStatusEnum status,
    DateTime? completedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDataDeletionRequestView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDataDeletionRequestView.copyWith.fieldName(...)`
class _$DataDeletionRequestViewCWProxyImpl
    implements _$DataDeletionRequestViewCWProxy {
  const _$DataDeletionRequestViewCWProxyImpl(this._value);

  final DataDeletionRequestView _value;

  @override
  DataDeletionRequestView id(String id) => this(id: id);

  @override
  DataDeletionRequestView brandCustomerId(String? brandCustomerId) =>
      this(brandCustomerId: brandCustomerId);

  @override
  DataDeletionRequestView customerDisplayName(String customerDisplayName) =>
      this(customerDisplayName: customerDisplayName);

  @override
  DataDeletionRequestView requestedVia(
    DataDeletionRequestViewRequestedViaEnum requestedVia,
  ) => this(requestedVia: requestedVia);

  @override
  DataDeletionRequestView requestedUtc(DateTime requestedUtc) =>
      this(requestedUtc: requestedUtc);

  @override
  DataDeletionRequestView dueOn(DateTime dueOn) => this(dueOn: dueOn);

  @override
  DataDeletionRequestView status(DataDeletionRequestViewStatusEnum status) =>
      this(status: status);

  @override
  DataDeletionRequestView completedUtc(DateTime? completedUtc) =>
      this(completedUtc: completedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DataDeletionRequestView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DataDeletionRequestView(...).copyWith(id: 12, name: "My name")
  /// ````
  DataDeletionRequestView call({
    Object? id = const $CopyWithPlaceholder(),
    Object? brandCustomerId = const $CopyWithPlaceholder(),
    Object? customerDisplayName = const $CopyWithPlaceholder(),
    Object? requestedVia = const $CopyWithPlaceholder(),
    Object? requestedUtc = const $CopyWithPlaceholder(),
    Object? dueOn = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? completedUtc = const $CopyWithPlaceholder(),
  }) {
    return DataDeletionRequestView(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      brandCustomerId: brandCustomerId == const $CopyWithPlaceholder()
          ? _value.brandCustomerId
          // ignore: cast_nullable_to_non_nullable
          : brandCustomerId as String?,
      customerDisplayName: customerDisplayName == const $CopyWithPlaceholder()
          ? _value.customerDisplayName
          // ignore: cast_nullable_to_non_nullable
          : customerDisplayName as String,
      requestedVia: requestedVia == const $CopyWithPlaceholder()
          ? _value.requestedVia
          // ignore: cast_nullable_to_non_nullable
          : requestedVia as DataDeletionRequestViewRequestedViaEnum,
      requestedUtc: requestedUtc == const $CopyWithPlaceholder()
          ? _value.requestedUtc
          // ignore: cast_nullable_to_non_nullable
          : requestedUtc as DateTime,
      dueOn: dueOn == const $CopyWithPlaceholder()
          ? _value.dueOn
          // ignore: cast_nullable_to_non_nullable
          : dueOn as DateTime,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as DataDeletionRequestViewStatusEnum,
      completedUtc: completedUtc == const $CopyWithPlaceholder()
          ? _value.completedUtc
          // ignore: cast_nullable_to_non_nullable
          : completedUtc as DateTime?,
    );
  }
}

extension $DataDeletionRequestViewCopyWith on DataDeletionRequestView {
  /// Returns a callable class that can be used as follows: `instanceOfDataDeletionRequestView.copyWith(...)` or like so:`instanceOfDataDeletionRequestView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DataDeletionRequestViewCWProxy get copyWith =>
      _$DataDeletionRequestViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DataDeletionRequestView _$DataDeletionRequestViewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('DataDeletionRequestView', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'customerDisplayName',
      'requestedVia',
      'requestedUtc',
      'dueOn',
      'status',
    ],
  );
  final val = DataDeletionRequestView(
    id: $checkedConvert('id', (v) => v as String),
    brandCustomerId: $checkedConvert('brandCustomerId', (v) => v as String?),
    customerDisplayName: $checkedConvert(
      'customerDisplayName',
      (v) => v as String,
    ),
    requestedVia: $checkedConvert(
      'requestedVia',
      (v) => $enumDecode(
        _$DataDeletionRequestViewRequestedViaEnumEnumMap,
        v,
        unknownValue:
            DataDeletionRequestViewRequestedViaEnum.unknownDefaultOpenApi,
      ),
    ),
    requestedUtc: $checkedConvert(
      'requestedUtc',
      (v) => DateTime.parse(v as String),
    ),
    dueOn: $checkedConvert(
      'dueOn',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$DataDeletionRequestViewStatusEnumEnumMap,
        v,
        unknownValue: DataDeletionRequestViewStatusEnum.unknownDefaultOpenApi,
      ),
    ),
    completedUtc: $checkedConvert(
      'completedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$DataDeletionRequestViewToJson(
  DataDeletionRequestView instance,
) => <String, dynamic>{
  'id': instance.id,
  'brandCustomerId': ?instance.brandCustomerId,
  'customerDisplayName': instance.customerDisplayName,
  'requestedVia':
      _$DataDeletionRequestViewRequestedViaEnumEnumMap[instance.requestedVia]!,
  'requestedUtc': instance.requestedUtc.toIso8601String(),
  'dueOn': const DateOnlyConverter().toJson(instance.dueOn),
  'status': _$DataDeletionRequestViewStatusEnumEnumMap[instance.status]!,
  'completedUtc': ?instance.completedUtc?.toIso8601String(),
};

const _$DataDeletionRequestViewRequestedViaEnumEnumMap = {
  DataDeletionRequestViewRequestedViaEnum.customerApp: 'CustomerApp',
  DataDeletionRequestViewRequestedViaEnum.panel: 'Panel',
  DataDeletionRequestViewRequestedViaEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};

const _$DataDeletionRequestViewStatusEnumEnumMap = {
  DataDeletionRequestViewStatusEnum.pending: 'Pending',
  DataDeletionRequestViewStatusEnum.completed: 'Completed',
  DataDeletionRequestViewStatusEnum.rejected: 'Rejected',
  DataDeletionRequestViewStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
