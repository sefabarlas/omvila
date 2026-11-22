// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_search_reschedule_slots_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicSearchRescheduleSlotsRequestCWProxy {
  PublicSearchRescheduleSlotsRequest date(DateTime date);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicSearchRescheduleSlotsRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicSearchRescheduleSlotsRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicSearchRescheduleSlotsRequest call({DateTime date});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicSearchRescheduleSlotsRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicSearchRescheduleSlotsRequest.copyWith.fieldName(...)`
class _$PublicSearchRescheduleSlotsRequestCWProxyImpl
    implements _$PublicSearchRescheduleSlotsRequestCWProxy {
  const _$PublicSearchRescheduleSlotsRequestCWProxyImpl(this._value);

  final PublicSearchRescheduleSlotsRequest _value;

  @override
  PublicSearchRescheduleSlotsRequest date(DateTime date) => this(date: date);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicSearchRescheduleSlotsRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicSearchRescheduleSlotsRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicSearchRescheduleSlotsRequest call({
    Object? date = const $CopyWithPlaceholder(),
  }) {
    return PublicSearchRescheduleSlotsRequest(
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
    );
  }
}

extension $PublicSearchRescheduleSlotsRequestCopyWith
    on PublicSearchRescheduleSlotsRequest {
  /// Returns a callable class that can be used as follows: `instanceOfPublicSearchRescheduleSlotsRequest.copyWith(...)` or like so:`instanceOfPublicSearchRescheduleSlotsRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicSearchRescheduleSlotsRequestCWProxy get copyWith =>
      _$PublicSearchRescheduleSlotsRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicSearchRescheduleSlotsRequest _$PublicSearchRescheduleSlotsRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicSearchRescheduleSlotsRequest', json, (
  $checkedConvert,
) {
  $checkKeys(json, requiredKeys: const ['date']);
  final val = PublicSearchRescheduleSlotsRequest(
    date: $checkedConvert(
      'date',
      (v) => const DateOnlyConverter().fromJson(v as String),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicSearchRescheduleSlotsRequestToJson(
  PublicSearchRescheduleSlotsRequest instance,
) => <String, dynamic>{'date': const DateOnlyConverter().toJson(instance.date)};
