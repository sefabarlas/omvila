// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_day_closing.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CashDayClosingCWProxy {
  CashDayClosing expectedCash(Money expectedCash);

  CashDayClosing countedCash(Money countedCash);

  CashDayClosing differenceMinor(int differenceMinor);

  CashDayClosing difference(CashDayClosingDifferenceEnum difference);

  CashDayClosing differenceNote(String? differenceNote);

  CashDayClosing closedByName(String? closedByName);

  CashDayClosing closedUtc(DateTime closedUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDayClosing(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDayClosing(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDayClosing call({
    Money expectedCash,
    Money countedCash,
    int differenceMinor,
    CashDayClosingDifferenceEnum difference,
    String? differenceNote,
    String? closedByName,
    DateTime closedUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCashDayClosing.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCashDayClosing.copyWith.fieldName(...)`
class _$CashDayClosingCWProxyImpl implements _$CashDayClosingCWProxy {
  const _$CashDayClosingCWProxyImpl(this._value);

  final CashDayClosing _value;

  @override
  CashDayClosing expectedCash(Money expectedCash) =>
      this(expectedCash: expectedCash);

  @override
  CashDayClosing countedCash(Money countedCash) =>
      this(countedCash: countedCash);

  @override
  CashDayClosing differenceMinor(int differenceMinor) =>
      this(differenceMinor: differenceMinor);

  @override
  CashDayClosing difference(CashDayClosingDifferenceEnum difference) =>
      this(difference: difference);

  @override
  CashDayClosing differenceNote(String? differenceNote) =>
      this(differenceNote: differenceNote);

  @override
  CashDayClosing closedByName(String? closedByName) =>
      this(closedByName: closedByName);

  @override
  CashDayClosing closedUtc(DateTime closedUtc) => this(closedUtc: closedUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDayClosing(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDayClosing(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDayClosing call({
    Object? expectedCash = const $CopyWithPlaceholder(),
    Object? countedCash = const $CopyWithPlaceholder(),
    Object? differenceMinor = const $CopyWithPlaceholder(),
    Object? difference = const $CopyWithPlaceholder(),
    Object? differenceNote = const $CopyWithPlaceholder(),
    Object? closedByName = const $CopyWithPlaceholder(),
    Object? closedUtc = const $CopyWithPlaceholder(),
  }) {
    return CashDayClosing(
      expectedCash: expectedCash == const $CopyWithPlaceholder()
          ? _value.expectedCash
          // ignore: cast_nullable_to_non_nullable
          : expectedCash as Money,
      countedCash: countedCash == const $CopyWithPlaceholder()
          ? _value.countedCash
          // ignore: cast_nullable_to_non_nullable
          : countedCash as Money,
      differenceMinor: differenceMinor == const $CopyWithPlaceholder()
          ? _value.differenceMinor
          // ignore: cast_nullable_to_non_nullable
          : differenceMinor as int,
      difference: difference == const $CopyWithPlaceholder()
          ? _value.difference
          // ignore: cast_nullable_to_non_nullable
          : difference as CashDayClosingDifferenceEnum,
      differenceNote: differenceNote == const $CopyWithPlaceholder()
          ? _value.differenceNote
          // ignore: cast_nullable_to_non_nullable
          : differenceNote as String?,
      closedByName: closedByName == const $CopyWithPlaceholder()
          ? _value.closedByName
          // ignore: cast_nullable_to_non_nullable
          : closedByName as String?,
      closedUtc: closedUtc == const $CopyWithPlaceholder()
          ? _value.closedUtc
          // ignore: cast_nullable_to_non_nullable
          : closedUtc as DateTime,
    );
  }
}

extension $CashDayClosingCopyWith on CashDayClosing {
  /// Returns a callable class that can be used as follows: `instanceOfCashDayClosing.copyWith(...)` or like so:`instanceOfCashDayClosing.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CashDayClosingCWProxy get copyWith => _$CashDayClosingCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashDayClosing _$CashDayClosingFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CashDayClosing', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'expectedCash',
          'countedCash',
          'differenceMinor',
          'difference',
          'closedUtc',
        ],
      );
      final val = CashDayClosing(
        expectedCash: $checkedConvert(
          'expectedCash',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        countedCash: $checkedConvert(
          'countedCash',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        differenceMinor: $checkedConvert(
          'differenceMinor',
          (v) => (v as num).toInt(),
        ),
        difference: $checkedConvert(
          'difference',
          (v) => $enumDecode(
            _$CashDayClosingDifferenceEnumEnumMap,
            v,
            unknownValue: CashDayClosingDifferenceEnum.unknownDefaultOpenApi,
          ),
        ),
        differenceNote: $checkedConvert('differenceNote', (v) => v as String?),
        closedByName: $checkedConvert('closedByName', (v) => v as String?),
        closedUtc: $checkedConvert(
          'closedUtc',
          (v) => DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CashDayClosingToJson(CashDayClosing instance) =>
    <String, dynamic>{
      'expectedCash': instance.expectedCash.toJson(),
      'countedCash': instance.countedCash.toJson(),
      'differenceMinor': instance.differenceMinor,
      'difference': _$CashDayClosingDifferenceEnumEnumMap[instance.difference]!,
      'differenceNote': ?instance.differenceNote,
      'closedByName': ?instance.closedByName,
      'closedUtc': instance.closedUtc.toIso8601String(),
    };

const _$CashDayClosingDifferenceEnumEnumMap = {
  CashDayClosingDifferenceEnum.none: 'None',
  CashDayClosingDifferenceEnum.shortage: 'Shortage',
  CashDayClosingDifferenceEnum.surplus: 'Surplus',
  CashDayClosingDifferenceEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
