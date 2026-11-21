// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_definition_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageDefinitionItemsInnerCWProxy {
  PackageDefinitionItemsInner serviceId(String serviceId);

  PackageDefinitionItemsInner serviceName(String serviceName);

  PackageDefinitionItemsInner sessionCount(int sessionCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageDefinitionItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageDefinitionItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageDefinitionItemsInner call({
    String serviceId,
    String serviceName,
    int sessionCount,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageDefinitionItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageDefinitionItemsInner.copyWith.fieldName(...)`
class _$PackageDefinitionItemsInnerCWProxyImpl
    implements _$PackageDefinitionItemsInnerCWProxy {
  const _$PackageDefinitionItemsInnerCWProxyImpl(this._value);

  final PackageDefinitionItemsInner _value;

  @override
  PackageDefinitionItemsInner serviceId(String serviceId) =>
      this(serviceId: serviceId);

  @override
  PackageDefinitionItemsInner serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  PackageDefinitionItemsInner sessionCount(int sessionCount) =>
      this(sessionCount: sessionCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageDefinitionItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageDefinitionItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageDefinitionItemsInner call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? sessionCount = const $CopyWithPlaceholder(),
  }) {
    return PackageDefinitionItemsInner(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      sessionCount: sessionCount == const $CopyWithPlaceholder()
          ? _value.sessionCount
          // ignore: cast_nullable_to_non_nullable
          : sessionCount as int,
    );
  }
}

extension $PackageDefinitionItemsInnerCopyWith on PackageDefinitionItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfPackageDefinitionItemsInner.copyWith(...)` or like so:`instanceOfPackageDefinitionItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageDefinitionItemsInnerCWProxy get copyWith =>
      _$PackageDefinitionItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageDefinitionItemsInner _$PackageDefinitionItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PackageDefinitionItemsInner', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['serviceId', 'serviceName', 'sessionCount'],
  );
  final val = PackageDefinitionItemsInner(
    serviceId: $checkedConvert('serviceId', (v) => v as String),
    serviceName: $checkedConvert('serviceName', (v) => v as String),
    sessionCount: $checkedConvert('sessionCount', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$PackageDefinitionItemsInnerToJson(
  PackageDefinitionItemsInner instance,
) => <String, dynamic>{
  'serviceId': instance.serviceId,
  'serviceName': instance.serviceName,
  'sessionCount': instance.sessionCount,
};
