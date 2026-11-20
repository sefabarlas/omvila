// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_service_price.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$BranchServicePriceCWProxy {
  BranchServicePrice price(Money price);

  BranchServicePrice durationOverrideMinutes(int? durationOverrideMinutes);

  BranchServicePrice isAvailable(bool isAvailable);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchServicePrice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchServicePrice(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchServicePrice call({
    Money price,
    int? durationOverrideMinutes,
    bool isAvailable,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfBranchServicePrice.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfBranchServicePrice.copyWith.fieldName(...)`
class _$BranchServicePriceCWProxyImpl implements _$BranchServicePriceCWProxy {
  const _$BranchServicePriceCWProxyImpl(this._value);

  final BranchServicePrice _value;

  @override
  BranchServicePrice price(Money price) => this(price: price);

  @override
  BranchServicePrice durationOverrideMinutes(int? durationOverrideMinutes) =>
      this(durationOverrideMinutes: durationOverrideMinutes);

  @override
  BranchServicePrice isAvailable(bool isAvailable) =>
      this(isAvailable: isAvailable);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `BranchServicePrice(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// BranchServicePrice(...).copyWith(id: 12, name: "My name")
  /// ````
  BranchServicePrice call({
    Object? price = const $CopyWithPlaceholder(),
    Object? durationOverrideMinutes = const $CopyWithPlaceholder(),
    Object? isAvailable = const $CopyWithPlaceholder(),
  }) {
    return BranchServicePrice(
      price: price == const $CopyWithPlaceholder()
          ? _value.price
          // ignore: cast_nullable_to_non_nullable
          : price as Money,
      durationOverrideMinutes:
          durationOverrideMinutes == const $CopyWithPlaceholder()
          ? _value.durationOverrideMinutes
          // ignore: cast_nullable_to_non_nullable
          : durationOverrideMinutes as int?,
      isAvailable: isAvailable == const $CopyWithPlaceholder()
          ? _value.isAvailable
          // ignore: cast_nullable_to_non_nullable
          : isAvailable as bool,
    );
  }
}

extension $BranchServicePriceCopyWith on BranchServicePrice {
  /// Returns a callable class that can be used as follows: `instanceOfBranchServicePrice.copyWith(...)` or like so:`instanceOfBranchServicePrice.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$BranchServicePriceCWProxy get copyWith =>
      _$BranchServicePriceCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

BranchServicePrice _$BranchServicePriceFromJson(Map<String, dynamic> json) =>
    $checkedCreate('BranchServicePrice', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['price', 'isAvailable']);
      final val = BranchServicePrice(
        price: $checkedConvert(
          'price',
          (v) => Money.fromJson(v as Map<String, dynamic>),
        ),
        durationOverrideMinutes: $checkedConvert(
          'durationOverrideMinutes',
          (v) => (v as num?)?.toInt(),
        ),
        isAvailable: $checkedConvert('isAvailable', (v) => v as bool),
      );
      return val;
    });

Map<String, dynamic> _$BranchServicePriceToJson(BranchServicePrice instance) =>
    <String, dynamic>{
      'price': instance.price.toJson(),
      'durationOverrideMinutes': ?instance.durationOverrideMinutes,
      'isAvailable': instance.isAvailable,
    };
