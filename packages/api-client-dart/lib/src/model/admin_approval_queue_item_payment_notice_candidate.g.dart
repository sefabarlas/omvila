// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'admin_approval_queue_item_payment_notice_candidate.dart';

// **************************************************************************
// CopyWithGenerator
// **************************************************************************

abstract class _$AdminApprovalQueueItemPaymentNoticeCandidateCWProxy {
  AdminApprovalQueueItemPaymentNoticeCandidate bankTransactionId(
    String bankTransactionId,
  );

  AdminApprovalQueueItemPaymentNoticeCandidate verdict(MatchVerdict verdict);

  AdminApprovalQueueItemPaymentNoticeCandidate codeFound(bool codeFound);

  AdminApprovalQueueItemPaymentNoticeCandidate amountMatches(
    bool amountMatches,
  );

  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItemPaymentNoticeCandidate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItemPaymentNoticeCandidate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItemPaymentNoticeCandidate call({
    String bankTransactionId,
    MatchVerdict verdict,
    bool codeFound,
    bool amountMatches,
  });
}

/// Proxy class for `copyWith` functionality. This is a callable class and can be used as follows: `instanceOfAdminApprovalQueueItemPaymentNoticeCandidate.copyWith(...)`. Additionally contains functions for specific fields e.g. `instanceOfAdminApprovalQueueItemPaymentNoticeCandidate.copyWith.fieldName(...)`
class _$AdminApprovalQueueItemPaymentNoticeCandidateCWProxyImpl
    implements _$AdminApprovalQueueItemPaymentNoticeCandidateCWProxy {
  const _$AdminApprovalQueueItemPaymentNoticeCandidateCWProxyImpl(this._value);

  final AdminApprovalQueueItemPaymentNoticeCandidate _value;

  @override
  AdminApprovalQueueItemPaymentNoticeCandidate bankTransactionId(
    String bankTransactionId,
  ) => this(bankTransactionId: bankTransactionId);

  @override
  AdminApprovalQueueItemPaymentNoticeCandidate verdict(MatchVerdict verdict) =>
      this(verdict: verdict);

  @override
  AdminApprovalQueueItemPaymentNoticeCandidate codeFound(bool codeFound) =>
      this(codeFound: codeFound);

  @override
  AdminApprovalQueueItemPaymentNoticeCandidate amountMatches(
    bool amountMatches,
  ) => this(amountMatches: amountMatches);

  @override
  /// This function **does support** nullification of nullable fields. All `null` values passed to `non-nullable` fields will be ignored. You can also use `AdminApprovalQueueItemPaymentNoticeCandidate(...).copyWith.fieldName(...)` to override fields one at a time with nullification support.
  ///
  /// Usage
  /// ```dart
  /// AdminApprovalQueueItemPaymentNoticeCandidate(...).copyWith(id: 12, name: "My name")
  /// ````
  AdminApprovalQueueItemPaymentNoticeCandidate call({
    Object? bankTransactionId = const $CopyWithPlaceholder(),
    Object? verdict = const $CopyWithPlaceholder(),
    Object? codeFound = const $CopyWithPlaceholder(),
    Object? amountMatches = const $CopyWithPlaceholder(),
  }) {
    return AdminApprovalQueueItemPaymentNoticeCandidate(
      bankTransactionId: bankTransactionId == const $CopyWithPlaceholder()
          ? _value.bankTransactionId
          // ignore: cast_nullable_to_non_nullable
          : bankTransactionId as String,
      verdict: verdict == const $CopyWithPlaceholder()
          ? _value.verdict
          // ignore: cast_nullable_to_non_nullable
          : verdict as MatchVerdict,
      codeFound: codeFound == const $CopyWithPlaceholder()
          ? _value.codeFound
          // ignore: cast_nullable_to_non_nullable
          : codeFound as bool,
      amountMatches: amountMatches == const $CopyWithPlaceholder()
          ? _value.amountMatches
          // ignore: cast_nullable_to_non_nullable
          : amountMatches as bool,
    );
  }
}

extension $AdminApprovalQueueItemPaymentNoticeCandidateCopyWith
    on AdminApprovalQueueItemPaymentNoticeCandidate {
  /// Returns a callable class that can be used as follows: `instanceOfAdminApprovalQueueItemPaymentNoticeCandidate.copyWith(...)` or like so:`instanceOfAdminApprovalQueueItemPaymentNoticeCandidate.copyWith.fieldName(...)`.
  // ignore: library_private_types_in_public_api
  _$AdminApprovalQueueItemPaymentNoticeCandidateCWProxy get copyWith =>
      _$AdminApprovalQueueItemPaymentNoticeCandidateCWProxyImpl(this);
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdminApprovalQueueItemPaymentNoticeCandidate
_$AdminApprovalQueueItemPaymentNoticeCandidateFromJson(
  Map<String, dynamic> json,
) => $checkedCreate('AdminApprovalQueueItemPaymentNoticeCandidate', json, (
  $checkedConvert,
) {
  $checkKeys(
    json,
    requiredKeys: const [
      'bankTransactionId',
      'verdict',
      'codeFound',
      'amountMatches',
    ],
  );
  final val = AdminApprovalQueueItemPaymentNoticeCandidate(
    bankTransactionId: $checkedConvert('bankTransactionId', (v) => v as String),
    verdict: $checkedConvert(
      'verdict',
      (v) => $enumDecode(
        _$MatchVerdictEnumMap,
        v,
        unknownValue: MatchVerdict.unknownDefaultOpenApi,
      ),
    ),
    codeFound: $checkedConvert('codeFound', (v) => v as bool),
    amountMatches: $checkedConvert('amountMatches', (v) => v as bool),
  );
  return val;
});

Map<String, dynamic> _$AdminApprovalQueueItemPaymentNoticeCandidateToJson(
  AdminApprovalQueueItemPaymentNoticeCandidate instance,
) => <String, dynamic>{
  'bankTransactionId': instance.bankTransactionId,
  'verdict': _$MatchVerdictEnumMap[instance.verdict]!,
  'codeFound': instance.codeFound,
  'amountMatches': instance.amountMatches,
};

const _$MatchVerdictEnumMap = {
  MatchVerdict.codeMatches: 'CodeMatches',
  MatchVerdict.checkRequired: 'CheckRequired',
  MatchVerdict.unknownDefaultOpenApi: 'unknown_default_open_api',
};
