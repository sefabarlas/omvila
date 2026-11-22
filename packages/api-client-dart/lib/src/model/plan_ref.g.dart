// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'plan_ref.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PlanRefCWProxy {
  PlanRef code(String code);

  PlanRef name(String name);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PlanRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PlanRef(...).copyWith(id: 12, name: "My name")
  /// ````
  PlanRef call({String code, String name});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPlanRef.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPlanRef.copyWith.fieldName(...)`
class _$PlanRefCWProxyImpl implements _$PlanRefCWProxy {
  const _$PlanRefCWProxyImpl(this._value);

  final PlanRef _value;

  @override
  PlanRef code(String code) => this(code: code);

  @override
  PlanRef name(String name) => this(name: name);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PlanRef(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PlanRef(...).copyWith(id: 12, name: "My name")
  /// ````
  PlanRef call({
    Object? code = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
  }) {
    return PlanRef(
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
    );
  }
}

extension $PlanRefCopyWith on PlanRef {
  /// Returns a callable class that can be used as follows: `instanceOfPlanRef.copyWith(...)` or like so:`instanceOfPlanRef.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PlanRefCWProxy get copyWith => _$PlanRefCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlanRef _$PlanRefFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PlanRef', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['code', 'name']);
      final val = PlanRef(
        code: $checkedConvert('code', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$PlanRefToJson(PlanRef instance) => <String, dynamic>{
  'code': instance.code,
  'name': instance.name,
};
