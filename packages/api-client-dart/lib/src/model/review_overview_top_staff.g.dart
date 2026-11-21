// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_overview_top_staff.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReviewOverviewTopStaffCWProxy {
  ReviewOverviewTopStaff staffId(String staffId);

  ReviewOverviewTopStaff fullName(String fullName);

  ReviewOverviewTopStaff averageRating(num averageRating);

  ReviewOverviewTopStaff count(int count);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewOverviewTopStaff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewOverviewTopStaff(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewOverviewTopStaff call({
    String staffId,
    String fullName,
    num averageRating,
    int count,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReviewOverviewTopStaff.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReviewOverviewTopStaff.copyWith.fieldName(...)`
class _$ReviewOverviewTopStaffCWProxyImpl
    implements _$ReviewOverviewTopStaffCWProxy {
  const _$ReviewOverviewTopStaffCWProxyImpl(this._value);

  final ReviewOverviewTopStaff _value;

  @override
  ReviewOverviewTopStaff staffId(String staffId) => this(staffId: staffId);

  @override
  ReviewOverviewTopStaff fullName(String fullName) => this(fullName: fullName);

  @override
  ReviewOverviewTopStaff averageRating(num averageRating) =>
      this(averageRating: averageRating);

  @override
  ReviewOverviewTopStaff count(int count) => this(count: count);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewOverviewTopStaff(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewOverviewTopStaff(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewOverviewTopStaff call({
    Object? staffId = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? averageRating = const $CopyWithPlaceholder(),
    Object? count = const $CopyWithPlaceholder(),
  }) {
    return ReviewOverviewTopStaff(
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      averageRating: averageRating == const $CopyWithPlaceholder()
          ? _value.averageRating
          // ignore: cast_nullable_to_non_nullable
          : averageRating as num,
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
    );
  }
}

extension $ReviewOverviewTopStaffCopyWith on ReviewOverviewTopStaff {
  /// Returns a callable class that can be used as follows: `instanceOfReviewOverviewTopStaff.copyWith(...)` or like so:`instanceOfReviewOverviewTopStaff.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReviewOverviewTopStaffCWProxy get copyWith =>
      _$ReviewOverviewTopStaffCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReviewOverviewTopStaff _$ReviewOverviewTopStaffFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('ReviewOverviewTopStaff', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['staffId', 'fullName', 'averageRating', 'count'],
  );
  final val = ReviewOverviewTopStaff(
    staffId: $checkedConvert('staffId', (v) => v as String),
    fullName: $checkedConvert('fullName', (v) => v as String),
    averageRating: $checkedConvert('averageRating', (v) => v as num),
    count: $checkedConvert('count', (v) => (v as num).toInt()),
  );
  return val;
});

Map<String, dynamic> _$ReviewOverviewTopStaffToJson(
  ReviewOverviewTopStaff instance,
) => <String, dynamic>{
  'staffId': instance.staffId,
  'fullName': instance.fullName,
  'averageRating': instance.averageRating,
  'count': instance.count,
};
