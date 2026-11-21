// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_appointment_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeAppointmentPageCWProxy {
  MeAppointmentPage items(List<MeAppointmentSummary> items);

  MeAppointmentPage nextCursor(String? nextCursor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentPage(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentPage call({
    List<MeAppointmentSummary> items,
    String? nextCursor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeAppointmentPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeAppointmentPage.copyWith.fieldName(...)`
class _$MeAppointmentPageCWProxyImpl implements _$MeAppointmentPageCWProxy {
  const _$MeAppointmentPageCWProxyImpl(this._value);

  final MeAppointmentPage _value;

  @override
  MeAppointmentPage items(List<MeAppointmentSummary> items) =>
      this(items: items);

  @override
  MeAppointmentPage nextCursor(String? nextCursor) =>
      this(nextCursor: nextCursor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeAppointmentPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeAppointmentPage(...).copyWith(id: 12, name: "My name")
  /// ````
  MeAppointmentPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
  }) {
    return MeAppointmentPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<MeAppointmentSummary>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
    );
  }
}

extension $MeAppointmentPageCopyWith on MeAppointmentPage {
  /// Returns a callable class that can be used as follows: `instanceOfMeAppointmentPage.copyWith(...)` or like so:`instanceOfMeAppointmentPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeAppointmentPageCWProxy get copyWith =>
      _$MeAppointmentPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeAppointmentPage _$MeAppointmentPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeAppointmentPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor']);
      final val = MeAppointmentPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) => MeAppointmentSummary.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$MeAppointmentPageToJson(MeAppointmentPage instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
    };
