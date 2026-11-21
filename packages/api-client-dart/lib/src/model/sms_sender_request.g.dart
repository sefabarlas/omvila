// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sms_sender_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SmsSenderRequestCWProxy {
  SmsSenderRequest name(String name);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsSenderRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsSenderRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsSenderRequest call({String name});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSmsSenderRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSmsSenderRequest.copyWith.fieldName(...)`
class _$SmsSenderRequestCWProxyImpl implements _$SmsSenderRequestCWProxy {
  const _$SmsSenderRequestCWProxyImpl(this._value);

  final SmsSenderRequest _value;

  @override
  SmsSenderRequest name(String name) => this(name: name);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SmsSenderRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SmsSenderRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  SmsSenderRequest call({Object? name = const $CopyWithPlaceholder()}) {
    return SmsSenderRequest(
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
    );
  }
}

extension $SmsSenderRequestCopyWith on SmsSenderRequest {
  /// Returns a callable class that can be used as follows: `instanceOfSmsSenderRequest.copyWith(...)` or like so:`instanceOfSmsSenderRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SmsSenderRequestCWProxy get copyWith => _$SmsSenderRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SmsSenderRequest _$SmsSenderRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SmsSenderRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['name']);
      final val = SmsSenderRequest(
        name: $checkedConvert('name', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$SmsSenderRequestToJson(SmsSenderRequest instance) =>
    <String, dynamic>{'name': instance.name};
