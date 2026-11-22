// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'commission_report.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CommissionReportCWProxy {
  CommissionReport branchId(String? branchId);

  CommissionReport from(DateTime from);

  CommissionReport to(DateTime to);

  CommissionReport currency(CommissionReportCurrencyEnum currency);

  CommissionReport items(List<CommissionReportRow> items);

  CommissionReport totalCommissionMinor(int totalCommissionMinor);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionReport(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionReport(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionReport call({
    String? branchId,
    DateTime from,
    DateTime to,
    CommissionReportCurrencyEnum currency,
    List<CommissionReportRow> items,
    int totalCommissionMinor,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCommissionReport.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCommissionReport.copyWith.fieldName(...)`
class _$CommissionReportCWProxyImpl implements _$CommissionReportCWProxy {
  const _$CommissionReportCWProxyImpl(this._value);

  final CommissionReport _value;

  @override
  CommissionReport branchId(String? branchId) => this(branchId: branchId);

  @override
  CommissionReport from(DateTime from) => this(from: from);

  @override
  CommissionReport to(DateTime to) => this(to: to);

  @override
  CommissionReport currency(CommissionReportCurrencyEnum currency) =>
      this(currency: currency);

  @override
  CommissionReport items(List<CommissionReportRow> items) => this(items: items);

  @override
  CommissionReport totalCommissionMinor(int totalCommissionMinor) =>
      this(totalCommissionMinor: totalCommissionMinor);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CommissionReport(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CommissionReport(...).copyWith(id: 12, name: "My name")
  /// ````
  CommissionReport call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? from = const $CopyWithPlaceholder(),
    Object? to = const $CopyWithPlaceholder(),
    Object? currency = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? totalCommissionMinor = const $CopyWithPlaceholder(),
  }) {
    return CommissionReport(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String?,
      from: from == const $CopyWithPlaceholder()
          ? _value.from
          // ignore: cast_nullable_to_non_nullable
          : from as DateTime,
      to: to == const $CopyWithPlaceholder()
          ? _value.to
          // ignore: cast_nullable_to_non_nullable
          : to as DateTime,
      currency: currency == const $CopyWithPlaceholder()
          ? _value.currency
          // ignore: cast_nullable_to_non_nullable
          : currency as CommissionReportCurrencyEnum,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<CommissionReportRow>,
      totalCommissionMinor: totalCommissionMinor == const $CopyWithPlaceholder()
          ? _value.totalCommissionMinor
          // ignore: cast_nullable_to_non_nullable
          : totalCommissionMinor as int,
    );
  }
}

extension $CommissionReportCopyWith on CommissionReport {
  /// Returns a callable class that can be used as follows: `instanceOfCommissionReport.copyWith(...)` or like so:`instanceOfCommissionReport.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CommissionReportCWProxy get copyWith => _$CommissionReportCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CommissionReport _$CommissionReportFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CommissionReport', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'from',
          'to',
          'currency',
          'items',
          'totalCommissionMinor',
        ],
      );
      final val = CommissionReport(
        branchId: $checkedConvert('branchId', (v) => v as String?),
        from: $checkedConvert(
          'from',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        to: $checkedConvert(
          'to',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        currency: $checkedConvert(
          'currency',
          (v) => $enumDecode(
            _$CommissionReportCurrencyEnumEnumMap,
            v,
            unknownValue: CommissionReportCurrencyEnum.unknownDefaultOpenApi,
          ),
        ),
        items: $checkedConvert(
          'items',
          (v) => (v as List<dynamic>)
              .map(
                (e) => CommissionReportRow.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
        totalCommissionMinor: $checkedConvert(
          'totalCommissionMinor',
          (v) => (v as num).toInt(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CommissionReportToJson(CommissionReport instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'from': const DateOnlyConverter().toJson(instance.from),
      'to': const DateOnlyConverter().toJson(instance.to),
      'currency': _$CommissionReportCurrencyEnumEnumMap[instance.currency]!,
      'items': instance.items.map((e) => e.toJson()).toList(),
      'totalCommissionMinor': instance.totalCommissionMinor,
    };

const _$CommissionReportCurrencyEnumEnumMap = {
  CommissionReportCurrencyEnum.TRY: 'TRY',
  CommissionReportCurrencyEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
