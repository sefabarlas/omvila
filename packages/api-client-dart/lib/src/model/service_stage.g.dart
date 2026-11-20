// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_stage.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ServiceStageCWProxy {
  ServiceStage name(String name);

  ServiceStage durationMinutes(int durationMinutes);

  ServiceStage staffBusy(bool staffBusy);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceStage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceStage(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceStage call({String name, int durationMinutes, bool staffBusy});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfServiceStage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfServiceStage.copyWith.fieldName(...)`
class _$ServiceStageCWProxyImpl implements _$ServiceStageCWProxy {
  const _$ServiceStageCWProxyImpl(this._value);

  final ServiceStage _value;

  @override
  ServiceStage name(String name) => this(name: name);

  @override
  ServiceStage durationMinutes(int durationMinutes) =>
      this(durationMinutes: durationMinutes);

  @override
  ServiceStage staffBusy(bool staffBusy) => this(staffBusy: staffBusy);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceStage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceStage(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceStage call({
    Object? name = const $CopyWithPlaceholder(),
    Object? durationMinutes = const $CopyWithPlaceholder(),
    Object? staffBusy = const $CopyWithPlaceholder(),
  }) {
    return ServiceStage(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      durationMinutes: durationMinutes == const $CopyWithPlaceholder()
          ? _value.durationMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationMinutes as int,
      staffBusy: staffBusy == const $CopyWithPlaceholder()
          ? _value.staffBusy
          // ignore: cast_nullable_to_non_nullable
          : staffBusy as bool,
    );
  }
}

extension $ServiceStageCopyWith on ServiceStage {
  /// Returns a callable class that can be used as follows: `instanceOfServiceStage.copyWith(...)` or like so:`instanceOfServiceStage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ServiceStageCWProxy get copyWith => _$ServiceStageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ServiceStage _$ServiceStageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ServiceStage', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['name', 'durationMinutes', 'staffBusy'],
      );
      final val = ServiceStage(
        name: $checkedConvert('name', (v) => v as String),
        durationMinutes: $checkedConvert(
          'durationMinutes',
          (v) => (v as num).toInt(),
        ),
        staffBusy: $checkedConvert('staffBusy', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$ServiceStageToJson(ServiceStage instance) =>
    <String, dynamic>{
      'name': instance.name,
      'durationMinutes': instance.durationMinutes,
      'staffBusy': instance.staffBusy,
    };
