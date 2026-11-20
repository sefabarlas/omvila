// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'manage_reschedule_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ManageRescheduleRequestCWProxy {
  ManageRescheduleRequest startUtc(DateTime startUtc);

  ManageRescheduleRequest items(List<ManageRescheduleRequestItemsInner>? items);

  ManageRescheduleRequest scope(ManageRescheduleRequestScopeEnum? scope);

  ManageRescheduleRequest notifyCustomer(bool? notifyCustomer);

  ManageRescheduleRequest allowOutsideWorkingHours(
    bool? allowOutsideWorkingHours,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageRescheduleRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageRescheduleRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageRescheduleRequest call({
    DateTime startUtc,
    List<ManageRescheduleRequestItemsInner>? items,
    ManageRescheduleRequestScopeEnum? scope,
    bool? notifyCustomer,
    bool? allowOutsideWorkingHours,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfManageRescheduleRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfManageRescheduleRequest.copyWith.fieldName(...)`
class _$ManageRescheduleRequestCWProxyImpl
    implements _$ManageRescheduleRequestCWProxy {
  const _$ManageRescheduleRequestCWProxyImpl(this._value);

  final ManageRescheduleRequest _value;

  @override
  ManageRescheduleRequest startUtc(DateTime startUtc) =>
      this(startUtc: startUtc);

  @override
  ManageRescheduleRequest items(
    List<ManageRescheduleRequestItemsInner>? items,
  ) => this(items: items);

  @override
  ManageRescheduleRequest scope(ManageRescheduleRequestScopeEnum? scope) =>
      this(scope: scope);

  @override
  ManageRescheduleRequest notifyCustomer(bool? notifyCustomer) =>
      this(notifyCustomer: notifyCustomer);

  @override
  ManageRescheduleRequest allowOutsideWorkingHours(
    bool? allowOutsideWorkingHours,
  ) => this(allowOutsideWorkingHours: allowOutsideWorkingHours);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ManageRescheduleRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ManageRescheduleRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  ManageRescheduleRequest call({
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? scope = const $CopyWithPlaceholder(),
    Object? notifyCustomer = const $CopyWithPlaceholder(),
    Object? allowOutsideWorkingHours = const $CopyWithPlaceholder(),
  }) {
    return ManageRescheduleRequest(
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ManageRescheduleRequestItemsInner>?,
      scope: scope == const $CopyWithPlaceholder()
          ? _value.scope
          // ignore: cast_nullable_to_non_nullable
          : scope as ManageRescheduleRequestScopeEnum?,
      notifyCustomer: notifyCustomer == const $CopyWithPlaceholder()
          ? _value.notifyCustomer
          // ignore: cast_nullable_to_non_nullable
          : notifyCustomer as bool?,
      allowOutsideWorkingHours:
          allowOutsideWorkingHours == const $CopyWithPlaceholder()
          ? _value.allowOutsideWorkingHours
          // ignore: cast_nullable_to_non_nullable
          : allowOutsideWorkingHours as bool?,
    );
  }
}

extension $ManageRescheduleRequestCopyWith on ManageRescheduleRequest {
  /// Returns a callable class that can be used as follows: `instanceOfManageRescheduleRequest.copyWith(...)` or like so:`instanceOfManageRescheduleRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ManageRescheduleRequestCWProxy get copyWith =>
      _$ManageRescheduleRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ManageRescheduleRequest _$ManageRescheduleRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ManageRescheduleRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['startUtc']);
  final val = ManageRescheduleRequest(
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>?)
          ?.map(
            (e) => ManageRescheduleRequestItemsInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
    scope: $checkedConvert(
      'scope',
      (v) => $enumDecodeNullable(
        _$ManageRescheduleRequestScopeEnumEnumMap,
        v,
        unknownValue: ManageRescheduleRequestScopeEnum.unknownDefaultOpenApi,
      ),
    ),
    notifyCustomer: $checkedConvert(
      'notifyCustomer',
      (v) => v as bool? ?? true,
    ),
    allowOutsideWorkingHours: $checkedConvert(
      'allowOutsideWorkingHours',
      (v) => v as bool? ?? false,
    ),
  );
  return val;
});

Map<String, dynamic> _$ManageRescheduleRequestToJson(
  ManageRescheduleRequest instance,
) => <String, dynamic>{
  'startUtc': instance.startUtc.toIso8601String(),
  'items': ?instance.items?.map((e) => e.toJson()).toList(),
  'scope': ?_$ManageRescheduleRequestScopeEnumEnumMap[instance.scope],
  'notifyCustomer': ?instance.notifyCustomer,
  'allowOutsideWorkingHours': ?instance.allowOutsideWorkingHours,
};

const _$ManageRescheduleRequestScopeEnumEnumMap = {
  ManageRescheduleRequestScopeEnum.thisOnly: 'ThisOnly',
  ManageRescheduleRequestScopeEnum.thisAndFollowing: 'ThisAndFollowing',
  ManageRescheduleRequestScopeEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
