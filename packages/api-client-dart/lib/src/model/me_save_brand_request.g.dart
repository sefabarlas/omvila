// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'me_save_brand_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$MeSaveBrandRequestCWProxy {
  MeSaveBrandRequest brandSlug(String brandSlug);

  MeSaveBrandRequest source_(MeSaveBrandRequestSource_Enum source_);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeSaveBrandRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeSaveBrandRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeSaveBrandRequest call({
    String brandSlug,
    MeSaveBrandRequestSource_Enum source_,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfMeSaveBrandRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfMeSaveBrandRequest.copyWith.fieldName(...)`
class _$MeSaveBrandRequestCWProxyImpl implements _$MeSaveBrandRequestCWProxy {
  const _$MeSaveBrandRequestCWProxyImpl(this._value);

  final MeSaveBrandRequest _value;

  @override
  MeSaveBrandRequest brandSlug(String brandSlug) => this(brandSlug: brandSlug);

  @override
  MeSaveBrandRequest source_(MeSaveBrandRequestSource_Enum source_) =>
      this(source_: source_);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `MeSaveBrandRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// MeSaveBrandRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  MeSaveBrandRequest call({
    Object? brandSlug = const $CopyWithPlaceholder(),
    Object? source_ = const $CopyWithPlaceholder(),
  }) {
    return MeSaveBrandRequest(
      brandSlug: brandSlug == const $CopyWithPlaceholder()
          ? _value.brandSlug
          // ignore: cast_nullable_to_non_nullable
          : brandSlug as String,
      source_: source_ == const $CopyWithPlaceholder()
          ? _value.source_
          // ignore: cast_nullable_to_non_nullable
          : source_ as MeSaveBrandRequestSource_Enum,
    );
  }
}

extension $MeSaveBrandRequestCopyWith on MeSaveBrandRequest {
  /// Returns a callable class that can be used as follows: `instanceOfMeSaveBrandRequest.copyWith(...)` or like so:`instanceOfMeSaveBrandRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$MeSaveBrandRequestCWProxy get copyWith =>
      _$MeSaveBrandRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

MeSaveBrandRequest _$MeSaveBrandRequestFromJson(Map<String, dynamic> json) =>
    $checkedCreate('MeSaveBrandRequest', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['brandSlug', 'source']);
      final val = MeSaveBrandRequest(
        brandSlug: $checkedConvert('brandSlug', (v) => v as String),
        source_: $checkedConvert(
          'source',
          (v) => $enumDecode(
            _$MeSaveBrandRequestSource_EnumEnumMap,
            v,
            unknownValue: MeSaveBrandRequestSource_Enum.unknownDefaultOpenApi,
          ),
        ),
      );
      return val;
    }, fieldKeyMap: const {'source_': 'source'});

Map<String, dynamic> _$MeSaveBrandRequestToJson(MeSaveBrandRequest instance) =>
    <String, dynamic>{
      'brandSlug': instance.brandSlug,
      'source': _$MeSaveBrandRequestSource_EnumEnumMap[instance.source_]!,
    };

const _$MeSaveBrandRequestSource_EnumEnumMap = {
  MeSaveBrandRequestSource_Enum.qr: 'Qr',
  MeSaveBrandRequestSource_Enum.link: 'Link',
  MeSaveBrandRequestSource_Enum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
