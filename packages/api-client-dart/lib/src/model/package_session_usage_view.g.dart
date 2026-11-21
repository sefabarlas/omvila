// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'package_session_usage_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PackageSessionUsageViewCWProxy {
  PackageSessionUsageView appointmentId(String appointmentId);

  PackageSessionUsageView serviceName(String serviceName);

  PackageSessionUsageView appointmentStartUtc(DateTime appointmentStartUtc);

  PackageSessionUsageView usedUtc(DateTime usedUtc);

  PackageSessionUsageView revertedUtc(DateTime? revertedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageSessionUsageView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageSessionUsageView(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageSessionUsageView call({
    String appointmentId,
    String serviceName,
    DateTime appointmentStartUtc,
    DateTime usedUtc,
    DateTime? revertedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPackageSessionUsageView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPackageSessionUsageView.copyWith.fieldName(...)`
class _$PackageSessionUsageViewCWProxyImpl
    implements _$PackageSessionUsageViewCWProxy {
  const _$PackageSessionUsageViewCWProxyImpl(this._value);

  final PackageSessionUsageView _value;

  @override
  PackageSessionUsageView appointmentId(String appointmentId) =>
      this(appointmentId: appointmentId);

  @override
  PackageSessionUsageView serviceName(String serviceName) =>
      this(serviceName: serviceName);

  @override
  PackageSessionUsageView appointmentStartUtc(DateTime appointmentStartUtc) =>
      this(appointmentStartUtc: appointmentStartUtc);

  @override
  PackageSessionUsageView usedUtc(DateTime usedUtc) => this(usedUtc: usedUtc);

  @override
  PackageSessionUsageView revertedUtc(DateTime? revertedUtc) =>
      this(revertedUtc: revertedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PackageSessionUsageView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PackageSessionUsageView(...).copyWith(id: 12, name: "My name")
  /// ````
  PackageSessionUsageView call({
    Object? appointmentId = const $CopyWithPlaceholder(),
    Object? serviceName = const $CopyWithPlaceholder(),
    Object? appointmentStartUtc = const $CopyWithPlaceholder(),
    Object? usedUtc = const $CopyWithPlaceholder(),
    Object? revertedUtc = const $CopyWithPlaceholder(),
  }) {
    return PackageSessionUsageView(
      appointmentId: appointmentId == const $CopyWithPlaceholder()
          ? _value.appointmentId
          // ignore: cast_nullable_to_non_nullable
          : appointmentId as String,
      serviceName: serviceName == const $CopyWithPlaceholder()
          ? _value.serviceName
          // ignore: cast_nullable_to_non_nullable
          : serviceName as String,
      appointmentStartUtc: appointmentStartUtc == const $CopyWithPlaceholder()
          ? _value.appointmentStartUtc
          // ignore: cast_nullable_to_non_nullable
          : appointmentStartUtc as DateTime,
      usedUtc: usedUtc == const $CopyWithPlaceholder()
          ? _value.usedUtc
          // ignore: cast_nullable_to_non_nullable
          : usedUtc as DateTime,
      revertedUtc: revertedUtc == const $CopyWithPlaceholder()
          ? _value.revertedUtc
          // ignore: cast_nullable_to_non_nullable
          : revertedUtc as DateTime?,
    );
  }
}

extension $PackageSessionUsageViewCopyWith on PackageSessionUsageView {
  /// Returns a callable class that can be used as follows: `instanceOfPackageSessionUsageView.copyWith(...)` or like so:`instanceOfPackageSessionUsageView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PackageSessionUsageViewCWProxy get copyWith =>
      _$PackageSessionUsageViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PackageSessionUsageView _$PackageSessionUsageViewFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PackageSessionUsageView', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'appointmentId',
      'serviceName',
      'appointmentStartUtc',
      'usedUtc',
    ],
  );
  final val = PackageSessionUsageView(
    appointmentId: $checkedConvert('appointmentId', (v) => v as String),
    serviceName: $checkedConvert('serviceName', (v) => v as String),
    appointmentStartUtc: $checkedConvert(
      'appointmentStartUtc',
      (v) => DateTime.parse(v as String),
    ),
    usedUtc: $checkedConvert('usedUtc', (v) => DateTime.parse(v as String)),
    revertedUtc: $checkedConvert(
      'revertedUtc',
      (v) => v == null ? null : DateTime.parse(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$PackageSessionUsageViewToJson(
  PackageSessionUsageView instance,
) => <String, dynamic>{
  'appointmentId': instance.appointmentId,
  'serviceName': instance.serviceName,
  'appointmentStartUtc': instance.appointmentStartUtc.toIso8601String(),
  'usedUtc': instance.usedUtc.toIso8601String(),
  'revertedUtc': ?instance.revertedUtc?.toIso8601String(),
};
