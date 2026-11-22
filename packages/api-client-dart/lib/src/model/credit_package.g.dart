// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'credit_package.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$CreditPackageCWProxy {
  CreditPackage id(String id);

  CreditPackage code(String code);

  CreditPackage name(String name);

  CreditPackage credits(int credits);

  CreditPackage price(Money? price);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPackage(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPackage call({
    String id,
    String code,
    String name,
    int credits,
    Money? price,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfCreditPackage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfCreditPackage.copyWith.fieldName(...)`
class _$CreditPackageCWProxyImpl implements _$CreditPackageCWProxy {
  const _$CreditPackageCWProxyImpl(this._value);

  final CreditPackage _value;

  @override
  CreditPackage id(String id) => this(id: id);

  @override
  CreditPackage code(String code) => this(code: code);

  @override
  CreditPackage name(String name) => this(name: name);

  @override
  CreditPackage credits(int credits) => this(credits: credits);

  @override
  CreditPackage price(Money? price) => this(price: price);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `CreditPackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// CreditPackage(...).copyWith(id: 12, name: "My name")
  /// ````
  CreditPackage call({
    Object? id = const $CopyWithPlaceholder(),
    Object? code = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
  }) {
    return CreditPackage(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      code: code == const $CopyWithPlaceholder()
          ? _value.code
          // ignore: cast_nullable_to_non_nullable
          : code as String,
      name: name == const $CopyWithPlaceholder()
          ? _value.name
          // ignore: cast_nullable_to_non_nullable
          : name as String,
      credits: credits == const $CopyWithPlaceholder()
          ? _value.credits
          // ignore: cast_nullable_to_non_nullable
          : credits as int,
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money?,
    );
  }
}

extension $CreditPackageCopyWith on CreditPackage {
  /// Returns a callable class that can be used as follows: `instanceOfCreditPackage.copyWith(...)` or like so:`instanceOfCreditPackage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$CreditPackageCWProxy get copyWith => _$CreditPackageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreditPackage _$CreditPackageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('CreditPackage', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['id', 'code', 'name', 'credits', 'price'],
      );
      final val = CreditPackage(
        id: $checkedConvert('id', (v) => v as String),
        code: $checkedConvert('code', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        credits: $checkedConvert('credits', (v) => (v as num).toInt()),
        price: $checkedConvert(
          'price',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
      );
      return val;
    });

Map<String, dynamic> _$CreditPackageToJson(CreditPackage instance) =>
    <String, dynamic>{
      'id': instance.id,
      'code': instance.code,
      'name': instance.name,
      'credits': instance.credits,
      'price': instance.price?.toJson(),
    };
