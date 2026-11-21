// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_package_item_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerPackageItemViewCWProxy {
  CustomerPackageItemView customerPackageItemId(String customerPackageItemId);

  CustomerPackageItemView serviceId(String serviceId);

  CustomerPackageItemView serviceName(String serviceName);

  CustomerPackageItemView totalSessions(int totalSessions);

  CustomerPackageItemView remainingSessions(int remainingSessions);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPackageItemView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPackageItemView(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPackageItemView call({
    String customerPackageItemId,
    String serviceId,
    String serviceName,
    int totalSessions,
    int remainingSessions,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerPackageItemView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerPackageItemView.copyWith.fieldName(...)`
class _$CustomerPackageItemViewCWProxyImpl
    implements _$CustomerPackageItemViewCWProxy {
  const _$CustomerPackageItemViewCWProxyImpl(this._value);

  final CustomerPackageItemView _value;

  @override
  CustomerPackageItemView customerPackageItemId(String customerPackageItemId) =>
      this(customerPackageItemId: customerPackageItemId);

  @override
  CustomerPackageItemView serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  CustomerPackageItemView serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  CustomerPackageItemView totalSessions(int totalSessions) =>
      this(totalSessions: totalSessions);

  @override
  CustomerPackageItemView remainingSessions(int remainingSessions) =>
      this(remainingSessions: remainingSessions);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerPackageItemView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerPackageItemView(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerPackageItemView call({
    Object? customerPackageItemId = const $CopyWithPlaceholder(),
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? totalSessions = const $CopyWithPlaceholder(),
    Object? remainingSessions = const $CopyWithPlaceholder(),
  }) {
    return CustomerPackageItemView(
      customerPackageItemId:
          customerPackageItemId == const $CopyWithPlaceholder()
          ? _value.customerPackageItemId
          // ignore: cast_nullable_to_non_nullable
          : customerPackageItemId as String,
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      totalSessions: totalSessions == const $CopyWithPlaceholder()
          ? _value.totalSessions
          // ignore: cast_nullable_to_non_nullable
          : totalSessions as int,
      remainingSessions: remainingSessions == const $CopyWithPlaceholder()
          ? _value.remainingSessions
          // ignore: cast_nullable_to_non_nullable
          : remainingSessions as int,
    );
  }
}

extension $CustomerPackageItemViewCopyWith on CustomerPackageItemView {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerPackageItemView.copyWith(...)` or like so:`instanceOfCustomerPackageItemView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerPackageItemViewCWProxy get copyWith =>
      _$CustomerPackageItemViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerPackageItemView _$CustomerPackageItemViewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('CustomerPackageItemView', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'customerPackageItemId',
      'serviceId',
      'serviceName',
      'totalSessions',
      'remainingSessions',
    ],
  );
  final val = CustomerPackageItemView(
    customerPackageItemId: $checkedConvert(
      'customerPackageItemId',
      (v) => v as String,
    ),
    serviceId: $checkedConvert('serviceId', (v) => v as String),
    serviceName: $checkedConvert('serviceName', (v) => v as String),
    totalSessions: $checkedConvert('totalSessions', (v) => (v as num).toInt()),
    remainingSessions: $checkedConvert(
      'remainingSessions',
      (v) => (v as num).toInt(),
    ),
  );
  return val;
});

Map<String, dynamic> _$CustomerPackageItemViewToJson(
  CustomerPackageItemView instance,
) => <String, dynamic>{
  'customerPackageItemId': instance.customerPackageItemId,
  'serviceId': instance.serviceId,
  'serviceName': instance.serviceName,
  'totalSessions': instance.totalSessions,
  'remainingSessions': instance.remainingSessions,
};
