// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'my_commission_entry.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MyCommissionEntryCWProxy {
  MyCommissionEntry id(String id);

  MyCommissionEntry type(CommissionEntryType type);

  MyCommissionEntry amountMinor(int amountMinor);

  MyCommissionEntry status(CommissionEntryStatus status);

  MyCommissionEntry occurredUtc(DateTime occurredUtc);

  MyCommissionEntry customerName(String customerName);

  MyCommissionEntry description(String description);

  MyCommissionEntry appointmentId(String? appointmentId);

  MyCommissionEntry customerPackageId(String? customerPackageId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MyCommissionEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MyCommissionEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  MyCommissionEntry call({
    String id,
    CommissionEntryType type,
    int amountMinor,
    CommissionEntryStatus status,
    DateTime occurredUtc,
    String customerName,
    String description,
    String? appointmentId,
    String? customerPackageId,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMyCommissionEntry.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMyCommissionEntry.copyWith.fieldName(...)`
class _$MyCommissionEntryCWProxyImpl implements _$MyCommissionEntryCWProxy {
  const _$MyCommissionEntryCWProxyImpl(this._value);

  final MyCommissionEntry _value;

  @override
  MyCommissionEntry id(String id) => this(id: id);

  @override
  MyCommissionEntry type(CommissionEntryType type) => this(type: type);

  @override
  MyCommissionEntry amountMinor(int amountMinor) =>
      this(amountMinor: amountMinor);

  @override
  MyCommissionEntry status(CommissionEntryStatus status) =>
      this(status: status);

  @override
  MyCommissionEntry occurredUtc(DateTime occurredUtc) =>
      this(occurredUtc: occurredUtc);

  @override
  MyCommissionEntry customerName(String customerName) =>
      this(customerName: customerName);

  @override
  MyCommissionEntry description(String description) =>
      this(description: description);

  @override
  MyCommissionEntry appointmentId(String? appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  MyCommissionEntry customerPackageId(String? customerPackageId) =>
      this(customerPackageId: customerPackageId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MyCommissionEntry(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MyCommissionEntry(...).copyWith(id: 12, name: "My name")
  /// ````
  MyCommissionEntry call({
    Object? id = const $CopyWithPlaceholder(),
    Object? type = const $CopyWithPlaceholder(),
    Object? amountMinor = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? occurredUtc = const $CopyWithPlaceholder(),
    Object? customerName = const $CopyWithPlaceholder(),
    Object? description = const $CopyWithPlaceholder(),
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? customerPackageId = const $CopyWithPlaceholder(),
  }) {
    return MyCommissionEntry(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      type: type == const $CopyWithPlaceholder()
          ? _value.type
          // ignore: cast_nullable_to_non_nullable
          : type as CommissionEntryType,
      amountMinor: amountMinor == const $CopyWithPlaceholder()
          ? _value.amountMinor
          // ignore: cast_nullable_to_non_nullable
          : amountMinor as int,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as CommissionEntryStatus,
      occurredUtc: occurredUtc == const $CopyWithPlaceholder()
          ? _value.occurredUtc
          // ignore: cast_nullable_to_non_nullable
          : occurredUtc as DateTime,
      customerName: customerName == const $CopyWithPlaceholder()
          ? _value.customerName
          // ignore: cast_nullable_to_non_nullable
          : customerName as String,
      description: description == const $CopyWithPlaceholder()
          ? _value.description
          // ignore: cast_nullable_to_non_nullable
          : description as String,
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String?,
      customerPackageId: customerPackageId == const $CopyWithPlaceholder()
          ? _value.customerPackageId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageId as String?,
    );
  }
}

extension $MyCommissionEntryCopyWith on MyCommissionEntry {
  /// Returns a callable class that can be used as follows: `instanceOfMyCommissionEntry.copyWith(...)` or like so:`instanceOfMyCommissionEntry.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MyCommissionEntryCWProxy get copyWith =>
      _$MyCommissionEntryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MyCommissionEntry _$MyCommissionEntryFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MyCommissionEntry', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'type',
          'amountMinor',
          'status',
          'occurredUtc',
          'customerName',
          'description',
        ],
      );
      final val = MyCommissionEntry(
        id: $checkedConvert('id', (v) => v as String),
        type: $checkedConvert(
          'type',
          (v) => $enumDecode(
            _$CommissionEntryTypeEnumMap,
            v,
            unknownValue: CommissionEntryType.unknownDefaultOpenApi,
          ),
        ),
        amountMinor: $checkedConvert('amountMinor', (v) => (v as num).toInt()),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$CommissionEntryStatusEnumMap,
            v,
            unknownValue: CommissionEntryStatus.unknownDefaultOpenApi,
          ),
        ),
        occurredUtc: $checkedConvert(
          'occurredUtc',
          (v) => DateTime.parse(v as String),
        ),
        customerName: $checkedConvert('customerName', (v) => v as String),
        description: $checkedConvert('description', (v) => v as String),
        appointmentId: $checkedConvert('appointmentId', (v) => v as String?),
        customerPackageId: $checkedConvert(
          'customerPackageId',
          (v) => v as String?,
        ),
      );
      return val;
    });

Map<String, dynamic> _$MyCommissionEntryToJson(MyCommissionEntry instance) =>
    <String, dynamic>{
      'id': instance.id,
      'type': _$CommissionEntryTypeEnumMap[instance.type]!,
      'amountMinor': instance.amountMinor,
      'status': _$CommissionEntryStatusEnumMap[instance.status]!,
      'occurredUtc': instance.occurredUtc.toIso8601String(),
      'customerName': instance.customerName,
      'description': instance.description,
      'appointmentId': ?instance.appointmentId,
      'customerPackageId': ?instance.customerPackageId,
    };

const _$CommissionEntryTypeEnumMap = {
  CommissionEntryType.service: 'Service',
  CommissionEntryType.sale: 'Sale',
  CommissionEntryType.unknownDefaultOpenApi: 'unknown_default_open_api',
};

const _$CommissionEntryStatusEnumMap = {
  CommissionEntryStatus.accrued: 'Accrued',
  CommissionEntryStatus.approved: 'Approved',
  CommissionEntryStatus.paid: 'Paid',
  CommissionEntryStatus.reversed: 'Reversed',
  CommissionEntryStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
