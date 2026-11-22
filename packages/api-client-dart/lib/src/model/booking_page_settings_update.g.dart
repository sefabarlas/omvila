// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'booking_page_settings_update.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BookingPageSettingsUpdateCWProxy {
  BookingPageSettingsUpdate slug(String slug);

  BookingPageSettingsUpdate coverColor(
    BookingPageSettingsUpdateCoverColorEnum coverColor,
  );

  BookingPageSettingsUpdate showReviews(bool showReviews);

  BookingPageSettingsUpdate showPrices(bool showPrices);

  BookingPageSettingsUpdate preAppointmentNote(String? preAppointmentNote);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BookingPageSettingsUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BookingPageSettingsUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  BookingPageSettingsUpdate call({
    String slug,
    BookingPageSettingsUpdateCoverColorEnum coverColor,
    bool showReviews,
    bool showPrices,
    String? preAppointmentNote,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBookingPageSettingsUpdate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBookingPageSettingsUpdate.copyWith.fieldName(...)`
class _$BookingPageSettingsUpdateCWProxyImpl
    implements _$BookingPageSettingsUpdateCWProxy {
  const _$BookingPageSettingsUpdateCWProxyImpl(this._value);

  final BookingPageSettingsUpdate _value;

  @override
  BookingPageSettingsUpdate slug(String slug) => this(slug: slug);

  @override
  BookingPageSettingsUpdate coverColor(
    BookingPageSettingsUpdateCoverColorEnum coverColor,
  ) => this(coverColor: coverColor);

  @override
  BookingPageSettingsUpdate showReviews(bool showReviews) =>
      this(showReviews: showReviews);

  @override
  BookingPageSettingsUpdate showPrices(bool showPrices) =>
      this(showPrices: showPrices);

  @override
  BookingPageSettingsUpdate preAppointmentNote(String? preAppointmentNote) =>
      this(preAppointmentNote: preAppointmentNote);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BookingPageSettingsUpdate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BookingPageSettingsUpdate(...).copyWith(id: 12, name: "My name")
  /// ````
  BookingPageSettingsUpdate call({
    Object? slug = const $CopyWithPlaceholder(),
    Object? coverColor = const $CopyWithPlaceholder(),
    Object? showReviews = const $CopyWithPlaceholder(),
    Object? showPrices = const $CopyWithPlaceholder(),
    Object? preAppointmentNote = const $CopyWithPlaceholder(),
  }) {
    return BookingPageSettingsUpdate(
      slug: slug == const $CopyWithPlaceholder()
          ? _value.slug
          // ignore: cast_nullable_to_non_nullable
          : slug as String,
      coverColor: coverColor == const $CopyWithPlaceholder()
          ? _value.coverColor
          // ignore: cast_nullable_to_non_nullable
          : coverColor as BookingPageSettingsUpdateCoverColorEnum,
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
    );
  }
}

extension $BookingPageSettingsUpdateCopyWith on BookingPageSettingsUpdate {
  /// Returns a callable class that can be used as follows: `instanceOfBookingPageSettingsUpdate.copyWith(...)` or like so:`instanceOfBookingPageSettingsUpdate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BookingPageSettingsUpdateCWProxy get copyWith =>
      _$BookingPageSettingsUpdateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BookingPageSettingsUpdate _$BookingPageSettingsUpdateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('BookingPageSettingsUpdate', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const ['slug', 'coverColor', 'showReviews', 'showPrices'],
  );
  final val = BookingPageSettingsUpdate(
    slug: $checkedConvert('slug', (v) => v as String),
    coverColor: $checkedConvert(
      'coverColor',
      (v) => $enumDecode(
        _$BookingPageSettingsUpdateCoverColorEnumEnumMap,
        v,
        unknownValue:
            BookingPageSettingsUpdateCoverColorEnum.unknownDefaultOpenApi,
      ),
    ),
    showReviews: $checkedConvert('showReviews', (v) => v as bool),
    showPrices: $checkedConvert('showPrices', (v) => v as bool),
    preAppointmentNote: $checkedConvert(
      'preAppointmentNote',
      (v) => v as String?,
    ),
  );
  return val;
});

Map<String, dynamic> _$BookingPageSettingsUpdateToJson(
  BookingPageSettingsUpdate instance,
) => <String, dynamic>{
  'slug': instance.slug,
  'coverColor':
      _$BookingPageSettingsUpdateCoverColorEnumEnumMap[instance.coverColor]!,
  'showReviews': instance.showReviews,
  'showPrices': instance.showPrices,
  'preAppointmentNote': instance.preAppointmentNote,
};

const _$BookingPageSettingsUpdateCoverColorEnumEnumMap = {
  BookingPageSettingsUpdateCoverColorEnum.petrol: 'petrol',
  BookingPageSettingsUpdateCoverColorEnum.mercan: 'mercan',
  BookingPageSettingsUpdateCoverColorEnum.kum: 'kum',
  BookingPageSettingsUpdateCoverColorEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
