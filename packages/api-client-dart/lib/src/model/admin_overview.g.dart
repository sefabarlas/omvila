// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_overview.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminOverviewCWProxy {
  AdminOverview generatedUtc(DateTime generatedUtc);

  AdminOverview activeBrands(AdminOverviewActiveBrands activeBrands);

  AdminOverview pendingApprovals(
    AdminOverviewPendingApprovals pendingApprovals,
  );

  AdminOverview trialing(AdminOverviewTrialing trialing);

  AdminOverview pastDue(AdminOverviewPastDue pastDue);

  AdminOverview funnel(AdminOverviewFunnel funnel);

  AdminOverview attentionBrands(List<AdminAttentionBrand> attentionBrands);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverview call({
    DateTime generatedUtc,
    AdminOverviewActiveBrands activeBrands,
    AdminOverviewPendingApprovals pendingApprovals,
    AdminOverviewTrialing trialing,
    AdminOverviewPastDue pastDue,
    AdminOverviewFunnel funnel,
    List<AdminAttentionBrand> attentionBrands,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminOverview.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminOverview.copyWith.fieldName(...)`
class _$AdminOverviewCWProxyImpl implements _$AdminOverviewCWProxy {
  const _$AdminOverviewCWProxyImpl(this._value);

  final AdminOverview _value;

  @override
  AdminOverview generatedUtc(DateTime generatedUtc) =>
      this(generatedUtc: generatedUtc);

  @override
  AdminOverview activeBrands(AdminOverviewActiveBrands activeBrands) =>
      this(activeBrands: activeBrands);

  @override
  AdminOverview pendingApprovals(
    AdminOverviewPendingApprovals pendingApprovals,
  ) => this(pendingApprovals: pendingApprovals);

  @override
  AdminOverview trialing(AdminOverviewTrialing trialing) =>
      this(trialing: trialing);

  @override
  AdminOverview pastDue(AdminOverviewPastDue pastDue) => this(pastDue: pastDue);

  @override
  AdminOverview funnel(AdminOverviewFunnel funnel) => this(funnel: funnel);

  @override
  AdminOverview attentionBrands(List<AdminAttentionBrand> attentionBrands) =>
      this(attentionBrands: attentionBrands);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminOverview(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminOverview(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminOverview call({
    Object? generatedUtc = const $CopyWithPlaceholder(),
    Object? activeBrands = const $CopyWithPlaceholder(),
    Object? pendingApprovals = const $CopyWithPlaceholder(),
    Object? trialing = const $CopyWithPlaceholder(),
    Object? pastDue = const $CopyWithPlaceholder(),
    Object? funnel = const $CopyWithPlaceholder(),
    Object? attentionBrands = const $CopyWithPlaceholder(),
  }) {
    return AdminOverview(
      generatedUtc: generatedUtc == const $CopyWithPlaceholder()
          ? _value.generatedUtc
          // ignore: cast_nullable_to_non_nullable
          : generatedUtc as DateTime,
      activeBrands: activeBrands == const $CopyWithPlaceholder()
          ? _value.activeBrands
          // ignore: cast_nullable_to_non_nullable
          : activeBrands as AdminOverviewActiveBrands,
      pendingApprovals: pendingApprovals == const $CopyWithPlaceholder()
          ? _value.pendingApprovals
          // ignore: cast_nullable_to_non_nullable
          : pendingApprovals as AdminOverviewPendingApprovals,
      trialing: trialing == const $CopyWithPlaceholder()
          ? _value.trialing
          // ignore: cast_nullable_to_non_nullable
          : trialing as AdminOverviewTrialing,
      pastDue: pastDue == const $CopyWithPlaceholder()
          ? _value.pastDue
          // ignore: cast_nullable_to_non_nullable
          : pastDue as AdminOverviewPastDue,
      funnel: funnel == const $CopyWithPlaceholder()
          ? _value.funnel
          // ignore: cast_nullable_to_non_nullable
          : funnel as AdminOverviewFunnel,
      attentionBrands: attentionBrands == const $CopyWithPlaceholder()
          ? _value.attentionBrands
          // ignore: cast_nullable_to_non_nullable
          : attentionBrands as List<AdminAttentionBrand>,
    );
  }
}

extension $AdminOverviewCopyWith on AdminOverview {
  /// Returns a callable class that can be used as follows: `instanceOfAdminOverview.copyWith(...)` or like so:`instanceOfAdminOverview.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminOverviewCWProxy get copyWith => _$AdminOverviewCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminOverview _$AdminOverviewFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminOverview', json, ($checkedConvert) {
      $checkKeys(
        json,
        requiredKeys: const [
          'generatedUtc',
          'activeBrands',
          'pendingApprovals',
          'trialing',
          'pastDue',
          'funnel',
          'attentionBrands',
        ],
      );
      final val = AdminOverview(
        generatedUtc: $checkedConvert(
          'generatedUtc',
          (v) => DateTime.parse(v as String),
        ),
        activeBrands: $checkedConvert(
          'activeBrands',
          (v) => AdminOverviewActiveBrands.fromJson(v as Map<String, dynamic>),
        ),
        pendingApprovals: $checkedConvert(
          'pendingApprovals',
          (v) =>
              AdminOverviewPendingApprovals.fromJson(v as Map<String, dynamic>),
        ),
        trialing: $checkedConvert(
          'trialing',
          (v) => AdminOverviewTrialing.fromJson(v as Map<String, dynamic>),
        ),
        pastDue: $checkedConvert(
          'pastDue',
          (v) => AdminOverviewPastDue.fromJson(v as Map<String, dynamic>),
        ),
        funnel: $checkedConvert(
          'funnel',
          (v) => AdminOverviewFunnel.fromJson(v as Map<String, dynamic>),
        ),
        attentionBrands: $checkedConvert(
          'attentionBrands',
          (v) => (v as List<dynamic>)
              .map(
                (e) => AdminAttentionBrand.fromJson(e as Map<String, dynamic>),
              )
              .toList(),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminOverviewToJson(
  AdminOverview instance,
) => <String, dynamic>{
  'generatedUtc': instance.generatedUtc.toIso8601String(),
  'activeBrands': instance.activeBrands.toJson(),
  'pendingApprovals': instance.pendingApprovals.toJson(),
  'trialing': instance.trialing.toJson(),
  'pastDue': instance.pastDue.toJson(),
  'funnel': instance.funnel.toJson(),
  'attentionBrands': instance.attentionBrands.map((e) => e.toJson()).toList(),
};
