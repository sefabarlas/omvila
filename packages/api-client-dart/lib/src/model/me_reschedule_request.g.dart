// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_reschedule_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeRescheduleRequestCWProxy {
  MeRescheduleRequest startUtc(DateTime startUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeRescheduleRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeRescheduleRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeRescheduleRequest call({DateTime startUtc});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeRescheduleRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeRescheduleRequest.copyWith.fieldName(...)`
class _$MeRescheduleRequestCWProxyImpl implements _$MeRescheduleRequestCWProxy {
  const _$MeRescheduleRequestCWProxyImpl(this._value);

  final MeRescheduleRequest _value;

  @override
  MeRescheduleRequest startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeRescheduleRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeRescheduleRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeRescheduleRequest call({Object? startUtc = const $CopyWithPlaceholder()}) {
    return MeRescheduleRequest(
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
    );
  }
}

extension $MeRescheduleRequestCopyWith on MeRescheduleRequest {
  /// Returns a callable class that can be used as follows: `instanceOfMeRescheduleRequest.copyWith(...)` or like so:`instanceOfMeRescheduleRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeRescheduleRequestCWProxy get copyWith =>
      _$MeRescheduleRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeRescheduleRequest _$MeRescheduleRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeRescheduleRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['startUtc']);
      final val = MeRescheduleRequest(
        startUtc: $checkedConvert(
          'startUtc',
          (v) => DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$MeRescheduleRequestToJson(
  MeRescheduleRequest instance,
) => <String, dynamic>{'startUtc': instance.startUtc.toIso8601String()};
