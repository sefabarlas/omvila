//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

enum CancellationReason {
  @JsonValue(r'PlanChanged')
  planChanged(r'PlanChanged'),
  @JsonValue(r'Sick')
  sick(r'Sick'),
  @JsonValue(r'WantsDifferentTime')
  wantsDifferentTime(r'WantsDifferentTime'),
  @JsonValue(r'Other')
  other(r'Other'),
  @JsonValue(r'CustomerRequested')
  customerRequested(r'CustomerRequested'),
  @JsonValue(r'StaffUnavailable')
  staffUnavailable(r'StaffUnavailable'),
  @JsonValue(r'DeclinedBySalon')
  declinedBySalon(r'DeclinedBySalon'),
  @JsonValue(r'ApprovalExpired')
  approvalExpired(r'ApprovalExpired'),
  @JsonValue(r'DepositNotReceived')
  depositNotReceived(r'DepositNotReceived'),
  @JsonValue(r'AccountDeleted')
  accountDeleted(r'AccountDeleted'),
  @JsonValue(r'BrandReadOnly')
  brandReadOnly(r'BrandReadOnly'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const CancellationReason(this.value);

  final String value;

  @override
  String toString() => value;
}
