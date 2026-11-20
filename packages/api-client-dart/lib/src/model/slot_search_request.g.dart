// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'slot_search_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SlotSearchRequestCWProxy {
  SlotSearchRequest items(List<AppointmentItemRequest> items);

  SlotSearchRequest sameStaffForAll(bool? sameStaffForAll);

  SlotSearchRequest date(DateTime date);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SlotSearchRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SlotSearchRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  SlotSearchRequest call({
    List<AppointmentItemRequest> items,
    bool? sameStaffForAll,
    DateTime date,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSlotSearchRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSlotSearchRequest.copyWith.fieldName(...)`
class _$SlotSearchRequestCWProxyImpl implements _$SlotSearchRequestCWProxy {
  const _$SlotSearchRequestCWProxyImpl(this._value);

  final SlotSearchRequest _value;

  @override
  SlotSearchRequest items(List<AppointmentItemRequest> items) =>
      this(items: items);

  @override
  SlotSearchRequest sameStaffForAll(bool? sameStaffForAll) =>
      this(sameStaffForAll: sameStaffForAll);

  @override
  SlotSearchRequest date(DateTime date) => this(date: date);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SlotSearchRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SlotSearchRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  SlotSearchRequest call({
    Object? items = const $CopyWithPlaceholder(),
    Object? sameStaffForAll = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
  }) {
    return SlotSearchRequest(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AppointmentItemRequest>,
      sameStaffForAll: sameStaffForAll == const $CopyWithPlaceholder()
          ? _value.sameStaffForAll
          // ignore: cast_nullable_to_non_nullable
          : sameStaffForAll as bool?,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
    );
  }
}

extension $SlotSearchRequestCopyWith on SlotSearchRequest {
  /// Returns a callable class that can be used as follows: `instanceOfSlotSearchRequest.copyWith(...)` or like so:`instanceOfSlotSearchRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SlotSearchRequestCWProxy get copyWith =>
      _$SlotSearchRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SlotSearchRequest _$SlotSearchRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SlotSearchRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'date']);
      final val = SlotSearchRequest(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) =>
                    AppointmentItemRequest.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        sameStaffForAll: $checkedConvert(
          'sameStaffForAll',
          (v) => v as bool? ?? true,
        ),
        date: $checkedConvert('date', (v) => DateTime.parse(v as String)),
      );
      return val;
    });

Map<String, dynamic> _$SlotSearchRequestToJson(SlotSearchRequest instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'sameStaffForAll': ?instance.sameStaffForAll,
      'date': instance.date.toIso8601String(),
    };
