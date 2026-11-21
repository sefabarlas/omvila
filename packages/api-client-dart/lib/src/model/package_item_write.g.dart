// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_item_write.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageItemWriteCWProxy {
  PackageItemWrite serviceId(String serviceId);

  PackageItemWrite sessionCount(int sessionCount);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageItemWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageItemWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageItemWrite call({String serviceId, int sessionCount});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageItemWrite.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageItemWrite.copyWith.fieldName(...)`
class _$PackageItemWriteCWProxyImpl implements _$PackageItemWriteCWProxy {
  const _$PackageItemWriteCWProxyImpl(this._value);

  final PackageItemWrite _value;

  @override
  PackageItemWrite serviceId(String serviceId) => this(serviceId: serviceId);

  @override
  PackageItemWrite sessionCount(int sessionCount) =>
      this(sessionCount: sessionCount);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageItemWrite(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageItemWrite(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageItemWrite call({
    Object? serviceId = const $CopyWithPlaceholder(),
    Object? sessionCount = const $CopyWithPlaceholder(),
  }) {
    return PackageItemWrite(
      serviceId: serviceId == const $CopyWithPlaceholder()
          ? _value.serviceId
          // ignore: cast_nullable_to_non_nullable
          : serviceId as String,
      sessionCount: sessionCount == const $CopyWithPlaceholder()
          ? _value.sessionCount
          // ignore: cast_nullable_to_non_nullable
          : sessionCount as int,
    );
  }
}

extension $PackageItemWriteCopyWith on PackageItemWrite {
  /// Returns a callable class that can be used as follows: `instanceOfPackageItemWrite.copyWith(...)` or like so:`instanceOfPackageItemWrite.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageItemWriteCWProxy get copyWith => _$PackageItemWriteCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageItemWrite _$PackageItemWriteFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PackageItemWrite', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['serviceId', 'sessionCount']);
      final val = PackageItemWrite(
        serviceId: $checkedConvert('serviceId', (v) => v as String),
        sessionCount: $checkedConvert(
          'sessionCount',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PackageItemWriteToJson(PackageItemWrite instance) =>
    <String, dynamic>{
      'serviceId': instance.serviceId,
      'sessionCount': instance.sessionCount,
    };
