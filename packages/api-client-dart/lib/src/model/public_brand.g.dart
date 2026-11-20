// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicBrandCWProxy {
  PublicBrand id(String id);

  PublicBrand name(String name);

  PublicBrand slug(String slug);

  PublicBrand shortDescription(String? shortDescription);

  PublicBrand logoUrl(String? logoUrl);

  PublicBrand industryLabel(String? industryLabel);

  PublicBrand coverColor(PublicBrandCoverColorEnum coverColor);

  PublicBrand showPrices(bool showPrices);

  PublicBrand bookingEnabled(bool bookingEnabled);

  PublicBrand rating(PublicBrandRating? rating);

  PublicBrand branches(List<PublicBrandBranchesInner> branches);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicBrand call({
    String id,
    String name,
    String slug,
    String? shortDescription,
    String? logoUrl,
    String? industryLabel,
    PublicBrandCoverColorEnum coverColor,
    bool showPrices,
    bool bookingEnabled,
    PublicBrandRating? rating,
    List<PublicBrandBranchesInner> branches,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicBrand.copyWith.fieldName(...)`
class _$PublicBrandCWProxyImpl implements _$PublicBrandCWProxy {
  const _$PublicBrandCWProxyImpl(this._value);

  final PublicBrand _value;

  @override
  PublicBrand id(String id) => this(id: id);

  @override
  PublicBrand name(String name) => this(name: name);

  @override
  PublicBrand slug(String slug) => this(slug: slug);

  @override
  PublicBrand shortDescription(String? shortDescription) =>
      this(shortDescription: shortDescription);

  @override
  PublicBrand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  PublicBrand industryLabel(String? industryLabel) =>
      this(industryLabel: industryLabel);

  @override
  PublicBrand coverColor(PublicBrandCoverColorEnum coverColor) =>
      this(coverColor: coverColor);

  @override
  PublicBrand showPrices(bool showPrices) => this(showPrices: showPrices);

  @override
  PublicBrand bookingEnabled(bool bookingEnabled) =>
      this(bookingEnabled: bookingEnabled);

  @override
  PublicBrand rating(PublicBrandRating? rating) => this(rating: rating);

  @override
  PublicBrand branches(List<PublicBrandBranchesInner> branches) =>
      this(branches: branches);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicBrand call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? shortDescription = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
    Object? industryLabel = const $CopyWithPlaceholder(),
    Object? coverColor = const $CopyWithPlaceholder(),
    Object? showPrices = const $CopyWithPlaceholder(),
    Object? bookingEnabled = const $CopyWithPlaceholder(),
    Object? rating = const $CopyWithPlaceholder(),
    Object? branches = const $CopyWithPlaceholder(),
  }) {
    return PublicBrand(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      slug: slug == const $CopyWithPlaceholder()
          ? _value.slug
          // ignore: cast_nullable_to_non_nullable
          : slug as String,
      shortDescription: shortDescription == const $CopyWithPlaceholder()
          ? _value.shortDescription
          // ignore: cast_nullable_to_non_nullable
          : shortDescription as String?,
      logoUrl: logoUrl == const $CopyWithPlaceholder()
          ? _value.logoUrl
          // ignore: cast_nullable_to_non_nullable
          : logoUrl as String?,
      industryLabel: industryLabel == const $CopyWithPlaceholder()
          ? _value.industryLabel
          // ignore: cast_nullable_to_non_nullable
          : industryLabel as String?,
      coverColor: coverColor == const $CopyWithPlaceholder()
          ? _value.coverColor
          // ignore: cast_nullable_to_non_nullable
          : coverColor as PublicBrandCoverColorEnum,
      showPrices: showPrices == const $CopyWithPlaceholder()
          ? _value.showPrices
          // ignore: cast_nullable_to_non_nullable
          : showPrices as bool,
      bookingEnabled: bookingEnabled == const $CopyWithPlaceholder()
          ? _value.bookingEnabled
          // ignore: cast_nullable_to_non_nullable
          : bookingEnabled as bool,
      rating: rating == const $CopyWithPlaceholder()
          ? _value.rating
          // ignore: cast_nullable_to_non_nullable
          : rating as PublicBrandRating?,
      branches: branches == const $CopyWithPlaceholder()
          ? _value.branches
          // ignore: cast_nullable_to_non_nullable
          : branches as List<PublicBrandBranchesInner>,
    );
  }
}

extension $PublicBrandCopyWith on PublicBrand {
  /// Returns a callable class that can be used as follows: `instanceOfPublicBrand.copyWith(...)` or like so:`instanceOfPublicBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicBrandCWProxy get copyWith => _$PublicBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicBrand _$PublicBrandFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicBrand', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'name',
      'slug',
      'coverColor',
      'showPrices',
      'bookingEnabled',
      'branches',
    ],
  );
  final val = PublicBrand(
    id: $checkedConvert('id', (v) => v as String),
    name: $checkedConvert('name', (v) => v as String),
    slug: $checkedConvert('slug', (v) => v as String),
    shortDescription: $checkedConvert('shortDescription', (v) => v as String?),
    logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
    industryLabel: $checkedConvert('industryLabel', (v) => v as String?),
    coverColor: $checkedConvert(
      'coverColor',
      (v) => $enumDecode(
        _$PublicBrandCoverColorEnumEnumMap,
        v,
        unknownValue: PublicBrandCoverColorEnum.unknownDefaultOpenApi,
      ),
    ),
    showPrices: $checkedConvert('showPrices', (v) => v as bool),
    bookingEnabled: $checkedConvert('bookingEnabled', (v) => v as bool),
    rating: $checkedConvert(
      'rating',
      (v) => v == null
          ? null
          : PublicBrandRating.fromJson(v as Map<String, dynamic>),
    ),
    branches: $checkedConvert(
      'branches',
      (v) => (v as List<dynamic>)
          .map(
            (e) => PublicBrandBranchesInner.fromJson(e as Map<String, dynamic>),
          )
          .toList(),
    ),
  );
  return val;
});

Map<String, dynamic> _$PublicBrandToJson(PublicBrand instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'slug': instance.slug,
      'shortDescription': ?instance.shortDescription,
      'logoUrl': ?instance.logoUrl,
      'industryLabel': ?instance.industryLabel,
      'coverColor': _$PublicBrandCoverColorEnumEnumMap[instance.coverColor]!,
      'showPrices': instance.showPrices,
      'bookingEnabled': instance.bookingEnabled,
      'rating': ?instance.rating?.toJson(),
      'branches': instance.branches.map((e) => e.toJson()).toList(),
    };

const _$PublicBrandCoverColorEnumEnumMap = {
  PublicBrandCoverColorEnum.petrol: 'petrol',
  PublicBrandCoverColorEnum.mercan: 'mercan',
  PublicBrandCoverColorEnum.kum: 'kum',
  PublicBrandCoverColorEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
