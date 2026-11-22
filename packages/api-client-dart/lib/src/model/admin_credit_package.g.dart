// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_credit_package.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminCreditPackageCWProxy {
  AdminCreditPackage code(String code);

  AdminCreditPackage name(String name);

  AdminCreditPackage credits(int credits);

  AdminCreditPackage price(Money? price);

  AdminCreditPackage isActive(bool isActive);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditPackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditPackage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditPackage call({
    String code,
    String name,
    int credits,
    Money? price,
    bool isActive,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminCreditPackage.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminCreditPackage.copyWith.fieldName(...)`
class _$AdminCreditPackageCWProxyImpl implements _$AdminCreditPackageCWProxy {
  const _$AdminCreditPackageCWProxyImpl(this._value);

  final AdminCreditPackage _value;

  @override
  AdminCreditPackage code(String code) => this(code: code);

  @override
  AdminCreditPackage name(String name) => this(name: name);

  @override
  AdminCreditPackage credits(int credits) => this(credits: credits);

  @override
  AdminCreditPackage price(Money? price) => this(price: price);

  @override
  AdminCreditPackage isActive(bool isActive) => this(isActive: isActive);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminCreditPackage(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminCreditPackage(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminCreditPackage call({
    Object? code = const $CopyWithPlaceholder(),
    Object? name = const $CopyWithPlaceholder(),
    Object? credits = const $CopyWithPlaceholder(),
    Object? price = const $CopyWithPlaceholder(),
    Object? isActive = const $CopyWithPlaceholder(),
  }) {
    return AdminCreditPackage(
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
      isActive: isActive == const $CopyWithPlaceholder()
          ? _value.isActive
          // ignore: cast_nullable_to_non_nullable
          : isActive as bool,
    );
  }
}

extension $AdminCreditPackageCopyWith on AdminCreditPackage {
  /// Returns a callable class that can be used as follows: `instanceOfAdminCreditPackage.copyWith(...)` or like so:`instanceOfAdminCreditPackage.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminCreditPackageCWProxy get copyWith =>
      _$AdminCreditPackageCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminCreditPackage _$AdminCreditPackageFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminCreditPackage', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const ['code', 'name', 'credits', 'price', 'isActive'],
      );
      final val = AdminCreditPackage(
        code: $checkedConvert('code', (v) => v as String),
        name: $checkedConvert('name', (v) => v as String),
        credits: $checkedConvert('credits', (v) => (v as num).toInt()),
        price: $checkedConvert(
          'price',
          (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
        ),
        isActive: $checkedConvert('isActive', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$AdminCreditPackageToJson(AdminCreditPackage instance) =>
    <String, dynamic>{
      'code': instance.code,
      'name': instance.name,
      'credits': instance.credits,
      'price': instance.price?.toJson(),
      'isActive': instance.isActive,
    };
