// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'review_page.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$ReviewPageCWProxy {
  ReviewPage items(List<ReviewView> items);

  ReviewPage nextCursor(String? nextCursor);

  ReviewPage overview(ReviewOverview overview);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewPage(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewPage call({
    List<ReviewView> items,
    String? nextCursor,
    ReviewOverview overview,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfReviewPage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfReviewPage.copyWith.fieldName(...)`
class _$ReviewPageCWProxyImpl implements _$ReviewPageCWProxy {
  const _$ReviewPageCWProxyImpl(this._value);

  final ReviewPage _value;

  @override
  ReviewPage items(List<ReviewView> items) => this(items: items);

  @override
  ReviewPage nextCursor(String? nextCursor) => this(nextCursor: nextCursor);

  @override
  ReviewPage overview(ReviewOverview overview) => this(overview: overview);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `ReviewPage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// ReviewPage(...).copyWith(id: 12, name: "My name")
  /// ````
  ReviewPage call({
    Object? items = const $CopyWithPlaceholder(),
    Object? nextCursor = const $CopyWithPlaceholder(),
    Object? overview = const $CopyWithPlaceholder(),
  }) {
    return ReviewPage(
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<ReviewView>,
      nextCursor: nextCursor == const $CopyWithPlaceholder()
          ? _value.nextCursor
          // ignore: cast_nullable_to_non_nullable
          : nextCursor as String?,
      overview: overview == const $CopyWithPlaceholder()
          ? _value.overview
          // ignore: cast_nullable_to_non_nullable
          : overview as ReviewOverview,
    );
  }
}

extension $ReviewPageCopyWith on ReviewPage {
  /// Returns a callable class that can be used as follows: `instanceOfReviewPage.copyWith(...)` or like so:`instanceOfReviewPage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$ReviewPageCWProxy get copyWith => _$ReviewPageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ReviewPage _$ReviewPageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('ReviewPage', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['items', 'nextCursor', 'overview']);
      final val = ReviewPage(
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map((e) => ReviewView.fromJson(e as Map<String, dynamic>))
              .toList(),
        ),
        nextCursor: $checkedConvert('nextCursor', (v) => v as String?),
        overview: $checkedConvert(
          'overview',
          (v) => ReviewOverview.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$ReviewPageToJson(ReviewPage instance) =>
    <String, dynamic>{
      'items': instance.items.map((e) => e.toJson()).toList(),
      'nextCursor': instance.nextCursor,
      'overview': instance.overview.toJson(),
    };
