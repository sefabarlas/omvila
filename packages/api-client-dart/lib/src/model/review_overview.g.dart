// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReviewOverviewCWProxy {
  ReviewOverview averageRating(num? averageRating);

  ReviewOverview ratingCount(int ratingCount);

  ReviewOverview totalCount(int totalCount);

  ReviewOverview awaitingReplyCount(int awaitingReplyCount);

  ReviewOverview thisMonthCount(int thisMonthCount);

  ReviewOverview thisMonthInviteResponsePercent(
    num? thisMonthInviteResponsePercent,
  );

  ReviewOverview topStaff(ReviewOverviewTopStaff? topStaff);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewOverview call({
    num? averageRating,
    int ratingCount,
    int totalCount,
    int awaitingReplyCount,
    int thisMonthCount,
    num? thisMonthInviteResponsePercent,
    ReviewOverviewTopStaff? topStaff,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReviewOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReviewOverview.copyWith.fieldName(...)`
class _$ReviewOverviewCWProxyImpl implements _$ReviewOverviewCWProxy {
  const _$ReviewOverviewCWProxyImpl(this._value);

  final ReviewOverview _value;

  @override
  ReviewOverview averageRating(num? averageRating) =>
      this(averageRating: averageRating);

  @override
  ReviewOverview ratingCount(int ratingCount) => this(ratingCount: ratingCount);

  @override
  ReviewOverview totalCount(int totalCount) => this(totalCount: totalCount);

  @override
  ReviewOverview awaitingReplyCount(int awaitingReplyCount) =>
      this(awaitingReplyCount: awaitingReplyCount);

  @override
  ReviewOverview thisMonthCount(int thisMonthCount) =>
      this(thisMonthCount: thisMonthCount);

  @override
  ReviewOverview thisMonthInviteResponsePercent(
    num? thisMonthInviteResponsePercent,
  ) => this(thisMonthInviteResponsePercent: thisMonthInviteResponsePercent);

  @override
  ReviewOverview topStaff(ReviewOverviewTopStaff? topStaff) =>
      this(topStaff: topStaff);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewOverview call({
    Object? averageRating = const $CopyWithPlaceholder(),
    Object? ratingCount = const $CopyWithPlaceholder(),
    Object? totalCount = const $CopyWithPlaceholder(),
    Object? awaitingReplyCount = const $CopyWithPlaceholder(),
    Object? thisMonthCount = const $CopyWithPlaceholder(),
    Object? thisMonthInviteResponsePercent = const $CopyWithPlaceholder(),
    Object? topStaff = const $CopyWithPlaceholder(),
  }) {
    return ReviewOverview(
      averageRating: averageRating == const $CopyWithPlaceholder()
          ? _value.averageRating
          // ignore: cast_nullable_to_non_nullable
          : averageRating as num?,
      ratingCount: ratingCount == const $CopyWithPlaceholder()
          ? _value.ratingCount
          // ignore: cast_nullable_to_non_nullable
          : ratingCount as int,
      totalCount: totalCount == const $CopyWithPlaceholder()
          ? _value.totalCount
          // ignore: cast_nullable_to_non_nullable
          : totalCount as int,
      awaitingReplyCount: awaitingReplyCount == const $CopyWithPlaceholder()
          ? _value.awaitingReplyCount
          // ignore: cast_nullable_to_non_nullable
          : awaitingReplyCount as int,
      thisMonthCount: thisMonthCount == const $CopyWithPlaceholder()
          ? _value.thisMonthCount
          // ignore: cast_nullable_to_non_nullable
          : thisMonthCount as int,
      thisMonthInviteResponsePercent:
          thisMonthInviteResponsePercent == const $CopyWithPlaceholder()
          ? _value.thisMonthInviteResponsePercent
          // ignore: cast_nullable_to_non_nullable
          : thisMonthInviteResponsePercent as num?,
      topStaff: topStaff == const $CopyWithPlaceholder()
          ? _value.topStaff
          // ignore: cast_nullable_to_non_nullable
          : topStaff as ReviewOverviewTopStaff?,
    );
  }
}

extension $ReviewOverviewCopyWith on ReviewOverview {
  /// Returns a callable class that can be used as follows: `instanceOfReviewOverview.copyWith(...)` or like so:`instanceOfReviewOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReviewOverviewCWProxy get copyWith => _$ReviewOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReviewOverview _$ReviewOverviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReviewOverview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'averageRating',
          'ratingCount',
          'totalCount',
          'awaitingReplyCount',
          'thisMonthCount',
        ],
      );
      final val = ReviewOverview(
        averageRating: $checkedConvert('averageRating', (v) => v as num?),
        ratingCount: $checkedConvert('ratingCount', (v) => (v as num).toInt()),
        totalCount: $checkedConvert('totalCount', (v) => (v as num).toInt()),
        awaitingReplyCount: $checkedConvert(
          'awaitingReplyCount',
          (v) => (v as num).toInt(),
        ),
        thisMonthCount: $checkedConvert(
          'thisMonthCount',
          (v) => (v as num).toInt(),
        ),
        thisMonthInviteResponsePercent: $checkedConvert(
          'thisMonthInviteResponsePercent',
          (v) => v as num?,
        ),
        topStaff: $checkedConvert(
          'topStaff',
          (v) => v == null
              ? null
              : ReviewOverviewTopStaff.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ReviewOverviewToJson(
  ReviewOverview instance,
) => <String, dynamic>{
  'averageRating': instance.averageRating,
  'ratingCount': instance.ratingCount,
  'totalCount': instance.totalCount,
  'awaitingReplyCount': instance.awaitingReplyCount,
  'thisMonthCount': instance.thisMonthCount,
  'thisMonthInviteResponsePercent': ?instance.thisMonthInviteResponsePercent,
  'topStaff': ?instance.topStaff?.toJson(),
};
