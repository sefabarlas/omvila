// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'booking_page_settings.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BookingPageSettingsCWProxy {
  BookingPageSettings slug(String slug);

  BookingPageSettings url(String url);

  BookingPageSettings coverColor(BookingPageSettingsCoverColorEnum coverColor);

  BookingPageSettings showReviews(bool showReviews);

  BookingPageSettings showPrices(bool showPrices);

  BookingPageSettings preAppointmentNote(String? preAppointmentNote);

  BookingPageSettings version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BookingPageSettings(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BookingPageSettings(...).copyWith(id: 12, name: "My name")
  /// ````
  BookingPageSettings call({
    String slug,
    String url,
    BookingPageSettingsCoverColorEnum coverColor,
    bool showReviews,
    bool showPrices,
    String? preAppointmentNote,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBookingPageSettings.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBookingPageSettings.copyWith.fieldName(...)`
class _$BookingPageSettingsCWProxyImpl implements _$BookingPageSettingsCWProxy {
  const _$BookingPageSettingsCWProxyImpl(this._value);

  final BookingPageSettings _value;

  @override
  BookingPageSettings slug(String slug) => this(slug: slug);

  @override
  BookingPageSettings url(String url) => this(url: url);

  @override
  BookingPageSettings coverColor(
    BookingPageSettingsCoverColorEnum coverColor,
  ) => this(coverColor: coverColor);

  @override
  BookingPageSettings showReviews(bool showReviews) =>
      this(showReviews: showReviews);

  @override
  BookingPageSettings showPrices(bool showPrices) =>
      this(showPrices: showPrices);

  @override
  BookingPageSettings preAppointmentNote(String? preAppointmentNote) =>
      this(preAppointmentNote: preAppointmentNote);

  @override
  BookingPageSettings version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BookingPageSettings(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BookingPageSettings(...).copyWith(id: 12, name: "My name")
  /// ````
  BookingPageSettings call({
    Object? slug = const $CopyWithPlaceholder(),
    Object? url = const $CopyWithPlaceholder(),
    Object? coverColor = const $CopyWithPlaceholder(),
    Object? showReviews = const $CopyWithPlaceholder(),
    Object? showPrices = const $CopyWithPlaceholder(),
    Object? preAppointmentNote = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return BookingPageSettings(
      slug: slug == const $CopyWithPlaceholder()
          ? _value.slug
          // ignore: cast_nullable_to_non_nullable
          : slug as String,
      url: url == const $CopyWithPlaceholder()
          ? _value.url
          // ignore: cast_nullable_to_non_nullable
          : url as String,
      coverColor: coverColor == const $CopyWithPlaceholder()
          ? _value.coverColor
          // ignore: cast_nullable_to_non_nullable
          : coverColor as BookingPageSettingsCoverColorEnum,
      showReviews: showReviews == const $CopyWithPlaceholder()
          ? _value.showReviews
          // ignore: cast_nullable_to_non_nullable
          : showReviews as bool,
      showPrices: showPrices == const $CopyWithPlaceholder()
          ? _value.showPrices
          // ignore: cast_nullable_to_non_nullable
          : showPrices as bool,
      preAppointmentNote: preAppointmentNote == const $CopyWithPlaceholder()
          ? _value.preAppointmentNote
          // ignore: cast_nullable_to_non_nullable
          : preAppointmentNote as String?,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $BookingPageSettingsCopyWith on BookingPageSettings {
  /// Returns a callable class that can be used as follows: `instanceOfBookingPageSettings.copyWith(...)` or like so:`instanceOfBookingPageSettings.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BookingPageSettingsCWProxy get copyWith =>
      _$BookingPageSettingsCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BookingPageSettings _$BookingPageSettingsFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BookingPageSettings', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'slug',
          'url',
          'coverColor',
          'showReviews',
          'showPrices',
          'version',
        ],
      );
      final val = BookingPageSettings(
        slug: $checkedConvert('slug', (v) => v as String),
        url: $checkedConvert('url', (v) => v as String),
        coverColor: $checkedConvert(
          'coverColor',
          (v) => $enumDecode(
            _$BookingPageSettingsCoverColorEnumEnumMap,
            v,
            unknownValue:
                BookingPageSettingsCoverColorEnum.unknownDefaultOpenApi,
          ),
        ),
        showReviews: $checkedConvert('showReviews', (v) => v as bool),
        showPrices: $checkedConvert('showPrices', (v) => v as bool),
        preAppointmentNote: $checkedConvert(
          'preAppointmentNote',
          (v) => v as String?,
        ),
        version: $checkedConvert('version', (v) => v as String),
      );
      return val;
    });

Map<String, dynamic> _$BookingPageSettingsToJson(
  BookingPageSettings instance,
) => <String, dynamic>{
  'slug': instance.slug,
  'url': instance.url,
  'coverColor':
      _$BookingPageSettingsCoverColorEnumEnumMap[instance.coverColor]!,
  'showReviews': instance.showReviews,
  'showPrices': instance.showPrices,
  'preAppointmentNote': ?instance.preAppointmentNote,
  'version': instance.version,
};

const _$BookingPageSettingsCoverColorEnumEnumMap = {
  BookingPageSettingsCoverColorEnum.petrol: 'petrol',
  BookingPageSettingsCoverColorEnum.mercan: 'mercan',
  BookingPageSettingsCoverColorEnum.kum: 'kum',
  BookingPageSettingsCoverColorEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
