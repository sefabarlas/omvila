// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_create_services_from_templates_request_items_inner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageCreateServicesFromTemplatesRequestItemsInnerCWProxy {
  ManageCreateServicesFromTemplatesRequestItemsInner templateId(
    String templateId,
  );

  ManageCreateServicesFromTemplatesRequestItemsInner price(Money? price);

  ManageCreateServicesFromTemplatesRequestItemsInner durationMinutes(
    int? durationMinutes,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCreateServicesFromTemplatesRequestItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCreateServicesFromTemplatesRequestItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCreateServicesFromTemplatesRequestItemsInner call({
    String templateId,
    Money? price,
    int? durationMinutes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageCreateServicesFromTemplatesRequestItemsInner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageCreateServicesFromTemplatesRequestItemsInner.copyWith.fieldName(...)`
class _$ManageCreateServicesFromTemplatesRequestItemsInnerCWProxyImpl
    implements _$ManageCreateServicesFromTemplatesRequestItemsInnerCWProxy {
  const _$ManageCreateServicesFromTemplatesRequestItemsInnerCWProxyImpl(
    this._value,
  );

  final ManageCreateServicesFromTemplatesRequestItemsInner _value;

  @override
  ManageCreateServicesFromTemplatesRequestItemsInner templateId(
    String templateId,
  ) => this(templateId: templateId);

  @override
  ManageCreateServicesFromTemplatesRequestItemsInner price(Money? price) =>
      this(price: price);

  @override
  ManageCreateServicesFromTemplatesRequestItemsInner durationMinutes(
    int? durationMinutes,
  ) => this(durationMinutes: durationMinutes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageCreateServicesFromTemplatesRequestItemsInner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageCreateServicesFromTemplatesRequestItemsInner(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageCreateServicesFromTemplatesRequestItemsInner call({
    Object? templateId = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? durationMinutes = const $CopyWithPlaceholder(),
  }) {
    return ManageCreateServicesFromTemplatesRequestItemsInner(
      templateId: templateId == const $CopyWithPlaceholder()
          ? _value.templateId
          // ignore: cast_nullable_to_non_nullable
          : templateId as String,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money?,
      durationMinutes: durationMinutes == const $CopyWithPlaceholder()
          ? _value.durationMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationMinutes as int?,
    );
  }
}

extension $ManageCreateServicesFromTemplatesRequestItemsInnerCopyWith
    on ManageCreateServicesFromTemplatesRequestItemsInner {
  /// Returns a callable class that can be used as follows: `instanceOfManageCreateServicesFromTemplatesRequestItemsInner.copyWith(...)` or like so:`instanceOfManageCreateServicesFromTemplatesRequestItemsInner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageCreateServicesFromTemplatesRequestItemsInnerCWProxy get copyWith =>
      _$ManageCreateServicesFromTemplatesRequestItemsInnerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageCreateServicesFromTemplatesRequestItemsInner
_$ManageCreateServicesFromTemplatesRequestItemsInnerFromJson(
  Map<String, dynamic> json,
) => $checkedCreate(
  'ManageCreateServicesFromTemplatesRequestItemsInner',
  json,
  ($checkedConvert) {
    $checkKeys(json, requiredKeys: const ['templateId']);
    final val = ManageCreateServicesFromTemplatesRequestItemsInner(
      templateId: $checkedConvert('templateId', (v) => v as String),
      price: $checkedConvert(
        'price',
        (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
      ),
      durationMinutes: $checkedConvert(
        'durationMinutes',
        (v) => (v as num?)?.toInt(),
      ),
    );
    return val;
  },
);

Map<String, dynamic> _$ManageCreateServicesFromTemplatesRequestItemsInnerToJson(
  ManageCreateServicesFromTemplatesRequestItemsInner instance,
) => <String, dynamic>{
  'templateId': instance.templateId,
  'price': ?instance.price?.toJson(),
  'durationMinutes': ?instance.durationMinutes,
};
