// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'booking_token_result.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BookingTokenResultCWProxy {
  BookingTokenResult bookingToken(String bookingToken);

  BookingTokenResult expiresInSeconds(int expiresInSeconds);

  BookingTokenResult phoneMasked(String phoneMasked);

  BookingTokenResult knownCustomerFirstName(String? knownCustomerFirstName);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BookingTokenResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BookingTokenResult(...).copyWith(id: 12, name: "My name")
  /// ````
  BookingTokenResult call({
    String bookingToken,
    int expiresInSeconds,
    String phoneMasked,
    String? knownCustomerFirstName,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBookingTokenResult.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBookingTokenResult.copyWith.fieldName(...)`
class _$BookingTokenResultCWProxyImpl implements _$BookingTokenResultCWProxy {
  const _$BookingTokenResultCWProxyImpl(this._value);

  final BookingTokenResult _value;

  @override
  BookingTokenResult bookingToken(String bookingToken) =>
      this(bookingToken: bookingToken);

  @override
  BookingTokenResult expiresInSeconds(int expiresInSeconds) =>
      this(expiresInSeconds: expiresInSeconds);

  @override
  BookingTokenResult phoneMasked(String phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  BookingTokenResult knownCustomerFirstName(String? knownCustomerFirstName) =>
      this(knownCustomerFirstName: knownCustomerFirstName);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BookingTokenResult(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BookingTokenResult(...).copyWith(id: 12, name: "My name")
  /// ````
  BookingTokenResult call({
    Object? bookingToken = const $CopyWithPlaceholder(),
    Object? expiresInSeconds = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
    Object? knownCustomerFirstName = const $CopyWithPlaceholder(),
  }) {
    return BookingTokenResult(
      bookingToken: bookingToken == const $CopyWithPlaceholder()
          ? _value.bookingToken
          // ignore: cast_nullable_to_non_nullable
          : bookingToken as String,
      expiresInSeconds: expiresInSeconds == const $CopyWithPlaceholder()
          ? _value.expiresInSeconds
          // ignore: cast_nullable_to_non_nullable
          : expiresInSeconds as int,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String,
      knownCustomerFirstName:
          knownCustomerFirstName == const $CopyWithPlaceholder()
          ? _value.knownCustomerFirstName
          // ignore: cast_nullable_to_non_nullable
          : knownCustomerFirstName as String?,
    );
  }
}

extension $BookingTokenResultCopyWith on BookingTokenResult {
  /// Returns a callable class that can be used as follows: `instanceOfBookingTokenResult.copyWith(...)` or like so:`instanceOfBookingTokenResult.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BookingTokenResultCWProxy get copyWith =>
      _$BookingTokenResultCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BookingTokenResult _$BookingTokenResultFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BookingTokenResult', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['bookingToken', 'expiresInSeconds', 'phoneMasked'],
      );
      final val = BookingTokenResult(
        bookingToken: $checkedConvert('bookingToken', (v) => v as String),
        expiresInSeconds: $checkedConvert(
          'expiresInSeconds',
          (v) => (v as num).toInt(),
        ),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String),
        knownCustomerFirstName: $checkedConvert(
          'knownCustomerFirstName',
          (v) => v as String?,
        ),
      );
      return val;
    });

Map<String, dynamic> _$BookingTokenResultToJson(BookingTokenResult instance) =>
    <String, dynamic>{
      'bookingToken': instance.bookingToken,
      'expiresInSeconds': instance.expiresInSeconds,
      'phoneMasked': instance.phoneMasked,
      'knownCustomerFirstName': ?instance.knownCustomerFirstName,
    };
