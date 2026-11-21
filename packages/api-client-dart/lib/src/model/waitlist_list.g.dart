// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'waitlist_list.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WaitlistListCWProxy {
  WaitlistList branchId(String branchId);

  WaitlistList timeZone(String timeZone);

  WaitlistList generatedUtc(DateTime generatedUtc);

  WaitlistList items(List<WaitlistEntryView> items);

  WaitlistList overview(WaitlistOverview overview);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistList(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistList call({
    String branchId,
    String timeZone,
    DateTime generatedUtc,
    List<WaitlistEntryView> items,
    WaitlistOverview overview,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWaitlistList.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWaitlistList.copyWith.fieldName(...)`
class _$WaitlistListCWProxyImpl implements _$WaitlistListCWProxy {
  const _$WaitlistListCWProxyImpl(this._value);

  final WaitlistList _value;

  @override
  WaitlistList branchId(String branchId) => this(branchId: branchId);

  @override
  WaitlistList timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  WaitlistList generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  WaitlistList items(List<WaitlistEntryView> items) => this(items: items);

  @override
  WaitlistList overview(WaitlistOverview overview) => this(overview: overview);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistList(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistList(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistList call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? overview = const $CopyWithPlaceholder(),
  }) {
    return WaitlistList(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      generatedUtc: generatedUtc == const $CopyWithPlaceholder()
          ? _value.generatedUtc
          // ignore: cast_nullable_to_non_nullable
          : generatedUtc as DateTime,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<WaitlistEntryView>,
      overview: overview == const $CopyWithPlaceholder()
          ? _value.overview
          // ignore: cast_nullable_to_non_nullable
          : overview as WaitlistOverview,
    );
  }
}

extension $WaitlistListCopyWith on WaitlistList {
  /// Returns a callable class that can be used as follows: `instanceOfWaitlistList.copyWith(...)` or like so:`instanceOfWaitlistList.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WaitlistListCWProxy get copyWith => _$WaitlistListCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WaitlistList _$WaitlistListFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WaitlistList', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'timeZone',
          'generatedUtc',
          'items',
          'overview',
        ],
      );
      final val = WaitlistList(
        branchId: $checkedConvert('branchId', (v) => v as String),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        generatedUtc: $checkedConvert(
          'generatedUtc',
          (v) => DateTime.parse(v as String),
        ),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => WaitlistEntryView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        overview: $checkedConvert(
          'overview',
          (v) => WaitlistOverview.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$WaitlistListToJson(WaitlistList instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'timeZone': instance.timeZone,
      'generatedUtc': instance.generatedUtc.toIso8601String(),
      'items': instance.items.map((e) => e.toJson()).toList(),
      'overview': instance.overview.toJson(),
    };
