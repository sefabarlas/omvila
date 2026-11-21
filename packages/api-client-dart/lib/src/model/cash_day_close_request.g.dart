// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_day_close_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CashDayCloseRequestCWProxy {
  CashDayCloseRequest branchId(String branchId);

  CashDayCloseRequest date(DateTime date);

  CashDayCloseRequest countedCash(Money countedCash);

  CashDayCloseRequest differenceNote(String? differenceNote);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDayCloseRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDayCloseRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDayCloseRequest call({
    String branchId,
    DateTime date,
    Money countedCash,
    String? differenceNote,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCashDayCloseRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCashDayCloseRequest.copyWith.fieldName(...)`
class _$CashDayCloseRequestCWProxyImpl implements _$CashDayCloseRequestCWProxy {
  const _$CashDayCloseRequestCWProxyImpl(this._value);

  final CashDayCloseRequest _value;

  @override
  CashDayCloseRequest branchId(String branchId) => this(branchId: branchId);

  @override
  CashDayCloseRequest date(DateTime date) => this(date: date);

  @override
  CashDayCloseRequest countedCash(Money countedCash) =>
      this(countedCash: countedCash);

  @override
  CashDayCloseRequest differenceNote(String? differenceNote) =>
      this(differenceNote: differenceNote);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CashDayCloseRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CashDayCloseRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  CashDayCloseRequest call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? date = const $CopyWithPlaceholder(),
    Object? countedCash = const $CopyWithPlaceholder(),
    Object? differenceNote = const $CopyWithPlaceholder(),
  }) {
    return CashDayCloseRequest(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      date: date == const $CopyWithPlaceholder()
          ? _value.date
          // ignore: cast_nullable_to_non_nullable
          : date as DateTime,
      countedCash: countedCash == const $CopyWithPlaceholder()
          ? _value.countedCash
          // ignore: cast_nullable_to_non_nullable
          : countedCash as Money,
      differenceNote: differenceNote == const $CopyWithPlaceholder()
          ? _value.differenceNote
          // ignore: cast_nullable_to_non_nullable
          : differenceNote as String?,
    );
  }
}

extension $CashDayCloseRequestCopyWith on CashDayCloseRequest {
  /// Returns a callable class that can be used as follows: `instanceOfCashDayCloseRequest.copyWith(...)` or like so:`instanceOfCashDayCloseRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CashDayCloseRequestCWProxy get copyWith =>
      _$CashDayCloseRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashDayCloseRequest _$CashDayCloseRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CashDayCloseRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['branchId', 'date', 'countedCash']);
      final val = CashDayCloseRequest(
        branchId: $checkedConvert('branchId', (v) => v as String),
        date: $checkedConvert('date', (v) => DateTime.parse(v as String)),
        countedCash: $checkedConvert(
          'countedCash',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        differenceNote: $checkedConvert('differenceNote', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$CashDayCloseRequestToJson(
  CashDayCloseRequest instance,
) => <String, dynamic>{
  'branchId': instance.branchId,
  'date': instance.date.toIso8601String(),
  'countedCash': instance.countedCash.toJson(),
  'differenceNote': ?instance.differenceNote,
};
