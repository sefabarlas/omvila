// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_summary.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CustomerSummaryCWProxy {
  CustomerSummary id(String id);

  CustomerSummary fullName(String fullName);

  CustomerSummary phone(String? phone);

  CustomerSummary phoneMasked(String? phoneMasked);

  CustomerSummary birthYear(int? birthYear);

  CustomerSummary visitCount(int visitCount);

  CustomerSummary noShowCount(int noShowCount);

  CustomerSummary lastVisitOn(DateTime? lastVisitOn);

  CustomerSummary nextAppointmentUtc(DateTime? nextAppointmentUtc);

  CustomerSummary totalSpent(Money? totalSpent);

  CustomerSummary isNew(bool? isNew);

  CustomerSummary isNoShowRisk(bool? isNoShowRisk);

  CustomerSummary tags(List<String> tags);

  CustomerSummary notes(String? notes);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerSummary call({
    String id,
    String fullName,
    String? phone,
    String? phoneMasked,
    int? birthYear,
    int visitCount,
    int noShowCount,
    DateTime? lastVisitOn,
    DateTime? nextAppointmentUtc,
    Money? totalSpent,
    bool? isNew,
    bool? isNoShowRisk,
    List<String> tags,
    String? notes,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCustomerSummary.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCustomerSummary.copyWith.fieldName(...)`
class _$CustomerSummaryCWProxyImpl implements _$CustomerSummaryCWProxy {
  const _$CustomerSummaryCWProxyImpl(this._value);

  final CustomerSummary _value;

  @override
  CustomerSummary id(String id) => this(id: id);

  @override
  CustomerSummary fullName(String fullName) => this(fullName: fullName);

  @override
  CustomerSummary phone(String? phone) => this(phone: phone);

  @override
  CustomerSummary phoneMasked(String? phoneMasked) =>
      this(phoneMasked: phoneMasked);

  @override
  CustomerSummary birthYear(int? birthYear) => this(birthYear: birthYear);

  @override
  CustomerSummary visitCount(int visitCount) => this(visitCount: visitCount);

  @override
  CustomerSummary noShowCount(int noShowCount) =>
      this(noShowCount: noShowCount);

  @override
  CustomerSummary lastVisitOn(DateTime? lastVisitOn) =>
      this(lastVisitOn: lastVisitOn);

  @override
  CustomerSummary nextAppointmentUtc(DateTime? nextAppointmentUtc) =>
      this(nextAppointmentUtc: nextAppointmentUtc);

  @override
  CustomerSummary totalSpent(Money? totalSpent) => this(totalSpent: totalSpent);

  @override
  CustomerSummary isNew(bool? isNew) => this(isNew: isNew);

  @override
  CustomerSummary isNoShowRisk(bool? isNoShowRisk) =>
      this(isNoShowRisk: isNoShowRisk);

  @override
  CustomerSummary tags(List<String> tags) => this(tags: tags);

  @override
  CustomerSummary notes(String? notes) => this(notes: notes);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CustomerSummary(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CustomerSummary(...).copyWith(id: 12, name: "My name")
  /// ````
  CustomerSummary call({
    Object? id = const $CopyWithPlaceholder(),
    Object? fullName = const $CopyWithPlaceholder(),
    Object? phone = const $CopyWithPlaceholder(),
    Object? phoneMasked = const $CopyWithPlaceholder(),
    Object? birthYear = const $CopyWithPlaceholder(),
    Object? visitCount = const $CopyWithPlaceholder(),
    Object? noShowCount = const $CopyWithPlaceholder(),
    Object? lastVisitOn = const $CopyWithPlaceholder(),
    Object? nextAppointmentUtc = const $CopyWithPlaceholder(),
    Object? totalSpent = const $CopyWithPlaceholder(),
    Object? isNew = const $CopyWithPlaceholder(),
    Object? isNoShowRisk = const $CopyWithPlaceholder(),
    Object? tags = const $CopyWithPlaceholder(),
    Object? notes = const $CopyWithPlaceholder(),
  }) {
    return CustomerSummary(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      fullName: fullName == const $CopyWithPlaceholder()
          ? _value.fullName
          // ignore: cast_nullable_to_non_nullable
          : fullName as String,
      phone: phone == const $CopyWithPlaceholder()
          ? _value.phone
          // ignore: cast_nullable_to_non_nullable
          : phone as String?,
      phoneMasked: phoneMasked == const $CopyWithPlaceholder()
          ? _value.phoneMasked
          // ignore: cast_nullable_to_non_nullable
          : phoneMasked as String?,
      birthYear: birthYear == const $CopyWithPlaceholder()
          ? _value.birthYear
          // ignore: cast_nullable_to_non_nullable
          : birthYear as int?,
      visitCount: visitCount == const $CopyWithPlaceholder()
          ? _value.visitCount
          // ignore: cast_nullable_to_non_nullable
          : visitCount as int,
      noShowCount: noShowCount == const $CopyWithPlaceholder()
          ? _value.noShowCount
          // ignore: cast_nullable_to_non_nullable
          : noShowCount as int,
      lastVisitOn: lastVisitOn == const $CopyWithPlaceholder()
          ? _value.lastVisitOn
          // ignore: cast_nullable_to_non_nullable
          : lastVisitOn as DateTime?,
      nextAppointmentUtc: nextAppointmentUtc == const $CopyWithPlaceholder()
          ? _value.nextAppointmentUtc
          // ignore: cast_nullable_to_non_nullable
          : nextAppointmentUtc as DateTime?,
      totalSpent: totalSpent == const $CopyWithPlaceholder()
          ? _value.totalSpent
          // ignore: cast_nullable_to_non_nullable
          : totalSpent as Money?,
      isNew: isNew == const $CopyWithPlaceholder()
          ? _value.isNew
          // ignore: cast_nullable_to_non_nullable
          : isNew as bool?,
      isNoShowRisk: isNoShowRisk == const $CopyWithPlaceholder()
          ? _value.isNoShowRisk
          // ignore: cast_nullable_to_non_nullable
          : isNoShowRisk as bool?,
      tags: tags == const $CopyWithPlaceholder()
          ? _value.tags
          // ignore: cast_nullable_to_non_nullable
          : tags as List<String>,
      notes: notes == const $CopyWithPlaceholder()
          ? _value.notes
          // ignore: cast_nullable_to_non_nullable
          : notes as String?,
    );
  }
}

extension $CustomerSummaryCopyWith on CustomerSummary {
  /// Returns a callable class that can be used as follows: `instanceOfCustomerSummary.copyWith(...)` or like so:`instanceOfCustomerSummary.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CustomerSummaryCWProxy get copyWith => _$CustomerSummaryCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerSummary _$CustomerSummaryFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CustomerSummary', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'fullName',
          'visitCount',
          'noShowCount',
          'tags',
        ],
      );
      final val = CustomerSummary(
        id: $checkedConvert('id', (v) => v as String),
        fullName: $checkedConvert('fullName', (v) => v as String),
        phone: $checkedConvert('phone', (v) => v as String?),
        phoneMasked: $checkedConvert('phoneMasked', (v) => v as String?),
        birthYear: $checkedConvert('birthYear', (v) => (v as num?)?.toInt()),
        visitCount: $checkedConvert('visitCount', (v) => (v as num).toInt()),
        noShowCount: $checkedConvert('noShowCount', (v) => (v as num).toInt()),
        lastVisitOn: $checkedConvert(
          'lastVisitOn',
          (v) => _$JsonConverterFromJson<String, DateTime>(
            v,
            const DateOnlyConverter().fromJson,
          ),
        ),
        nextAppointmentUtc: $checkedConvert(
          'nextAppointmentUtc',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        totalSpent: $checkedConvert(
          'totalSpent',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        isNew: $checkedConvert('isNew', (v) => v as bool?),
        isNoShowRisk: $checkedConvert('isNoShowRisk', (v) => v as bool?),
        tags: $checkedConvert(
          'tags',
          (v) => (v as List<dynamic>).map((e) => e as String).toList(),
        ),
        notes: $checkedConvert('notes', (v) => v as String?),
      );
      return val;
    });

Map<String, dynamic> _$CustomerSummaryToJson(CustomerSummary instance) =>
    <String, dynamic>{
      'id': instance.id,
      'fullName': instance.fullName,
      'phone': ?instance.phone,
      'phoneMasked': ?instance.phoneMasked,
      'birthYear': ?instance.birthYear,
      'visitCount': instance.visitCount,
      'noShowCount': instance.noShowCount,
      'lastVisitOn': ?_$JsonConverterToJson<String, DateTime>(
        instance.lastVisitOn,
        const DateOnlyConverter().toJson,
      ),
      'nextAppointmentUtc': ?instance.nextAppointmentUtc?.toIso8601String(),
      'totalSpent': ?instance.totalSpent?.toJson(),
      'isNew': ?instance.isNew,
      'isNoShowRisk': ?instance.isNoShowRisk,
      'tags': instance.tags,
      'notes': ?instance.notes,
    };

Value? _$JsonConverterFromJson<Json, Value>(
  Object? json,
  Value? Function(Json json) fromJson,
) => json == null ? null : fromJson(json as Json);

Json? _$JsonConverterToJson<Json, Value>(
  Value? value,
  Json? Function(Value value) toJson,
) => value == null ? null : toJson(value);
