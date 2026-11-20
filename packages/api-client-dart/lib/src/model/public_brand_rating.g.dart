// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_brand_rating.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicBrandRatingCWProxy {
  PublicBrandRating average(num average);

  PublicBrandRating count(int count);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicBrandRating(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicBrandRating(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicBrandRating call({num average, int count});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicBrandRating.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicBrandRating.copyWith.fieldName(...)`
class _$PublicBrandRatingCWProxyImpl implements _$PublicBrandRatingCWProxy {
  const _$PublicBrandRatingCWProxyImpl(this._value);

  final PublicBrandRating _value;

  @override
  PublicBrandRating average(num average) => this(average: average);

  @override
  PublicBrandRating count(int count) => this(count: count);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicBrandRating(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicBrandRating(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicBrandRating call({
    Object? average = const $CopyWithPlaceholder(),
    Object? count = const $CopyWithPlaceholder(),
  }) {
    return PublicBrandRating(
      average: average == const $CopyWithPlaceholder()
          ? _value.average
          // ignore: cast_nullable_to_non_nullable
          : average as num,
      count: count == const $CopyWithPlaceholder()
          ? _value.count
          // ignore: cast_nullable_to_non_nullable
          : count as int,
    );
  }
}

extension $PublicBrandRatingCopyWith on PublicBrandRating {
  /// Returns a callable class that can be used as follows: `instanceOfPublicBrandRating.copyWith(...)` or like so:`instanceOfPublicBrandRating.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicBrandRatingCWProxy get copyWith =>
      _$PublicBrandRatingCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicBrandRating _$PublicBrandRatingFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicBrandRating', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['average', 'count']);
      final val = PublicBrandRating(
        average: $checkedConvert('average', (v) => v as num),
        count: $checkedConvert('count', (v) => (v as num).toInt()),
      );
      return val;
    });

Map<String, dynamic> _$PublicBrandRatingToJson(PublicBrandRating instance) =>
    <String, dynamic>{'average': instance.average, 'count': instance.count};
