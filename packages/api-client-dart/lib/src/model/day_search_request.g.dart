// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'day_search_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$DaySearchRequestCWProxy {
  DaySearchRequest items(List<AppointmentItemRequest> items);

  DaySearchRequest sameStaffForAll(bool? sameStaffForAll);

  DaySearchRequest from(DateTime from);

  DaySearchRequest to(DateTime to);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DaySearchRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DaySearchRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  DaySearchRequest call({
    List<AppointmentItemRequest> items,
    bool? sameStaffForAll,
    DateTime from,
    DateTime to,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfDaySearchRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfDaySearchRequest.copyWith.fieldName(...)`
class _$DaySearchRequestCWProxyImpl implements _$DaySearchRequestCWProxy {
  const _$DaySearchRequestCWProxyImpl(this._value);

  final DaySearchRequest _value;

  @override
  DaySearchRequest items(List<AppointmentItemRequest> items) =>
      this(items: items);

  @override
  DaySearchRequest sameStaffForAll(bool? sameStaffForAll) =>
      this(sameStaffForAll: sameStaffForAll);

  @override
  DaySearchRequest from(DateTime from) => this(from: from);

  @override
  DaySearchRequest to(DateTime to) => this(to: to);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `DaySearchRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// DaySearchRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  DaySearchRequest call({
    Object? items = const $CopyWithPlaceholder(),
    Object? sameStaffForAll = const $CopyWithPlaceholder(),
    Object? from = const $CopyWithPlaceholder(),
    Object? to = const $CopyWithPlaceholder(),
  }) {
    return DaySearchRequest(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<AppointmentItemRequest>,
      sameStaffForAll: sameStaffForAll == const $CopyWithPlaceholder()
          ? _value.sameStaffForAll
          // ignore: cast_nullable_to_non_nullable
          : sameStaffForAll as bool?,
      from: from == const $CopyWithPlaceholder()
          ? _value.from
          // ignore: cast_nullable_to_non_nullable
          : from as DateTime,
      to: to == const $CopyWithPlaceholder()
          ? _value.to
          // ignore: cast_nullable_to_non_nullable
          : to as DateTime,
    );
  }
}

extension $DaySearchRequestCopyWith on DaySearchRequest {
  /// Returns a callable class that can be used as follows: `instanceOfDaySearchRequest.copyWith(...)` or like so:`instanceOfDaySearchRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$DaySearchRequestCWProxy get copyWith => _$DaySearchRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DaySearchRequest _$DaySearchRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('DaySearchRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'from', 'to']);
      final val = DaySearchRequest(
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
        from: $checkedConvert('from', (v) => DateTime.parse(v as String)),
        to: $checkedConvert('to', (v) => DateTime.parse(v as String)),
      );
      return val;
    });

Map<String, dynamic> _$DaySearchRequestToJson(DaySearchRequest instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'sameStaffForAll': ?instance.sameStaffForAll,
      'from': instance.from.toIso8601String(),
      'to': instance.to.toIso8601String(),
    };
