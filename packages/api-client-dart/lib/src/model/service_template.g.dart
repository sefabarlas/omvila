// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'service_template.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ServiceTemplateCWProxy {
  ServiceTemplate id(String id);

  ServiceTemplate categoryName(String categoryName);

  ServiceTemplate name(String name);

  ServiceTemplate defaultDurationMinutes(int defaultDurationMinutes);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceTemplate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceTemplate(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceTemplate call({
    String id,
    String categoryName,
    String name,
    int defaultDurationMinutes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfServiceTemplate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfServiceTemplate.copyWith.fieldName(...)`
class _$ServiceTemplateCWProxyImpl implements _$ServiceTemplateCWProxy {
  const _$ServiceTemplateCWProxyImpl(this._value);

  final ServiceTemplate _value;

  @override
  ServiceTemplate id(String id) => this(id: id);

  @override
  ServiceTemplate categoryName(String categoryName) =>
      this(categoryName: categoryName);

  @override
  ServiceTemplate name(String name) => this(name: name);

  @override
  ServiceTemplate defaultDurationMinutes(int defaultDurationMinutes) =>
      this(defaultDurationMinutes: defaultDurationMinutes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ServiceTemplate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ServiceTemplate(...).copyWith(id: 12, name: "My name")
  /// ````
  ServiceTemplate call({
    Object? id = const $CopyWithPlaceholder(),
    Object? categoryName = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? defaultDurationMinutes = const $CopyWithPlaceholder(),
  }) {
    return ServiceTemplate(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      categoryName: categoryName == const $CopyWithPlaceholder()
          ? _value.categoryName
          // ignore: cast_nullable_to_non_nullable
          : categoryName as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      defaultDurationMinutes:
          defaultDurationMinutes == const $CopyWithPlaceholder()
          ? _value.defaultDurationMinutes
          // ignore: cast_nullable_to_non_nullable
          : defaultDurationMinutes as int,
    );
  }
}

extension $ServiceTemplateCopyWith on ServiceTemplate {
  /// Returns a callable class that can be used as follows: `instanceOfServiceTemplate.copyWith(...)` or like so:`instanceOfServiceTemplate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ServiceTemplateCWProxy get copyWith => _$ServiceTemplateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ServiceTemplate _$ServiceTemplateFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ServiceTemplate', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'categoryName',
          'name',
          'defaultDurationMinutes',
        ],
      );
      final val = ServiceTemplate(
        id: $checkedConvert('id', (v) => v as String),
        categoryName: $checkedConvert('categoryName', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        defaultDurationMinutes: $checkedConvert(
          'defaultDurationMinutes',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ServiceTemplateToJson(ServiceTemplate instance) =>
    <String, dynamic>{
      'id': instance.id,
      'categoryName': instance.categoryName,
      'name': instance.name,
      'defaultDurationMinutes': instance.defaultDurationMinutes,
    };
