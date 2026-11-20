// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_banner.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$SubscriptionBannerCWProxy {
  SubscriptionBanner status(SubscriptionBannerStatusEnum status);

  SubscriptionBanner trialEndsOn(DateTime? trialEndsOn);

  SubscriptionBanner pastDueSince(DateTime? pastDueSince);

  SubscriptionBanner readOnlyOn(DateTime? readOnlyOn);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionBanner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionBanner(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionBanner call({
    SubscriptionBannerStatusEnum status,
    DateTime? trialEndsOn,
    DateTime? pastDueSince,
    DateTime? readOnlyOn,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfSubscriptionBanner.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfSubscriptionBanner.copyWith.fieldName(...)`
class _$SubscriptionBannerCWProxyImpl implements _$SubscriptionBannerCWProxy {
  const _$SubscriptionBannerCWProxyImpl(this._value);

  final SubscriptionBanner _value;

  @override
  SubscriptionBanner status(SubscriptionBannerStatusEnum status) =>
      this(status: status);

  @override
  SubscriptionBanner trialEndsOn(DateTime? trialEndsOn) =>
      this(trialEndsOn: trialEndsOn);

  @override
  SubscriptionBanner pastDueSince(DateTime? pastDueSince) =>
      this(pastDueSince: pastDueSince);

  @override
  SubscriptionBanner readOnlyOn(DateTime? readOnlyOn) =>
      this(readOnlyOn: readOnlyOn);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `SubscriptionBanner(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// SubscriptionBanner(...).copyWith(id: 12, name: "My name")
  /// ````
  SubscriptionBanner call({
    Object? status = const $CopyWithPlaceholder(),
    Object? trialEndsOn = const $CopyWithPlaceholder(),
    Object? pastDueSince = const $CopyWithPlaceholder(),
    Object? readOnlyOn = const $CopyWithPlaceholder(),
  }) {
    return SubscriptionBanner(
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as SubscriptionBannerStatusEnum,
      trialEndsOn: trialEndsOn == const $CopyWithPlaceholder()
          ? _value.trialEndsOn
          // ignore: cast_nullable_to_non_nullable
          : trialEndsOn as DateTime?,
      pastDueSince: pastDueSince == const $CopyWithPlaceholder()
          ? _value.pastDueSince
          // ignore: cast_nullable_to_non_nullable
          : pastDueSince as DateTime?,
      readOnlyOn: readOnlyOn == const $CopyWithPlaceholder()
          ? _value.readOnlyOn
          // ignore: cast_nullable_to_non_nullable
          : readOnlyOn as DateTime?,
    );
  }
}

extension $SubscriptionBannerCopyWith on SubscriptionBanner {
  /// Returns a callable class that can be used as follows: `instanceOfSubscriptionBanner.copyWith(...)` or like so:`instanceOfSubscriptionBanner.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$SubscriptionBannerCWProxy get copyWith =>
      _$SubscriptionBannerCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionBanner _$SubscriptionBannerFromJson(Map<String, dynamic> json) =>
    $checkedCreate('SubscriptionBanner', json, ($checkedConvert) {
      $checkKeys(json, requiredKeys: const ['status']);
      final val = SubscriptionBanner(
        status: $checkedConvert(
          'status',
          (v) => $enumDecode(
            _$SubscriptionBannerStatusEnumEnumMap,
            v,
            unknownValue: SubscriptionBannerStatusEnum.unknownDefaultOpenApi,
          ),
        ),
        trialEndsOn: $checkedConvert(
          'trialEndsOn',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        pastDueSince: $checkedConvert(
          'pastDueSince',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
        readOnlyOn: $checkedConvert(
          'readOnlyOn',
          (v) => v == null ? null : DateTime.parse(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$SubscriptionBannerToJson(SubscriptionBanner instance) =>
    <String, dynamic>{
      'status': _$SubscriptionBannerStatusEnumEnumMap[instance.status]!,
      'trialEndsOn': ?instance.trialEndsOn?.toIso8601String(),
      'pastDueSince': ?instance.pastDueSince?.toIso8601String(),
      'readOnlyOn': ?instance.readOnlyOn?.toIso8601String(),
    };

const _$SubscriptionBannerStatusEnumEnumMap = {
  SubscriptionBannerStatusEnum.trialing: 'Trialing',
  SubscriptionBannerStatusEnum.active: 'Active',
  SubscriptionBannerStatusEnum.pastDue: 'PastDue',
  SubscriptionBannerStatusEnum.readOnly: 'ReadOnly',
  SubscriptionBannerStatusEnum.cancelled: 'Cancelled',
  SubscriptionBannerStatusEnum.unknownDefaultOpenApi:
      'unknown_default_open_api',
};
