// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'waitlist_offer_request.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WaitlistOfferRequestCWProxy {
  WaitlistOfferRequest startUtc(DateTime startUtc);

  WaitlistOfferRequest staffId(String staffId);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistOfferRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistOfferRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistOfferRequest call({DateTime startUtc, String staffId});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWaitlistOfferRequest.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWaitlistOfferRequest.copyWith.fieldName(...)`
class _$WaitlistOfferRequestCWProxyImpl
    implements _$WaitlistOfferRequestCWProxy {
  const _$WaitlistOfferRequestCWProxyImpl(this._value);

  final WaitlistOfferRequest _value;

  @override
  WaitlistOfferRequest startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  WaitlistOfferRequest staffId(String staffId) => this(staffId: staffId);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistOfferRequest(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistOfferRequest(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistOfferRequest call({
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
  }) {
    return WaitlistOfferRequest(
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String,
    );
  }
}

extension $WaitlistOfferRequestCopyWith on WaitlistOfferRequest {
  /// Returns a callable class that can be used as follows: `instanceOfWaitlistOfferRequest.copyWith(...)` or like so:`instanceOfWaitlistOfferRequest.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WaitlistOfferRequestCWProxy get copyWith =>
      _$WaitlistOfferRequestCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WaitlistOfferRequest _$WaitlistOfferRequestFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('WaitlistOfferRequest', json, ($checkedConvert) {
  $checkKeys(json, requiredKeys: const ['startUtc', 'staffId']);
  final val = WaitlistOfferRequest(
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    staffId: $checkedConvert('staffId', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$WaitlistOfferRequestToJson(
  WaitlistOfferRequest instance,
) => <String, dynamic>{
  'startUtc': instance.startUtc.toIso8601String(),
  'staffId': instance.staffId,
};
