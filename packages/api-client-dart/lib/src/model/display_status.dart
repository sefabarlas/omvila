//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:json_annotation/json_annotation.dart';

/// docs/alan/durum-makinesi.md § Arayüz gösterimi
enum DisplayStatus {
  @JsonValue(r'PendingApproval')
  pendingApproval(r'PendingApproval'),
  @JsonValue(r'DepositPending')
  depositPending(r'DepositPending'),
  @JsonValue(r'Confirmed')
  confirmed(r'Confirmed'),
  @JsonValue(r'CheckedIn')
  checkedIn(r'CheckedIn'),
  @JsonValue(r'Completed')
  completed(r'Completed'),
  @JsonValue(r'Cancelled')
  cancelled(r'Cancelled'),
  @JsonValue(r'NoShow')
  noShow(r'NoShow'),
  @JsonValue(r'unknown_default_open_api')
  unknownDefaultOpenApi(r'unknown_default_open_api');

  const DisplayStatus(this.value);

  final String value;

  @override
  String toString() => value;
}
