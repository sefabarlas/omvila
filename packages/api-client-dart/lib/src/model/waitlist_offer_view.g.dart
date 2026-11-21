// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'waitlist_offer_view.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$WaitlistOfferViewCWProxy {
  WaitlistOfferView id(String id);

  WaitlistOfferView status(WaitlistOfferStatus status);

  WaitlistOfferView startUtc(DateTime startUtc);

  WaitlistOfferView endUtc(DateTime endUtc);

  WaitlistOfferView staffId(String? staffId);

  WaitlistOfferView staffName(String? staffName);

  WaitlistOfferView offeredUtc(DateTime offeredUtc);

  WaitlistOfferView holdExpiresUtc(DateTime holdExpiresUtc);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistOfferView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistOfferView(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistOfferView call({
    String id,
    WaitlistOfferStatus status,
    DateTime startUtc,
    DateTime endUtc,
    String? staffId,
    String? staffName,
    DateTime offeredUtc,
    DateTime holdExpiresUtc,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfWaitlistOfferView.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfWaitlistOfferView.copyWith.fieldName(...)`
class _$WaitlistOfferViewCWProxyImpl implements _$WaitlistOfferViewCWProxy {
  const _$WaitlistOfferViewCWProxyImpl(this._value);

  final WaitlistOfferView _value;

  @override
  WaitlistOfferView id(String id) => this(id: id);

  @override
  WaitlistOfferView status(WaitlistOfferStatus status) => this(status: status);

  @override
  WaitlistOfferView startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  WaitlistOfferView endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  WaitlistOfferView staffId(String? staffId) => this(staffId: staffId);

  @override
  WaitlistOfferView staffName(String? staffName) => this(staffName: staffName);

  @override
  WaitlistOfferView offeredUtc(DateTime offeredUtc) =>
      this(offeredUtc: offeredUtc);

  @override
  WaitlistOfferView holdExpiresUtc(DateTime holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `WaitlistOfferView(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// WaitlistOfferView(...).copyWith(id: 12, name: "My name")
  /// ````
  WaitlistOfferView call({
    Object? id = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? staffId = const $CopyWithPlaceholder(),
    Object? staffName = const $CopyWithPlaceholder(),
    Object? offeredUtc = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
  }) {
    return WaitlistOfferView(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as WaitlistOfferStatus,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      staffId: staffId == const $CopyWithPlaceholder()
          ? _value.staffId
          // ignore: cast_nullable_to_non_nullable
          : staffId as String?,
      staffName: staffName == const $CopyWithPlaceholder()
          ? _value.staffName
          // ignore: cast_nullable_to_non_nullable
          : staffName as String?,
      offeredUtc: offeredUtc == const $CopyWithPlaceholder()
          ? _value.offeredUtc
          // ignore: cast_nullable_to_non_nullable
          : offeredUtc as DateTime,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime,
    );
  }
}

extension $WaitlistOfferViewCopyWith on WaitlistOfferView {
  /// Returns a callable class that can be used as follows: `instanceOfWaitlistOfferView.copyWith(...)` or like so:`instanceOfWaitlistOfferView.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$WaitlistOfferViewCWProxy get copyWith =>
      _$WaitlistOfferViewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WaitlistOfferView _$WaitlistOfferViewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('WaitlistOfferView', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'id',
          'status',
          'startUtc',
          'endUtc',
          'offeredUtc',
          'holdExpiresUtc',
        ],
      );
      final val = WaitlistOfferView(
        id: $checkedConvert('id', (v) => v as String),
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$WaitlistOfferStatusEnumMap,
            v,
            unknownValue: WaitlistOfferStatus.unknownDefaultOpenApi,
          ),
        ),
        startUtc: $checkedConvert(
          'startUtc',
          (v) => DateTime.parse(v as String),
        ),
        endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
        staffId: $checkedConvert('staffId', (v) => v as String?),
        staffName: $checkedConvert('staffName', (v) => v as String?),
        offeredUtc: $checkedConvert(
          'offeredUtc',
          (v) => DateTime.parse(v as String),
        ),
        holdExpiresUtc: $checkedConvert(
          'holdExpiresUtc',
          (v) => DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$WaitlistOfferViewToJson(WaitlistOfferView instance) =>
    <String, dynamic>{
      'id': instance.id,
      'status': _$WaitlistOfferStatusEnumMap[instance.status]!,
      'startUtc': instance.startUtc.toIso8601String(),
      'endUtc': instance.endUtc.toIso8601String(),
      'staffId': ?instance.staffId,
      'staffName': ?instance.staffName,
      'offeredUtc': instance.offeredUtc.toIso8601String(),
      'holdExpiresUtc': instance.holdExpiresUtc.toIso8601String(),
    };

const _$WaitlistOfferStatusEnumMap = {
  WaitlistOfferStatus.pending: 'Pending',
  WaitlistOfferStatus.accepted: 'Accepted',
  WaitlistOfferStatus.declined: 'Declined',
  WaitlistOfferStatus.expired: 'Expired',
  WaitlistOfferStatus.withdrawn: 'Withdrawn',
  WaitlistOfferStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
