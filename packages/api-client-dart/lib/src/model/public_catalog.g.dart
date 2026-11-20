// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'public_catalog.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$PublicCatalogCWProxy {
  PublicCatalog branchId(String branchId);

  PublicCatalog timeZone(String timeZone);

  PublicCatalog allowStaffSelection(bool allowStaffSelection);

  PublicCatalog allowAnyStaffOption(bool allowAnyStaffOption);

  PublicCatalog priceNote(String? priceNote);

  PublicCatalog categories(List<PublicCatalogCategoriesInner> categories);

  PublicCatalog services(List<PublicCatalogServicesInner> services);

  PublicCatalog staff(List<PublicCatalogStaffInner> staff);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalog(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalog(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalog call({
    String branchId,
    String timeZone,
    bool allowStaffSelection,
    bool allowAnyStaffOption,
    String? priceNote,
    List<PublicCatalogCategoriesInner> categories,
    List<PublicCatalogServicesInner> services,
    List<PublicCatalogStaffInner> staff,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfPublicCatalog.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfPublicCatalog.copyWith.fieldName(...)`
class _$PublicCatalogCWProxyImpl implements _$PublicCatalogCWProxy {
  const _$PublicCatalogCWProxyImpl(this._value);

  final PublicCatalog _value;

  @override
  PublicCatalog branchId(String branchId) => this(branchId: branchId);

  @override
  PublicCatalog timeZone(String timeZone) => this(timeZone: timeZone);

  @override
  PublicCatalog allowStaffSelection(bool allowStaffSelection) =>
      this(allowStaffSelection: allowStaffSelection);

  @override
  PublicCatalog allowAnyStaffOption(bool allowAnyStaffOption) =>
      this(allowAnyStaffOption: allowAnyStaffOption);

  @override
  PublicCatalog priceNote(String? priceNote) => this(priceNote: priceNote);

  @override
  PublicCatalog categories(List<PublicCatalogCategoriesInner> categories) =>
      this(categories: categories);

  @override
  PublicCatalog services(List<PublicCatalogServicesInner> services) =>
      this(services: services);

  @override
  PublicCatalog staff(List<PublicCatalogStaffInner> staff) =>
      this(staff: staff);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `PublicCatalog(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// PublicCatalog(...).copyWith(id: 12, name: "My name")
  /// ````
  PublicCatalog call({
    Object? branchId = const $CopyWithPlaceholder(),
    Object? timeZone = const $CopyWithPlaceholder(),
    Object? allowStaffSelection = const $CopyWithPlaceholder(),
    Object? allowAnyStaffOption = const $CopyWithPlaceholder(),
    Object? priceNote = const $CopyWithPlaceholder(),
    Object? categories = const $CopyWithPlaceholder(),
    Object? services = const $CopyWithPlaceholder(),
    Object? staff = const $CopyWithPlaceholder(),
  }) {
    return PublicCatalog(
      branchId: branchId == const $CopyWithPlaceholder()
          ? _value.branchId
          // ignore: cast_nullable_to_non_nullable
          : branchId as String,
      timeZone: timeZone == const $CopyWithPlaceholder()
          ? _value.timeZone
          // ignore: cast_nullable_to_non_nullable
          : timeZone as String,
      allowStaffSelection: allowStaffSelection == const $CopyWithPlaceholder()
          ? _value.allowStaffSelection
          // ignore: cast_nullable_to_non_nullable
          : allowStaffSelection as bool,
      allowAnyStaffOption: allowAnyStaffOption == const $CopyWithPlaceholder()
          ? _value.allowAnyStaffOption
          // ignore: cast_nullable_to_non_nullable
          : allowAnyStaffOption as bool,
      priceNote: priceNote == const $CopyWithPlaceholder()
          ? _value.priceNote
          // ignore: cast_nullable_to_non_nullable
          : priceNote as String?,
      categories: categories == const $CopyWithPlaceholder()
          ? _value.categories
          // ignore: cast_nullable_to_non_nullable
          : categories as List<PublicCatalogCategoriesInner>,
      services: services == const $CopyWithPlaceholder()
          ? _value.services
          // ignore: cast_nullable_to_non_nullable
          : services as List<PublicCatalogServicesInner>,
      staff: staff == const $CopyWithPlaceholder()
          ? _value.staff
          // ignore: cast_nullable_to_non_nullable
          : staff as List<PublicCatalogStaffInner>,
    );
  }
}

extension $PublicCatalogCopyWith on PublicCatalog {
  /// Returns a callable class that can be used as follows: `instanceOfPublicCatalog.copyWith(...)` or like so:`instanceOfPublicCatalog.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$PublicCatalogCWProxy get copyWith => _$PublicCatalogCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PublicCatalog _$PublicCatalogFromJson(Map<String, dynamic> json) =>
    $checkedCreate('PublicCatalog', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'branchId',
          'timeZone',
          'allowStaffSelection',
          'allowAnyStaffOption',
          'categories',
          'services',
          'staff',
        ],
      );
      final val = PublicCatalog(
        branchId: $checkedConvert('branchId', (v) => v as String),
        timeZone: $checkedConvert('timeZone', (v) => v as String),
        allowStaffSelection: $checkedConvert(
          'allowStaffSelection',
          (v) => v as bool,
        ),
        allowAnyStaffOption: $checkedConvert(
          'allowAnyStaffOption',
          (v) => v as bool,
        ),
        priceNote: $checkedConvert('priceNote', (v) => v as String?),
        categories: $checkedConvert(
          'categories',
          (v) => (v as List<dynamic>)
              .map(
                (e) => PublicCatalogCategoriesInner.fromJson(
                  e as Map<String, dynamic>,
                ),
              )
              .toList(),
        ),
        services: $checkedConvert(
          'services',
          (v) => (v as List<dynamic>)
              .map(
                (e) => PublicCatalogServicesInner.fromJson(
                  e as Map<String, dynamic>,
                ),
              )
              .toList(),
        ),
        staff: $checkedConvert(
          'staff',
          (v) => (v as List<dynamic>)
              .map(
                (e) =>
                    PublicCatalogStaffInner.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$PublicCatalogToJson(PublicCatalog instance) =>
    <String, dynamic>{
      'branchId': instance.branchId,
      'timeZone': instance.timeZone,
      'allowStaffSelection': instance.allowStaffSelection,
      'allowAnyStaffOption': instance.allowAnyStaffOption,
      'priceNote': ?instance.priceNote,
      'categories': instance.categories.map((e) => e.toJson()).toList(),
      'services': instance.services.map((e) => e.toJson()).toList(),
      'staff': instance.staff.map((e) => e.toJson()).toList(),
    };
