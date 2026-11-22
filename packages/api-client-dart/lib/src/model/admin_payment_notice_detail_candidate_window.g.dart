// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_payment_notice_detail_candidate_window.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminPaymentNoticeDetailCandidateWindowCWProxy {
  AdminPaymentNoticeDetailCandidateWindow from(DateTime from);

  AdminPaymentNoticeDetailCandidateWindow to(DateTime to);

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetailCandidateWindow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetailCandidateWindow(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetailCandidateWindow call({DateTime from, DateTime to});
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminPaymentNoticeDetailCandidateWindow.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminPaymentNoticeDetailCandidateWindow.copyWith.fieldName(...)`
class _$AdminPaymentNoticeDetailCandidateWindowCWProxyImpl
    implements _$AdminPaymentNoticeDetailCandidateWindowCWProxy {
  const _$AdminPaymentNoticeDetailCandidateWindowCWProxyImpl(this._value);

  final AdminPaymentNoticeDetailCandidateWindow _value;

  @override
  AdminPaymentNoticeDetailCandidateWindow from(DateTime from) =>
      this(from: from);

  @override
  AdminPaymentNoticeDetailCandidateWindow to(DateTime to) => this(to: to);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminPaymentNoticeDetailCandidateWindow(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminPaymentNoticeDetailCandidateWindow(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminPaymentNoticeDetailCandidateWindow call({
    Object? from = const $CopyWithPlaceholder(),
    Object? to = const $CopyWithPlaceholder(),
  }) {
    return AdminPaymentNoticeDetailCandidateWindow(
      from: from == const $CopyWithPlaceholder()
          ? _value.from
          // ignore: cast_nullable_to_non_nullable
          : from as DateTime,
      to: to == const $CopyWithPlaceholder()
          ? _value.to
          // ignore: cast_nullable_to_non_nullable
          : to as DateTime,
    );
  }
}

extension $AdminPaymentNoticeDetailCandidateWindowCopyWith
    on AdminPaymentNoticeDetailCandidateWindow {
  /// Returns a callable class that can be used as follows: `instanceOfAdminPaymentNoticeDetailCandidateWindow.copyWith(...)` or like so:`instanceOfAdminPaymentNoticeDetailCandidateWindow.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminPaymentNoticeDetailCandidateWindowCWProxy get copyWith =>
      _$AdminPaymentNoticeDetailCandidateWindowCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminPaymentNoticeDetailCandidateWindow
_$AdminPaymentNoticeDetailCandidateWindowFromJson(Map<String, dynamic> json) =>
    $checkedCreate('AdminPaymentNoticeDetailCandidateWindow', json, (
      $checkedConvert,
    ) {
      $checkKeys(json, requiredKeys: const ['from', 'to']);
      final val = AdminPaymentNoticeDetailCandidateWindow(
        from: $checkedConvert(
          'from',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
        to: $checkedConvert(
          'to',
          (v) => const DateOnlyConverter().fromJson(v as String),
        ),
      );
      return val;
    });

Map<String, dynamic> _$AdminPaymentNoticeDetailCandidateWindowToJson(
  AdminPaymentNoticeDetailCandidateWindow instance,
) => <String, dynamic>{
  'from': const DateOnlyConverter().toJson(instance.from),
  'to': const DateOnlyConverter().toJson(instance.to),
};
