// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_waitlist_offer.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicWaitlistOfferCWProxy {
  PublicWaitlistOffer id(String id);

  PublicWaitlistOffer status(WaitlistOfferStatus status);

  PublicWaitlistOffer brand(PublicWaitlistOfferBrand brand);

  PublicWaitlistOffer branch(PublicWaitlistOfferBranch branch);

  PublicWaitlistOffer customerFirstName(String? customerFirstName);

  PublicWaitlistOffer startUtc(DateTime startUtc);

  PublicWaitlistOffer endUtc(DateTime endUtc);

  PublicWaitlistOffer timeZone(String timeZone);

  PublicWaitlistOffer holdExpiresUtc(DateTime holdExpiresUtc);

  PublicWaitlistOffer generatedUtc(DateTime generatedUtc);

  PublicWaitlistOffer items(List<PublicWaitlistOfferItemsInner> items);

  PublicWaitlistOffer total(Money total);

  PublicWaitlistOffer amountDue(Money amountDue);

  PublicWaitlistOffer requiresApproval(bool? requiresApproval);

  PublicWaitlistOffer depositAmount(Money? depositAmount);

  PublicWaitlistOffer appointmentAccessCode(String? appointmentAccessCode);

  PublicWaitlistOffer waitlistAccessCode(String waitlistAccessCode);

  PublicWaitlistOffer version(String version);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistOffer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistOffer(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistOffer call({
    String id,
    WaitlistOfferStatus status,
    PublicWaitlistOfferBrand brand,
    PublicWaitlistOfferBranch branch,
    String? customerFirstName,
    DateTime startUtc,
    DateTime endUtc,
    String timeZone,
    DateTime holdExpiresUtc,
    DateTime generatedUtc,
    List<PublicWaitlistOfferItemsInner> items,
    Money total,
    Money amountDue,
    bool? requiresApproval,
    Money? depositAmount,
    String? appointmentAccessCode,
    String waitlistAccessCode,
    String version,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicWaitlistOffer.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicWaitlistOffer.copyWith.fieldName(...)`
class _$PublicWaitlistOfferCWProxyImpl implements _$PublicWaitlistOfferCWProxy {
  const _$PublicWaitlistOfferCWProxyImpl(this._value);

  final PublicWaitlistOffer _value;

  @override
  PublicWaitlistOffer id(String id) => this(id: id);

  @override
  PublicWaitlistOffer status(WaitlistOfferStatus status) =>
      this(status: status);

  @override
  PublicWaitlistOffer brand(PublicWaitlistOfferBrand brand) =>
      this(brand: brand);

  @override
  PublicWaitlistOffer branch(PublicWaitlistOfferBranch branch) =>
      this(branch: branch);

  @override
  PublicWaitlistOffer customerFirstName(String? customerFirstName) =>
      this(customerFirstName: customerFirstName);

  @override
  PublicWaitlistOffer startUtc(DateTime startUtc) => this(startUtc: startUtc);

  @override
  PublicWaitlistOffer endUtc(DateTime endUtc) => this(endUtc: endUtc);

  @override
  PublicWaitlistOffer timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  PublicWaitlistOffer holdExpiresUtc(DateTime holdExpiresUtc) =>
      this(holdExpiresUtc: holdExpiresUtc);

  @override
  PublicWaitlistOffer generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  PublicWaitlistOffer items(List<PublicWaitlistOfferItemsInner> items) =>
      this(items: items);

  @override
  PublicWaitlistOffer total(Money total) => this(total: total);

  @override
  PublicWaitlistOffer amountDue(Money amountDue) => this(amountDue: amountDue);

  @override
  PublicWaitlistOffer requiresApproval(bool? requiresApproval) =>
      this(requiresApproval: requiresApproval);

  @override
  PublicWaitlistOffer depositAmount(Money? depositAmount) =>
      this(depositAmount: depositAmount);

  @override
  PublicWaitlistOffer appointmentAccessCode(String? appointmentAccessCode) =>
      this(appointmentAccessCode: appointmentAccessCode);

  @override
  PublicWaitlistOffer waitlistAccessCode(String waitlistAccessCode) =>
      this(waitlistAccessCode: waitlistAccessCode);

  @override
  PublicWaitlistOffer version(String version) => this(version: version);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicWaitlistOffer(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicWaitlistOffer(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicWaitlistOffer call({
    Object? id = const $CopyWithPlaceholder(),
    Object? status = const $CopyWithPlaceholder(),
    Object? brand = const $CopyWithPlaceholder(),
    Object? branch = const $CopyWithPlaceholder(),
    Object? customerFirstName = const $CopyWithPlaceholder(),
    Object? startUtc = const $CopyWithPlaceholder(),
    Object? endUtc = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? holdExpiresUtc = const $CopyWithPlaceholder(),
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? items = const $CopyWithPlaceholder(),
    Object? total = const $CopyWithPlaceholder(),
    Object? amountDue = const $CopyWithPlaceholder(),
    Object? requiresApproval = const $CopyWithPlaceholder(),
    Object? depositAmount = const $CopyWithPlaceholder(),
    Object? appointmentAccessCode = const $CopyWithPlaceholder(),
    Object? waitlistAccessCode = const $CopyWithPlaceholder(),
    Object? version = const $CopyWithPlaceholder(),
  }) {
    return PublicWaitlistOffer(
      id: id == const $CopyWithPlaceholder()
          ? _value.id
          // ignore: cast_nullable_to_non_nullable
          : id as String,
      status: status == const $CopyWithPlaceholder()
          ? _value.status
          // ignore: cast_nullable_to_non_nullable
          : status as WaitlistOfferStatus,
      brand: brand == const $CopyWithPlaceholder()
          ? _value.brand
          // ignore: cast_nullable_to_non_nullable
          : brand as PublicWaitlistOfferBrand,
      branch: branch == const $CopyWithPlaceholder()
          ? _value.branch
          // ignore: cast_nullable_to_non_nullable
          : branch as PublicWaitlistOfferBranch,
      customerFirstName: customerFirstName == const $CopyWithPlaceholder()
          ? _value.customerFirstName
          // ignore: cast_nullable_to_non_nullable
          : customerFirstName as String?,
      startUtc: startUtc == const $CopyWithPlaceholder()
          ? _value.startUtc
          // ignore: cast_nullable_to_non_nullable
          : startUtc as DateTime,
      endUtc: endUtc == const $CopyWithPlaceholder()
          ? _value.endUtc
          // ignore: cast_nullable_to_non_nullable
          : endUtc as DateTime,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      holdExpiresUtc: holdExpiresUtc == const $CopyWithPlaceholder()
          ? _value.holdExpiresUtc
          // ignore: cast_nullable_to_non_nullable
          : holdExpiresUtc as DateTime,
      generatedUtc: generatedUtc == const $CopyWithPlaceholder()
          ? _value.generatedUtc
          // ignore: cast_nullable_to_non_nullable
          : generatedUtc as DateTime,
      items: items == const $CopyWithPlaceholder()
          ? _value.items
          // ignore: cast_nullable_to_non_nullable
          : items as List<PublicWaitlistOfferItemsInner>,
      total: total == const $CopyWithPlaceholder()
          ? _value.total
          // ignore: cast_nullable_to_non_nullable
          : total as Money,
      amountDue: amountDue == const $CopyWithPlaceholder()
          ? _value.amountDue
          // ignore: cast_nullable_to_non_nullable
          : amountDue as Money,
      requiresApproval: requiresApproval == const $CopyWithPlaceholder()
          ? _value.requiresApproval
          // ignore: cast_nullable_to_non_nullable
          : requiresApproval as bool?,
      depositAmount: depositAmount == const $CopyWithPlaceholder()
          ? _value.depositAmount
          // ignore: cast_nullable_to_non_nullable
          : depositAmount as Money?,
      appointmentAccessCode:
          appointmentAccessCode == const $CopyWithPlaceholder()
          ? _value.appointmentAccessCode
          // ignore: cast_nullable_to_non_nullable
          : appointmentAccessCode as String?,
      waitlistAccessCode: waitlistAccessCode == const $CopyWithPlaceholder()
          ? _value.waitlistAccessCode
          // ignore: cast_nullable_to_non_nullable
          : waitlistAccessCode as String,
      version: version == const $CopyWithPlaceholder()
          ? _value.version
          // ignore: cast_nullable_to_non_nullable
          : version as String,
    );
  }
}

extension $PublicWaitlistOfferCopyWith on PublicWaitlistOffer {
  /// Returns a callable class that can be used as follows: `instanceOfPublicWaitlistOffer.copyWith(...)` or like so:`instanceOfPublicWaitlistOffer.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicWaitlistOfferCWProxy get copyWith =>
      _$PublicWaitlistOfferCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicWaitlistOffer _$PublicWaitlistOfferFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('PublicWaitlistOffer', json, ($checkedConvert) {
  $checkKeys(
    json,
    requiredKeys: const [
      'id',
      'status',
      'brand',
      'branch',
      'startUtc',
      'endUtc',
      'timeZone',
      'holdExpiresUtc',
      'generatedUtc',
      'items',
      'total',
      'amountDue',
      'waitlistAccessCode',
      'version',
    ],
  );
  final val = PublicWaitlistOffer(
    id: $checkedConvert('id', (v) => v as String),
    status: $checkedConvert(
      'status',
      (v) => $enumDecode(
        _$WaitlistOfferStatusEnumMap,
        v,
        unknownValue: WaitlistOfferStatus.unknownDefaultOpenApi,
      ),
    ),
    brand: $checkedConvert(
      'brand',
      (v) => PublicWaitlistOfferBrand.fromJson(v as Map<String, dynamic>),
    ),
    branch: $checkedConvert(
      'branch',
      (v) => PublicWaitlistOfferBranch.fromJson(v as Map<String, dynamic>),
    ),
    customerFirstName: $checkedConvert(
      'customerFirstName',
      (v) => v as String?,
    ),
    startUtc: $checkedConvert('startUtc', (v) => DateTime.parse(v as String)),
    endUtc: $checkedConvert('endUtc', (v) => DateTime.parse(v as String)),
    timeZone: $checkedConvert('timeZone', (v) => v as String),
    holdExpiresUtc: $checkedConvert(
      'holdExpiresUtc',
      (v) => DateTime.parse(v as String),
    ),
    generatedUtc: $checkedConvert(
      'generatedUtc',
      (v) => DateTime.parse(v as String),
    ),
    items: $checkedConvert(
      'items',
      (v) => (v as List<dynamic>)
          .map(
            (e) => PublicWaitlistOfferItemsInner.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList(),
    ),
    total: $checkedConvert(
      'total',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    amountDue: $checkedConvert(
      'amountDue',
      (v) => Money.fromJson(v as Map<String, dynamic>),
    ),
    requiresApproval: $checkedConvert('requiresApproval', (v) => v as bool?),
    depositAmount: $checkedConvert(
      'depositAmount',
      (v) => v == null ? null : Money.fromJson(v as Map<String, dynamic>),
    ),
    appointmentAccessCode: $checkedConvert(
      'appointmentAccessCode',
      (v) => v as String?,
    ),
    waitlistAccessCode: $checkedConvert(
      'waitlistAccessCode',
      (v) => v as String,
    ),
    version: $checkedConvert('version', (v) => v as String),
  );
  return val;
});

Map<String, dynamic> _$PublicWaitlistOfferToJson(
  PublicWaitlistOffer instance,
) => <String, dynamic>{
  'id': instance.id,
  'status': _$WaitlistOfferStatusEnumMap[instance.status]!,
  'brand': instance.brand.toJson(),
  'branch': instance.branch.toJson(),
  'customerFirstName': ?instance.customerFirstName,
  'startUtc': instance.startUtc.toIso8601String(),
  'endUtc': instance.endUtc.toIso8601String(),
  'timeZone': instance.timeZone,
  'holdExpiresUtc': instance.holdExpiresUtc.toIso8601String(),
  'generatedUtc': instance.generatedUtc.toIso8601String(),
  'items': instance.items.map((e) => e.toJson()).toList(),
  'total': instance.total.toJson(),
  'amountDue': instance.amountDue.toJson(),
  'requiresApproval': ?instance.requiresApproval,
  'depositAmount': ?instance.depositAmount?.toJson(),
  'appointmentAccessCode': ?instance.appointmentAccessCode,
  'waitlistAccessCode': instance.waitlistAccessCode,
  'version': instance.version,
};

const _$WaitlistOfferStatusEnumMap = {
  WaitlistOfferStatus.pending: 'Pending',
  WaitlistOfferStatus.accepted: 'Accepted',
  WaitlistOfferStatus.declined: 'Declined',
  WaitlistOfferStatus.expired: 'Expired',
  WaitlistOfferStatus.withdrawn: 'Withdrawn',
  WaitlistOfferStatus.unknownDefaultOpenApi: 'unknown_default_open_api',
};
