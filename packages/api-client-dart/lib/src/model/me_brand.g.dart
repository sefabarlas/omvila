// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_brand.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeBrandCWProxy {
  MeBrand id(String id);

  MeBrand name(String name);

  MeBrand slug(String slug);

  MeBrand logoUrl(String? logoUrl);

  MeBrand coverColor(MeBrandCoverColorEnum coverColor);

  MeBrand bookingEnabled(bool bookingEnabled);

  MeBrand hasBooked(bool hasBooked);

  MeBrand savedUtc(DateTime? savedUtc);

  MeBrand lastVisitOn(DateTime? lastVisitOn);

  MeBrand branches(List<MeBranchRef> branches);

  MeBrand activePackages(List<MeBrandPackageBadge> activePackages);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrand call({
    String id,
    String name,
    String slug,
    String? logoUrl,
    MeBrandCoverColorEnum coverColor,
    bool bookingEnabled,
    bool hasBooked,
    DateTime? savedUtc,
    DateTime? lastVisitOn,
    List<MeBranchRef> branches,
    List<MeBrandPackageBadge> activePackages,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeBrand.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeBrand.copyWith.fieldName(...)`
class _$MeBrandCWProxyImpl implements _$MeBrandCWProxy {
  const _$MeBrandCWProxyImpl(this._value);

  final MeBrand _value;

  @override
  MeBrand id(String id) => this(id: id);

  @override
  MeBrand name(String name) => this(name: name);

  @override
  MeBrand slug(String slug) => this(slug: slug);

  @override
  MeBrand logoUrl(String? logoUrl) => this(logoUrl: logoUrl);

  @override
  MeBrand coverColor(MeBrandCoverColorEnum coverColor) =>
      this(coverColor: coverColor);

  @override
  MeBrand bookingEnabled(bool bookingEnabled) =>
      this(bookingEnabled: bookingEnabled);

  @override
  MeBrand hasBooked(bool hasBooked) => this(hasBooked: hasBooked);

  @override
  MeBrand savedUtc(DateTime? savedUtc) => this(savedUtc: savedUtc);

  @override
  MeBrand lastVisitOn(DateTime? lastVisitOn) => this(lastVisitOn: lastVisitOn);

  @override
  MeBrand branches(List<MeBranchRef> branches) => this(branches: branches);

  @override
  MeBrand activePackages(List<MeBrandPackageBadge> activePackages) =>
      this(activePackages: activePackages);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeBrand(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeBrand(...).copyWith(id: 12, name: "My name")
  /// ````
  MeBrand call({
    Object? id = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? slug = const $CopyWithPlaceholder(),
    Object? logoUrl = const $CopyWithPlaceholder(),
    Object? coverColor = const $CopyWithPlaceholder(),
    Object? bookingEnabled = const $CopyWithPlaceholder(),
    Object? hasBooked = const $CopyWithPlaceholder(),
    Object? savedUtc = const $CopyWithPlaceholder(),
    Object? lastVisitOn = const $CopyWithPlaceholder(),
    Object? branches = const $CopyWithPlaceholder(),
    Object? activePackages = const $CopyWithPlaceholder(),
  }) {
    return MeBrand(
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
      logoUrl: logoUrl == const $CopyWithPlaceholder()
          ? _value.logoUrl
          // ignore: cast_nullable_to_non_nullable
          : logoUrl as String?,
      coverColor: coverColor == const $CopyWithPlaceholder()
          ? _value.coverColor
          // ignore: cast_nullable_to_non_nullable
          : coverColor as MeBrandCoverColorEnum,
      bookingEnabled: bookingEnabled == const $CopyWithPlaceholder()
          ? _value.bookingEnabled
          // ignore: cast_nullable_to_non_nullable
          : bookingEnabled as bool,
      hasBooked: hasBooked == const $CopyWithPlaceholder()
          ? _value.hasBooked
          // ignore: cast_nullable_to_non_nullable
          : hasBooked as bool,
      savedUtc: savedUtc == const $CopyWithPlaceholder()
          ? _value.savedUtc
          // ignore: cast_nullable_to_non_nullable
          : savedUtc as DateTime?,
      lastVisitOn: lastVisitOn == const $CopyWithPlaceholder()
          ? _value.lastVisitOn
          // ignore: cast_nullable_to_non_nullable
          : lastVisitOn as DateTime?,
      branches: branches == const $CopyWithPlaceholder()
          ? _value.branches
          // ignore: cast_nullable_to_non_nullable
          : branches as List<MeBranchRef>,
      activePackages: activePackages == const $CopyWithPlaceholder()
          ? _value.activePackages
          // ignore: cast_nullable_to_non_nullable
          : activePackages as List<MeBrandPackageBadge>,
    );
  }
}

extension $MeBrandCopyWith on MeBrand {
  /// Returns a callable class that can be used as follows: `instanceOfMeBrand.copyWith(...)` or like so:`instanceOfMeBrand.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeBrandCWProxy get copyWith => _$MeBrandCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeBrand _$MeBrandFromJson(Map<String, dynamic> json) => $checkedCreate(
  'MeBrand',
  json,
  ($checkedConvert) {
    $checkKeys(
      json,
      requiredKeys: const [
        'id',
        'name',
        'slug',
        'logoUrl',
        'coverColor',
        'bookingEnabled',
        'hasBooked',
        'savedUtc',
        'lastVisitOn',
        'branches',
        'activePackages',
      ],
    );
    final val = MeBrand(
      id: $checkedConvert('id', (v) => v as String),
      name: $checkedConvert('name', (v) => v as String),
      slug: $checkedConvert('slug', (v) => v as String),
      logoUrl: $checkedConvert('logoUrl', (v) => v as String?),
      coverColor: $checkedConvert(
        'coverColor',
        (v) => $enumDecode(
          _$MeBrandCoverColorEnumEnumMap,
          v,
          unknownValue: MeBrandCoverColorEnum.unknownDefaultOpenApi,
        ),
      ),
      bookingEnabled: $checkedConvert('bookingEnabled', (v) => v as bool),
      hasBooked: $checkedConvert('hasBooked', (v) => v as bool),
      savedUtc: $checkedConvert(
        'savedUtc',
        (v) => v == null ? null : DateTime.parse(v as String),
      ),
      lastVisitOn: $checkedConvert(
        'lastVisitOn',
        (v) => v == null ? null : DateTime.parse(v as String),
      ),
      branches: $checkedConvert(
        'branches',
        (v) => (v as List<dynamic>)
            .map((e) => MeBranchRef.fromJson(e as Map<String, dynamic>))
            .toList(),
      ),
      activePackages: $checkedConvert(
        'activePackages',
        (v) => (v as List<dynamic>)
            .map((e) => MeBrandPackageBadge.fromJson(e as Map<String, dynamic>))
            .toList(),
      ),
    );
    return val;
  },
);

Map<String, dynamic> _$MeBrandToJson(MeBrand instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'slug': instance.slug,
  'logoUrl': instance.logoUrl,
  'coverColor': _$MeBrandCoverColorEnumEnumMap[instance.coverColor]!,
  'bookingEnabled': instance.bookingEnabled,
  'hasBooked': instance.hasBooked,
  'savedUtc': instance.savedUtc?.toIso8601String(),
  'lastVisitOn': instance.lastVisitOn?.toIso8601String(),
  'branches': instance.branches.map((e) => e.toJson()).toList(),
  'activePackages': instance.activePackages.map((e) => e.toJson()).toList(),
};

const _$MeBrandCoverColorEnumEnumMap = {
  MeBrandCoverColorEnum.petrol: 'petrol',
  MeBrandCoverColorEnum.mercan: 'mercan',
  MeBrandCoverColorEnum.kum: 'kum',
  MeBrandCoverColorEnum.unknownDefaultOpenApi: 'unknown_default_open_api',
};
